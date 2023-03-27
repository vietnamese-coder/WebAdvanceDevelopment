//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TatBlog.Data.Contexts;
//using TatBlog.Data.Seeders;
//using TatBlog.Services.Blogs;
//using TatBlog.WebApp.Extensions;
//using NLog;
//using NLog.Web;
//using TatBlog.WebApp.Validations;

//var builder = WebApplication.CreateBuilder(args); {
//    builder
//           .ConfigureMVC()
//           .ConfigureNLog()
//           .ConfigureServices()
//           .ConfigureMapster()
//           .ConfigureFluentValidation();
//}


//var app = builder.Build(); {
//    app.UseRequestPipeLine();
//    app.UseBlogRoutes();
//    app.UseDataSeeder();
//}

//app.Run();

//Anh dinh

using Microsoft.EntityFrameworkCore;
using System.Net;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Extensions;
using TatBlog.WebApp.Mapster;
using TatBlog.WebApp.Validations;

var builder = WebApplication.CreateBuilder(args);
{
    builder
    .ConfigureMVC()
    .ConfigureNLog()
    .ConfigureServices()
    .ConfigureMapster()
    .ConfigureFluentValidation();

    /*Them cac dich vu duoc yeu cau boi MVC Framework*/
    builder.Services.AddControllersWithViews();

    /*Dang ky cac dich vu voi DI Container*/
    builder.Services.AddDbContext<BlogDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IBlogRepository, BlogRepository>();
    builder.Services.AddScoped<IDataSeeder, DataSeeder>();

}
var app = builder.Build();
{
    /*Cau hinh HTTP Request pipeline*/

    /*Them middleware de hien thi thong bao loi*/

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Blog/Error");
        /*Them middleware cho viec ap dung HSTS (them header
            Strict-TransportContext-Security vao HTTP Response)*/

        app.UseHsts();
    }

    /*Them middleware de chuyen huong HTTP sang HTTPS*/
    app.UseHttpsRedirection();

    /*Them middleware phuc vu cac yeu cau lien quan toi cac
        tap tin noi dung tinh nhu hinh anh, css,...*/
    app.UseStaticFiles();

    /*Them middleware lua chon endpoint phu hop nhat de 
        xu ly mot HTTP request*/

    app.UseRouting();



    /*Dinh nghia route template, route constraint cho cac endpoints
        ket hop voi cac action trong cac controller*/

    /*    app.MapControllerRoute(
        name: "posts-by-category",
        pattern: "blog/category/{slug}",
        defaults: new { controller = "Blog", action = "Category" });
        app.MapControllerRoute(
        name: "posts-by-tag",
        pattern: "blog/tag/{slug}",
        defaults: new { controller = "Blog", action = "Tag" });
        app.MapControllerRoute(
        name: "single-post",
        pattern: "blog/post/{year:int}/{month:int}/{day:int}/{title}",
        defaults: new { controller = "Blog", action = "Post" });*/

    //app.MapControllerRoute(
    //name: "single-post",
    //pattern: "blog/author/{name}",
    //defaults: new { controller = "Blog", action = "Author" });

    //app.MapControllerRoute(
    //name: "single-title",
    //pattern: "blog/title/{title}",
    //defaults: new { controller = "Blog", action = "Title" });

    //app.MapControllerRoute(
    //name: "single-category-name",
    //pattern: "blog/category/{name}",
    //defaults: new { controller = "Blog", action = "Category" });


    //app.MapControllerRoute(
    //    name: "default",
    //    pattern: "{controller=Blog}/{action=Index}/{id?}");

    app.UseBlogRoutes();

}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    seeder.Initialize();
}


/*app.MapGet("/", () => "Hello World!");*/

app.Run();

























/*var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try {

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment()) {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception exception) {
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally {
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}

*//*app.MapControllerRoute(
    name: "posts-by-category",
    pattern: "blog/category/{slug}",
    defaults: new { controller = "Blog", action = "Category" });

app.MapControllerRoute(
name: "posts-by-tag",
pattern: "blog/tag/{slug}",
defaults: new { controller = "Blog", action = "Tag" });

app.MapControllerRoute(
name: "single-post",
pattern: "blog/post/{year:int}/{month:int}/{day:int}/{title}",
defaults: new { controller = "Blog", action = "Post" });

app.MapControllerRoute(
name: "single-post",
pattern: "blog/author/{name}",
defaults: new { controller = "Blog", action = "Author" });

app.MapControllerRoute(
name: "single-title",
pattern: "blog/title/{title}",
defaults: new { controller = "Blog", action = "Title" });

app.MapControllerRoute(
name: "single-category-name",
pattern: "blog/category/{name}",
defaults: new { controller = "Blog", action = "Category" });*/



