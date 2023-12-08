using BlogApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<User> UserInfo { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Likes> Likes { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Feed> Feeds { get; set; }

        public DbSet<FeedLike> FeedLikes { get; set; }
        public DbSet<FeedComments> FeedComments { get; set; }

    }
}
