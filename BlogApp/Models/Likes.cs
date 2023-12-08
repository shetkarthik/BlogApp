using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Likes
    {
        [Key]
        public int? LikeId { get; set; }

        public string? UserId { get; set; }

        public int? BlogID { get; set; }

        public int? TotalLikeCount { get; set; }

        public Boolean? IsLiked { get; set; } = false;
    }
}
