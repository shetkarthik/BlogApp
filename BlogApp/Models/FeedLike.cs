using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class FeedLike
    {
        [Key]
        public int? FeedLikeId { get; set; }

        public string? UserId { get; set; }

        public int? FeedID { get; set; }

        public int? FeedTotalLikeCount { get; set; }

        public Boolean? IsLiked { get; set; } = false;
    }
}
