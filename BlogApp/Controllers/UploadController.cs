using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContext _authContext;

        public UploadController(IWebHostEnvironment hostingEnvironment, AppDbContext authcontext)
        {
            _hostingEnvironment = hostingEnvironment;
            _authContext = authcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [DisableRequestSizeLimit]

        public async Task<IActionResult> UploadContent()
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var files = Request.Form.Files;
            var description  = Request.Form ["description"];
            var title = Request.Form["title"];
            var subtitle = Request.Form["subtitle"];
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

                var newblog = new Blog
                {
                    FileName = total_file_names,
                    FilePath = total_file_string,
                    BlogDescription = description,
                    BlogTitle = title,
                    BlogKeywords = keywords,
                    BlogSubtitle = subtitle,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Likes = 0,
                   
                };

               

                _authContext.Blogs.Add(newblog);
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
        public IActionResult GetBlogs()
        {
            var blogs = _authContext.Blogs.ToList();

            if (blogs == null || !blogs.Any())
            {
                return NotFound();
            }

            return Json(blogs);
        }


    }
}
