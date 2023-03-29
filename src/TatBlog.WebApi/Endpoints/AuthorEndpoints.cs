﻿using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Models;
using TatBlog.Core.Contracts;
using MapsterMapper;
using Mapster;
using TatBlog.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using TatBlog.WebApi.Extensions;
using TatBlog.Services.Media;

namespace TatBlog.WebApi.Endpoints; 
public static class AuthorEndpoints {
    public static WebApplication  MapAuthorEndpoints(
        this WebApplication app) 
    {
        var routeGroupBuilder = app.MapGroup("/api/authors");

        routeGroupBuilder.MapGet("/", GetAuthors)
            .WithName ("GetAuthors")
            .Produces<PaginationResult<AuthorItem>>();

        routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
            .WithName ("GetAuthorById")
            .Produces<AuthorItem>()
            .Produces(404);

        routeGroupBuilder.MapGet(
            "/{slug:regex(^[a-z0-9]+$)}/posts",
            GetPostsByAuthorSlug)
            .WithName ("GetPostsByAuthorSlug")
            .Produces<PaginationResult<PostDto>>();

        routeGroupBuilder.MapPost("/", AddAuthor)
            .WithName("AddNewAuthor")
            .Produces(201)
            .Produces(400)
            .Produces(409);
            
        routeGroupBuilder.MapPost("/{id:int}/avatar", SetAuthorPicture)
            .WithName ("SetAuthorPicture")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<string>()
            .Produces(400);

        routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
            .WithName ("UpdateAnAuthor")
            .Produces(204)
            .Produces(400)
            .Produces(409);

        routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
            .WithName("UpdateAnAuthor")
            .Produces(204)
            .Produces(400);

        return app;
    }

    private static async Task<IResult> GetAuthors(
        [AsParameters] AuthorFilterModel model,
        IAuthorRepository authorRepository) {
        var authorsList = await authorRepository
            .GetPagedAuthorsAsync(model, model.Name);

        var paginationResult = 
            new PaginationResult<AuthorItem>(authorsList);
        return Results.Ok(paginationResult);
    }

    private static async Task<IResult> GetAuthorDetails(
        int id,
        IAuthorRepository authorRepository,
        IMapper mapper) {
        var author = await authorRepository.GetCachedAuthorByIdAsync(id);
        return author == null
            ? Results.NotFound($"Không tìm thấy tác giả có mã số {id}")
            :Results.Ok(mapper.Map<AuthorItem>(author));
    }

    private static async Task<IResult> GetPostsByAuthorId(
        int id,
        [AsParameters] PagingModel pagingModel,
        IBlogRepository blogRepository) {
        var postQuery = new PostQuery() {
            AuthorId = id,
            PublishedOnly = true,
        };

        var postsList = await blogRepository.GetPagedPostsAsync(
            postQuery, pagingModel,
            posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);
        
        return Results.Ok(paginationResult);
    }

    private static async Task<IResult> GetPostsByAuthorSlug(
        [FromRoute] string slug,
        [AsParameters] PagingModel pagingModel,
        IBlogRepository blogRepository) {
        var postQuery = new PostQuery() {
            AuthorSlug = slug,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPagedPostsAsync(
            postQuery, pagingModel,
            posts => posts.ProjectToType<PostDto>());
        var paginationResult = new PaginationResult<PostDto>(postsList);

        return Results.Ok(paginationResult);
    }

    private static async Task<IResult> AddAuthor(
        AuthorEditModel model,
        IValidator<AuthorEditModel> validator,
        IAuthorRepository authorRepository,
        IMapper mapper) {
        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid) { 
            return Results.BadRequest(
                validationResult.Errors.ToResponse());
        }

        if (await authorRepository
                .IsAuthorSlugExistedAsync(0, model.UrlSlug)) {
            return Results.Conflict(
                $"Slug '{model.UrlSlug}' đã được sử dụng");
        }

        var author = mapper.Map<Author>(model);
        await authorRepository.AddOrUpdateAsync(author);

        return Results.CreatedAtRoute(
            "GetAuthorById", new {author.Id},
            mapper.Map<AuthorItem>(author));
    }

    private static async Task<IResult> SetAuthorPicture(
        int id, IFormFile imageFile,
        IAuthorRepository authorRepository,
        IMediaManager mediaManager) {
        var imageUrl = await mediaManager.SaveFileAsync(
            imageFile.OpenReadStream(),
            imageFile.FileName, imageFile.ContentType);

        if (string.IsNullOrWhiteSpace(imageUrl)) {
            return Results.BadRequest("Không lưu được tập tin");
        }

        await authorRepository.SetImageUrlAsync (id, imageUrl);
        return Results.Ok(imageUrl);
    }

    private static async Task<IResult> UpdateAuthor(
        int id, AuthorEditModel model,
        IValidator<AuthorEditModel> validator,
        IAuthorRepository authorRepository,
        IMapper mapper) {
        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid) {
            return Results.BadRequest(
                validationResult.Errors.ToResponse());
        }

        if (await authorRepository
                .IsAuthorSlugExistedAsync(id, model.UrlSlug)) {
            return Results.Conflict(
                $"Slug '{model.UrlSlug} 'đã được sử dụng");
        }

        var author = mapper.Map<Author>(model);
        author.Id = id;

        return await authorRepository.AddOrUpdateAsync(author)
            ? Results.NoContent()
            : Results.NotFound();
    }

    private static async Task<IResult> DeleteAuthor(
        int id, IAuthorRepository authorRepository) {
        return await authorRepository.DeleteAuthorAsync(id)
            ? Results.NoContent()
            : Results.NotFound($"Could not find author with id  = {id}");
    }
}