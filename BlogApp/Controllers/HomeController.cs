using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContext _authContext;
        private readonly UserManager<IdentityUser> _userManager;

        public Boolean LikeStatus = true;

        public class LikeModel
        {
            public int TotalLikeCount { get; set; }
            public Boolean Status { get; set; }
        }



        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, AppDbContext authcontext, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _authContext = authcontext;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return _authContext.Blogs != null ?
                      View(await _authContext.Blogs.ToListAsync()) :
                      Problem("Entity set 'AppDbContext.Blogs'  is null.");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userdetails = _authContext.UserInfo.FirstOrDefault(x => x.UserId == userId);

            return View(userdetails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> BlogDetails(int? id)
        {
            if (id == null || _authContext.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _authContext.Blogs.FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        public async Task<IActionResult> UpdateLikes(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id == null || _authContext.Blogs == null)
            {
                return NotFound();
            }

            var like = await _authContext.Likes.FirstOrDefaultAsync(m => m.BlogID == id && m.UserId == userId);
            if (like == null)
            {
                var newLike = new Likes
                {
                    BlogID = id,
                    UserId = userId,
                    IsLiked = true,
                    TotalLikeCount = 1

                };

                await _authContext.AddAsync(newLike);
                await _authContext.SaveChangesAsync();
            }
            else if (like.IsLiked == true)
            {
                like.TotalLikeCount--;
                like.IsLiked = false;
                await _authContext.SaveChangesAsync();
            }
            else
            {
                like.TotalLikeCount++;
                like.IsLiked = true;
                await _authContext.SaveChangesAsync();
            }

            

            return RedirectToAction("BlogDetails", new { id = id });

        } 



        [HttpGet]
        public IActionResult GetLikes(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var likedblogs = _authContext.Likes.Where(like => like.BlogID == id).ToList();

            var currentblog = _authContext.Blogs.FirstOrDefault(m => m.BlogId == id);

            var userlikedpost =  _authContext.Likes.FirstOrDefault(m => m.BlogID == id && m.UserId == userId);


            if (likedblogs == null)
            {
                return NotFound();
            }

            foreach (var blog in likedblogs)
            {
                currentblog.Likes = (int)(currentblog.Likes + blog.TotalLikeCount);
            }

            if(userlikedpost != null)
            {
                LikeStatus = (bool)userlikedpost.IsLiked;
            }
            else
            {
                LikeStatus =false;
            }

            var updatedLikedPosts = new LikeModel
            {
                TotalLikeCount = (int)currentblog.Likes,
                Status = LikeStatus,
            };

            return Ok(updatedLikedPosts);
        }
        





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}