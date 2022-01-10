using System.Collections.ObjectModel;
using System.ComponentModel;
using MediaDownloader.Download;
using MediaDownloader.Properties;
using MediaDownloader.Utils;
using Syroot.Windows.IO;
using static MediaDownloader.Utils.Globals;
using static MediaDownloader.Utils.WindowsUtils;

namespace MediaDownloader.Data
{
    public class Download
    {
        public static ObservableCollection<Download> CurrentDownloads = new();

        public static NotifyProperty<decimal> OverallPercentage = new();

        private Downloader _downloader;

        public Download() : this(null) { }

        public Download(string url = null, string storagePath = null)
        {
            Url.Value   = url;
            StoragePath = storagePath ?? new KnownFolder(KnownFolderType.Downloads).Path;
            Percentage.Listeners.Add(
                _ =>
                {
                    if (CurrentDownloads.Count == 0)
                    {
                        OverallPercentage.Value = 100;
                        return;
                    }

                    decimal totalPercents = 0;
                    foreach (var download in CurrentDownloads)
                    {
                        totalPercents += download.Percentage.Value;
                    }

                    OverallPercentage.Value = totalPercents / CurrentDownloads.Count;
                }
            );
        }

        public NotifyProperty<string> Url { get; } = new();

        public string StoragePath { get; }

        public DownloadSettings DownloadSettings { get; } = new();

        public NotifyProperty<string> Destination { get; } = new();

        public NotifyProperty<string> Title { get; } = new();

        public NotifyProperty<decimal> Percentage { get; } = new() {Value = 100};

        public FileSize DownloadedSize { get; } = new();

        public FileSize TotalSize { get; } = new();

        public NotifyProperty<string> DownloadSpeed { get; } = new();

        public NotifyProperty<string> TimeRemaining { get; } = new();

        public NotifyProperty<DownloadStatus> CurrentStatus { get; } = new(DownloadStatus.Preparing);

        public MetadataSettings MetadataSettings { get; } = new();

        public bool Start()
        {
            if (string.IsNullOrWhiteSpace(Url.Value))
            {
                return false;
            }

            if (!IsValidUrl(Url.Value))
            {
                ShowInfo(Resources.EnterValidURL);
                return false;
            }

            if (CurrentDownloads.Contains(this))
            {
                CurrentDownloads.Remove(this);
            }


            Percentage.Value = 0;
            _downloader      = Downloader.GetDownloaderFor(this);

            BackgroundWorker worker = new();
            worker.DoWork += (_, _) => _downloader.Start();
            worker.RunWorkerAsync();
            CurrentDownloads.Add(this);

            return true;
        }

        public void Abort() { _downloader.Abort(); }

        public Download New()
        {
            Download download = new();
            download.DownloadSettings.CopyFrom(DownloadSettings);
            download.MetadataSettings.CopyFrom(MetadataSettings);

            return download;
        }

        public void OnDownloadAction()
        {
            switch (CurrentStatus.Value)
            {
                case DownloadStatus.Downloading:
                    Abort();
                    break;
                case DownloadStatus.Aborted:
                    Start();
                    break;
                case DownloadStatus.Finished:
                    ExploreFile(Destination.Value);
                    break;
                case DownloadStatus.Error:
                    CurrentDownloads.Remove(this);
                    break;
            }
        }
    }
}
