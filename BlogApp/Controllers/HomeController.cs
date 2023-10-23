using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContext _authContext;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, AppDbContext authcontext)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _authContext = authcontext;
        }

        public async Task<IActionResult> Index()
        {
            return _authContext.Blogs != null ?
                      View(await _authContext.Blogs.ToListAsync()) :
                      Problem("Entity set 'AppDbContext.Blogs'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}