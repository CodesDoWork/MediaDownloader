using System.Data.Entity;

namespace MediaDownloader.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=MediaDownloaderDb") { }

        public static DatabaseContext DBConnection { get; } = new();

        public virtual DbSet<SavedDownload> SavedDownloads { get; set; }

        public virtual DbSet<TitleModifier> TitleModifiers { get; set; }

        public static void InitDatabase()
        {
            DBConnection.Database.ExecuteSqlCommand(SavedDownload.CreateTableStatement);
            DBConnection.Database.ExecuteSqlCommand(TitleModifier.CreateTableStatement);
        }
    }
}
