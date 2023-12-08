using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class FeedComments
    {


            [Key]
            public int? CommentId { get; set; }

            public string? UserId { get; set; }

            public int? FeedID { get; set; }

            public string? FeedCommentString { get; set; }
    }
    
}
