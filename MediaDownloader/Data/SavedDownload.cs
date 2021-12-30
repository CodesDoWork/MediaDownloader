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
