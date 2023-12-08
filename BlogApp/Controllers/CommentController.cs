using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContext _authContext;
        private readonly UserManager<IdentityUser> _userManager;

        public Boolean LikeStatus = true;

        public class CommentModel
        {
            public int TotalCommentCount { get; set; }
            public List<Comment> Comments { get; set; }
        }

        public class FeedCommentModel
        {
            public int TotalCommentCount { get; set; }

            public List<FeedComments> Comments { get; set; }
        }



        public CommentController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, AppDbContext authcontext, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _authContext = authcontext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComments(int id,string comment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id == null || _authContext.Blogs == null)
            {
                return NotFound();
            }

            var newComment = new Comment
            {
                BlogID = id,
                UserId = userId,
                CommentString = comment
            };

           
                await _authContext.AddAsync(newComment);
                await _authContext.SaveChangesAsync();

            return RedirectToAction("BlogDetails", "Home", new { id = id });


        }


        [HttpGet]
        public IActionResult GetComments(int id)
        {
            List<Comment> comments = _authContext.Comments.Where(m => m.BlogID == id).ToList();

            if (comments == null || comments.Count == 0)
            {
                return NotFound();
            }

            var updatedCommentPosts = new CommentModel
            {
                TotalCommentCount = comments.Count,
                Comments = comments,
            };

            ViewBag.Comments = updatedCommentPosts;

            return Ok(updatedCommentPosts);
        }


        [HttpGet]
        public IActionResult GetFeedComments(int id)
        {
            List<FeedComments> comments = _authContext.FeedComments.Where(m => m.FeedID == id).ToList();

            if (comments == null || comments.Count == 0)
            {
                return Json("");
            }

            var updatedCommentPosts = new FeedCommentModel
            {
                TotalCommentCount = comments.Count,
                Comments = comments,
            };

            return Json(updatedCommentPosts);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateFeedComments(int id, string comment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id == null || _authContext.Feeds == null)
            {
                return NotFound();
            }

            var newComment = new FeedComments
            {
                FeedID = id,
                UserId = userId,
                FeedCommentString = comment
            };


            await _authContext.AddAsync(newComment);
            await _authContext.SaveChangesAsync();

           return RedirectToAction("Index", "Feeds", new { id = id });

          
        }




    }
}
