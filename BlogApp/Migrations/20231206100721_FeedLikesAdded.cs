using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Migrations
{
    public partial class FeedLikesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Feeds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Feeds",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FeedLikes",
                columns: table => new
                {
                    FeedLikeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedID = table.Column<int>(type: "int", nullable: true),
                    FeedTotalLikeCount = table.Column<int>(type: "int", nullable: true),
                    IsLiked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedLikes", x => x.FeedLikeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedLikes");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Feeds");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Feeds");
        }
    }
}
