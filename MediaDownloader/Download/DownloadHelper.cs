using System.Collections.ObjectModel;
using MediaDownloader.Data;
using MediaDownloader.Properties;
using static MediaDownloader.Utils.Globals;
using static MediaDownloader.Utils.WindowsUtils;

namespace MediaDownloader.Download
{
    public static class DownloadHelper
    {
        public static ObservableCollection<Data.Download> CurrentDownloads = new();

        public static void Download(Data.Download download)
        {
            if (string.IsNullOrWhiteSpace(download.Url.Value))
            {
                return;
            }

            if (!IsValidUrl(download.Url.Value))
            {
                ShowInfo(Resources.EnterValidURL);
                return;
            }

            CurrentDownloads.Remove(download);
            download.Start();
            CurrentDownloads.Add(download);
        }

        public static void OnDownloadAction(Data.Download download)
        {
            switch (download.CurrentStatus.Value)
            {
                case DownloadStatus.Downloading:
                    download.Abort();
                    break;
                case DownloadStatus.Aborted:
                    Download(download);
                    break;
                case DownloadStatus.Finished:
                    ExploreFile(download.Destination.Value);
                    break;
                case DownloadStatus.Error:
                    CurrentDownloads.Remove(download);
                    break;
            }
        }
    }
}
