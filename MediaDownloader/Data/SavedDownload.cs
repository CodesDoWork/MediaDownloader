#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using static MediaDownloader.Data.DatabaseContext;

namespace MediaDownloader.Data
{
    public class SavedDownload
    {
        public const string CreateTableStatement
            = "CREATE TABLE IF NOT EXISTS SavedDownloads (Url VARCHAR(200) PRIMARY KEY NOT NULL, Name VARCHAR(100) UNIQUE NOT NULL, StoragePath VARCHAR(256), LastDownload DATE);";

        [Key] public string Url { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? StoragePath { get; set; }

        public DateTime? LastDownload { get; set; }

        public static ObservableCollection<SavedDownload> GetLocalSavedDownloads()
        {
            DBConnection.SavedDownloads.Load();
            return DBConnection.SavedDownloads.Local;
        }
    }
}
