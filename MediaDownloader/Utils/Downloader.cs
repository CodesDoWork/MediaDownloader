using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using MediaDownloader.Data;
using MediaDownloader.Properties;
using static MediaDownloader.Utils.Globals;
using static MediaDownloader.Utils.WindowsUtils;

namespace MediaDownloader.Utils
{
    public static class Downloader
    {
        public static ObservableCollection<Download> CurrentDownloads = new();

        private static readonly Dictionary<Download, Process> DownloadProcesses = new();

        public static void Download(Download download)
        {
            if (string.IsNullOrWhiteSpace(download.Url))
            {
                return;
            }

            if (!IsValidUrl(download.Url))
            {
                ShowInfo(Resources.EnterValidURL);
                return;
            }

            CurrentDownloads.Remove(download);
            download.CurrentStatus.Value = DownloadStatus.Preparing;

            var downloadProcess = download.Start();

            DownloadProcesses[download] = downloadProcess;
            CurrentDownloads.Add(download);

            downloadProcess.Exited += (_, _) =>
            {
                DownloadProcesses.Remove(download);

                if (download.CurrentStatus.Value != DownloadStatus.Error &&
                    download.CurrentStatus.Value != DownloadStatus.Aborted)
                {
                    download.CurrentStatus.Value = DownloadStatus.Finished;
                    if (File.Exists(download.Destination.Value))
                    {
                        download.TotalSize.FileSizeBytes.Value = new FileInfo(download.Destination.Value).Length;
                    }
                }
            };
        }

        public static void OnDownloadAction(Download download)
        {
            switch (download.CurrentStatus.Value)
            {
                case DownloadStatus.Downloading:
                    download.CurrentStatus.Value = DownloadStatus.Aborted;
                    KillProcessAndChildren(DownloadProcesses[download].Id);
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
