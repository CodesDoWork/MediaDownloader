using System.ComponentModel;
using MediaDownloader.Download;
using MediaDownloader.Utils;
using Syroot.Windows.IO;

namespace MediaDownloader.Data
{
    public class Download
    {
        private Downloader _downloader;
        public Download() : this(null) { }

        public Download(string url = null, string storagePath = null)
        {
            Url.Value   = url;
            StoragePath = storagePath ?? new KnownFolder(KnownFolderType.Downloads).Path;
        }

        public NotifyProperty<string> Url { get; } = new();

        public string StoragePath { get; }

        public DownloadSettings DownloadSettings { get; } = new();

        public NotifyProperty<string> Destination { get; } = new();

        public NotifyProperty<string> Title { get; } = new();

        public NotifyProperty<decimal> Percentage { get; } = new();

        public FileSize DownloadedSize { get; } = new();

        public FileSize TotalSize { get; } = new();

        public NotifyProperty<string> DownloadSpeed { get; } = new();

        public NotifyProperty<string> TimeRemaining { get; } = new();

        public NotifyProperty<DownloadStatus> CurrentStatus { get; } = new(DownloadStatus.Preparing);

        public MetadataSettings MetadataSettings { get; } = new();

        public void Start()
        {
            _downloader = Downloader.GetDownloaderFor(this);

            BackgroundWorker worker = new();
            worker.DoWork += (_, _) => _downloader.Start();
            worker.RunWorkerAsync();
        }

        public void Abort() { _downloader.Abort(); }

        public Download New()
        {
            Download download = new();
            download.DownloadSettings.CopyFrom(DownloadSettings);
            download.MetadataSettings.CopyFrom(MetadataSettings);

            return download;
        }
    }
}
