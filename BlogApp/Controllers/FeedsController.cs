using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BlogApp.Controllers.CommentController;
using static BlogApp.Controllers.HomeController;

namespace BlogApp.Controllers
{
    [Authorize]
    public class FeedsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContext _authContext;
        private readonly UserManager<IdentityUser> _userManager;

        public Boolean LikeStatus = true;

        public class LikeModel
        {
            public int TotalLikeCount { get; set; }
            public Boolean Status { get; set; }
        }

        public FeedsController(IWebHostEnvironment hostingEnvironment, AppDbContext authcontext,UserManager<IdentityUser> userManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _authContext = authcontext;
            _userManager = userManager;
        }


        public IActionResult Index()
        {

            GetFeeds();
            return View();
        }


        [HttpPost]
        [DisableRequestSizeLimit]

        public async Task<IActionResult> UploadFeedContent()
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var files = Request.Form.Files;
            var description = Request.Form["description"];
            var title = Request.Form["title"];
            var keywords = Request.Form["keywords"];

            var fileUrls = new List<string>();
            var fileNames = new List<string>();

            var total_file_string = new string("");
            var total_file_names = new string("");


            if (files != null && files.Count > 0)
            {

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);

                        // Validate file type (e.g., allow only image files)
                        if (!IsImageFile(fileName))
                        {
                            return BadRequest(new { Message = "Invalid file type. Only image files are allowed." });
                        }

                        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", fileName);

                        try
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                            fileUrls.Add(fileUrl);
                            fileNames.Add(fileName);
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions that occur during file processing
                            return BadRequest(new { Message = "Error uploading files: " + ex.Message });
                        }
                    }
                }

                total_file_string = string.Join(",", fileUrls);
                total_file_names = string.Join(",", fileNames);

                var newfeed = new Feed
                {
                   FeedFileName = total_file_names,
                    FeedFilePath = total_file_string,
                    FeedDescription = description,
                    FeedTitle = title,
                    FeedKeywords = keywords,
                 
                    UserId = userId,
                    CreatedAt = DateTime.Now
                    

                };



                _authContext.Feeds.Add(newfeed);
                await _authContext.SaveChangesAsync();

                return RedirectToAction("");

            }
            else
            {
                return BadRequest(new { Message = "No files were uploaded." });

            }
        }

        private bool IsImageFile(string fileName)
        {
            // Define a list of allowed image file extensions (adjust as needed)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileExtension = Path.GetExtension(fileName);

            return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        [HttpGet]
        public IActionResult GetFeeds()
        {
            var feeds = _authContext.Feeds.ToList();

            if (feeds == null || !feeds.Any())
            {
                return NotFound();
            }

            return View(feeds);
        }

        [HttpGet]
        public IActionResult GetFeedLikes(int id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

   
            var likedFeeds = _authContext.FeedLikes.Where(like => like.FeedID == id).ToList();


            var currentFeed = _authContext.Feeds.FirstOrDefault(feed => feed.FeedId == id);

            if (currentFeed == null)
            {
                return NotFound();
            }

            currentFeed.Likes = likedFeeds.Sum(feed => feed.FeedTotalLikeCount);
            _authContext.SaveChanges();

            var userLikedPost = likedFeeds.FirstOrDefault(feed => feed.UserId == userId);


            var likeStatus = userLikedPost?.IsLiked ?? false;

            // Create a model with updated like information
            var updatedLikedPosts = new LikeModel
            {
                TotalLikeCount = (int)currentFeed.Likes,
                Status = likeStatus
            };

            return Ok(updatedLikedPosts);
        }


        public async Task<IActionResult> UpdateFeedLikes(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            

            if (id == null || _authContext.Blogs == null)
            {
                return NotFound();
            }

            var like = await _authContext.FeedLikes.FirstOrDefaultAsync(m => m.FeedID == id && m.UserId == userId);

            if (like == null)
            {
                var newLike = new FeedLike
                {
                    FeedID = id,
                    UserId = userId,
                    IsLiked = true,
                    FeedTotalLikeCount = 1

                };

                await _authContext.AddAsync(newLike);
                await _authContext.SaveChangesAsync();
            }
            else if (like.IsLiked == true)
            {
                like.FeedTotalLikeCount--;
                like.IsLiked = false;
                await _authContext.SaveChangesAsync();
            }
            else
            {
                like.FeedTotalLikeCount++;
                like.IsLiked = true;
                await _authContext.SaveChangesAsync();
            }



            return RedirectToAction("Index", "Feeds", new { id = id });

        }

        [HttpGet]
        public IActionResult GetUserName(string id) 
        {
            var userInfo = _authContext.UserInfo.FirstOrDefault(m => m.UserId == id);

            if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.FullName))
            {
                return Ok(userInfo.FullName);
            }

            return Ok("Unknown");
        }



    }
}
