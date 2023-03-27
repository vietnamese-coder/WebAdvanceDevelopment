using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers
{


    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        /* public IActionResult Index() {
             ViewBag.CurrentTime = DateTime.Now.ToString("HH:mm:ss");
             return View();
         }*/

        public async Task<IActionResult> Index(
            [FromQuery(Name = "k")] string keyword = null,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {

            // Tạo đối tượng chưa các điều kiện truy vấn
            var postQuery = new PostQuery()
            {
                // Chỉ lấy những bài viết có trạn thái Published
                PublishedOnly = true
            };

            // Truy vấn các bài viết theo điều kiện đã tạo
            var postsList = await _blogRepository
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            // Lưu lại điều kiện truy vấn để hiện thị trong View
            ViewBag.PostQuery = postQuery;

            // Truyền danh sách bài viết View để render ra HTML
            return View(postsList);
        }

        public IActionResult About()
            => View();

        public IActionResult Contact()
            => View();

        public IActionResult Rss()
            => Content("Nội dung sẽ được cập nhật");


    }
}
