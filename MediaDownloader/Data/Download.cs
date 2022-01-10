using System.Collections.ObjectModel;
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

        public Download(string url = null, string storagePath = null)
        {
            Url         = url;
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

        public string Url { get; set; }

        public string StoragePath { get; }

        private Downloader Downloader { get; set; }

        public NotifyProperty<DownloadType> DownloadType { get; } = new() {Value = Data.DownloadType.Audio};

        public NotifyProperty<VideoQuality> VideoQuality { get; } = new() {Value = Data.VideoQuality.Best};

        public NotifyProperty<string> Destination { get; } = new();

        public NotifyProperty<string> Title { get; } = new();

        public NotifyProperty<decimal> Percentage { get; } = new() {Value = 100};

        public FileSize DownloadedSize { get; } = new();

        public FileSize TotalSize { get; } = new();

        public NotifyProperty<string> DownloadSpeed { get; } = new();

        public NotifyProperty<string> TimeRemaining { get; } = new();

        public NotifyProperty<DownloadStatus> CurrentStatus { get; } = new() {Value = DownloadStatus.Preparing};

        public bool Start()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return false;
            }

            if (!IsValidUrl(Url))
            {
                ShowInfo(Resources.EnterValidURL);
                return false;
            }

            if (CurrentDownloads.Contains(this))
            {
                CurrentDownloads.Remove(this);
            }


            Percentage.Value = 0;
            Downloader       = Downloader.GetDownloaderFor(this);
            Downloader.Start();
            CurrentDownloads.Add(this);

            return true;
        }

        public void Abort() { Downloader.Abort(); }

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
