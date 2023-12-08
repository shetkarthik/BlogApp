using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Feed
    {
        [Key]
        public int FeedId { get; set; }

        public string? FeedTitle { get; set; }

        public string? FeedKeywords { get; set; }

        public string? FeedDescription { get; set; }
        public string? FeedFileName { get; set; }
        public string? FeedFilePath { get; set; }

        public string? UserId { get; set; }

        public int? Likes { get; set; }

        public string? Comments { get; set; }

        public DateTime? CreatedAt { get; set; } = default(DateTime?);
    }
}
