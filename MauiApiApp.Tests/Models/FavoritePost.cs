using SQLite;

namespace MauiApiApp.Tests.Models
{
    [Table("favorites")]
    public class FavoritePost
    {
        [PrimaryKey]
        public int PostId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime SavedAt { get; set; }
    }
}
