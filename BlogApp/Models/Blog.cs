using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        public string? BlogTitle { get; set; }

        public string? BlogSubtitle { get; set; }

        public string? BlogKeywords { get; set; }

        public string? BlogDescription { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public string? UserId { get; set; }

        public int? Likes { get; set; }

        public string? Comments { get; set; }

        public DateTime? CreatedAt { get; set; } = default(DateTime?);
     
        
    }
}
