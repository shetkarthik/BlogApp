using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Comment
    {
        [Key]
        public int? CommentId { get; set; }

        public string? UserId { get; set; }

        public int? BlogID { get; set; }

        public string? CommentString { get; set; }
    }
}
