﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders;

public class DataSeeder : IDataSeeder {
    private readonly BlogDbContext _dbContext;

    public DataSeeder(BlogDbContext dbContext) {
        _dbContext = dbContext;
    }   

    public void Initialize() {
        _dbContext.Database.EnsureCreated();

        if (_dbContext.Posts.Any()) return;

        var authors = AddAuthors();
        var categories = AddCategories();
        var tags =  AddTags();
        var posts = AddPosts (authors, categories, tags);

    }


    private IList<Author> AddAuthors() { 
        var authors = new List<Author>() {
            new() {
                FullName = "Jason Mouth",
                UrlSlug = "jason-mouth",
                Email = "json@gamil",
                JoinedDate = new DateTime(2022,10,21)
            },
            new() {
                FullName = "Jessica Wonder",
                UrlSlug = "jessica-wonder",
                Email = "jessica665@motip.com",
                JoinedDate = new DateTime(2020, 4, 19)
            }
        };

        _dbContext.Authors.AddRange(authors);
        _dbContext.SaveChanges();

        return authors;
    }

    

    private IList<Category> AddCategories() {
        var categories = new List<Category>() {
            new() {Name = ".NET Core", Description = ".NET Core", UrlSlug = "Hi 1"},
            new() {Name = "Architecture", Description = ".Architecture", UrlSlug = "Hi 2"},
            new() {Name = "Messaging", Description = "Messaging", UrlSlug = "Hi 3"},
            new() {Name = "OOP", Description = "OOP", UrlSlug = "Hi 4"},
            new() {Name = "Design Patterns", Description = "Design Patterns", UrlSlug = "Hi 5"},    
        };

        _dbContext.AddRange(categories);
        _dbContext.SaveChanges();

        return categories;
    }

    private IList<Tag> AddTags() { 
        var tags = new List<Tag>() {
            new() {Name = "Google", Description = "Google applications", UrlSlug = "Hi 6"},
            new() {Name = "ASP.NET MVC", Description = "ASP.NET MVC", UrlSlug = "Hi 7"},
            new() {Name = "Razor Page", Description = "Razor Page", UrlSlug = "Hi 8"},
            new() {Name = "Blazor", Description = "Blazor", UrlSlug = "Hi 9"},
            new() {Name = "Deep Learning", Description = "Deep Learning", UrlSlug = "Hi 10"},
            new() {Name = "Neural Network", Description = "Neural Network", UrlSlug = "Hi 11"},
        };

        _dbContext.AddRange(tags);
        _dbContext.SaveChanges();

        return tags;
    }

    private IList<Post> AddPosts(
        IList<Author> authors,
        IList<Category> categories,
        IList<Tag> tags)
    {     
        var posts = new List<Post>()
        {
            new() 
            {
                Title = "ASP.NET Core Diagnostic Scenarios",
                ShortDescription = "David and friends has a great relationship",
                Description = "Here's a few great DON'T and DO examples",
                Meta = "David and friends has a great relationship",
                UrlSlug = "aspnet-core-diagnostic-scenarios",
                Published = true,
                PostedDate = new DateTime (2021, 9, 30, 10, 20, 0),
                ModifiedDate = null,
                ViewCount = 10,
                Author = authors[0],
                Category = categories[0],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            }
        };

        _dbContext.AddRange(posts);
        _dbContext.SaveChanges();

        return posts;
    }


}