using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;
using TatBlog.Data.Mappings;

namespace TatBlog.Services.Blogs; 
public class BlogRepository : IBlogRepository {
    private readonly BlogDbContext _context;

    public BlogRepository(BlogDbContext context) {
        _context = context;
    }

    // Tìm bài viết có tên định danh là 'slug'
    // và được đăng vào tháng 'month' năm 'year'
    public async Task<Post> GetPostAsync (
        int year,
        int month,
        string slug,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author);

        if (year > 0) {
            postsQuery = postsQuery.Where (x => x.PostedDate.Year == year);
        }

        if (month > 0) {
            postsQuery = postsQuery.Where (x => x.PostedDate.Month == month);
        }

        if (!string.IsNullOrWhiteSpace(slug)) {
            postsQuery = postsQuery.Where (x => x.UrlSlug == slug);
        }

        return await postsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    // Tìm top N bài viết phổ được nhiều người xem nhất
    public async Task<IList<Post>> GetPopularArticlesAsync(
        int numPosts, CancellationToken cancellationToken = default) {
        return await _context.Set<Post>()
            .Include(x => x.Author)
            .Include(x => x.Category)
            .OrderByDescending (p => p.ViewCount)
            .Take(numPosts)
            .ToListAsync(cancellationToken);
    }

    // Kiểm tra xem tên định danh của bài viết đã có hay chưa
    public async Task<bool> IsPostSlugExistedAsync(
        int postId, 
        string slug, 
        CancellationToken cancellationToken = default) {
        return await _context.Set<Post>()
            .AnyAsync(x => x.Id != postId && x.UrlSlug == slug,
                cancellationToken);
    }

    // Tăng số lượt xem của một bài viết
    public async Task IncreaseViewCountAsync(
        int postId,
        CancellationToken cancellationToken = default) { 
        await _context.Set<Post>()
           .Where (x => x.Id == postId)
           .ExecuteUpdateAsync (p => 
                p.SetProperty(x=> x.ViewCount, x => x.ViewCount + 1), cancellationToken);
    }


    // Lấy danh sách chuyên mục và số lượng bài viết
    // nằm thuộc từng chuyên mục/ chủ đề
    public async Task<IList<CategoryItem>> GetCategoriesAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken = default) {

        IQueryable<Category> categories = _context.Set<Category>();
        if (showOnMenu) {
            categories = categories.Where (x => x.ShowOnMenu);
        }

        return await categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count( p => p.Published)
            })
        .ToListAsync(cancellationToken);
    }

    //public Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
    //{
    //    throw new NotImplementedException();
    //}

    public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default) {
        var tagQuery = _context.Set<Tag>()
            .Select(x => new TagItem() {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description= x.Description,
                PostCount= x.Posts.Count(p => p.Published)
            });
        return await tagQuery
            .ToPagedListAsync(pagingParams,cancellationToken);
    }


    private IQueryable<Post> FilterPosts(PostQuery condition) {
        IQueryable<Post> posts = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags);

        if (condition.Year > 0) {
            posts = posts.Where(x => x.PostedDate.Year == condition.Year);
        }

        if (condition.Month > 0) {
            posts = posts.Where(x => x.PostedDate.Month == condition.Month);
        }

        if (!string.IsNullOrWhiteSpace(condition.TitleSlug)) {
            posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
        }

        return posts;
    }

    public async Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery condition,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) {
        return await FilterPosts(condition).ToPagedListAsync(
            pageNumber, pageNumber,
            nameof(Post.PostedDate), "DESC",
            cancellationToken);
    }
}
