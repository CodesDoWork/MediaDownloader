using MediaDownloader.Download;
using MediaDownloader.Utils;
using Syroot.Windows.IO;

namespace MediaDownloader.Data
{
    public class Download
    {
        public Download(string url = null, string storagePath = null)
        {
            Url         = url;
            StoragePath = storagePath ?? new KnownFolder(KnownFolderType.Downloads).Path;
        }

        public string Url { get; set; }

        public string StoragePath { get; }

        private Downloader Downloader { get; set; }

        public NotifyProperty<DownloadType> DownloadType { get; } = new() {Value = Data.DownloadType.Audio};

        public NotifyProperty<VideoQuality> VideoQuality { get; } = new() {Value = Data.VideoQuality.Best};

        public NotifyProperty<string> Destination { get; } = new();

        public NotifyProperty<string> Title { get; } = new();

        public NotifyProperty<decimal> Percentage { get; } = new();

        public FileSize DownloadedSize { get; } = new();

        public FileSize TotalSize { get; } = new();

        public NotifyProperty<string> DownloadSpeed { get; } = new();

        public NotifyProperty<string> TimeRemaining { get; } = new();

        public NotifyProperty<DownloadStatus> CurrentStatus { get; } = new() {Value = DownloadStatus.Preparing};

        public void Start()
        {
            Downloader = Downloader.GetDownloaderFor(this);
            Downloader.Start();
        }

        public void Abort() { Downloader.Abort(); }
    }
}
