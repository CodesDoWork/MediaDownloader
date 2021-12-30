using System.Data.Entity;

namespace MediaDownloader.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=MediaDownloaderDb") { }

        public static DatabaseContext DBConnection { get; } = new();

        public virtual DbSet<SavedDownload> SavedDownloads { get; set; }
    }
}
