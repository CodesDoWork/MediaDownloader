using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using MediaDownloader.Properties;
using MediaDownloader.Windows;
using Syroot.Windows.IO;
using static MediaDownloader.Utils.WindowsUtils;

namespace MediaDownloader.Utils
{
    public static class Update
    {
        public const string ReleasesUrl = "https://github.com/CodesDoWork/MediaDownloader/releases";

        private const string LatestReleaseUrl = ReleasesUrl + "/latest";

        private const string ReleaseTagUrl = ReleasesUrl + "/tag/";

        public const string SetupFile = "MediaDownloader.msi";

        public const string PortableFile = "MediaDownloader-Portable.zip";

        public static void CheckForUpdates()
        {
            BackgroundWorker worker = new();
            worker.DoWork += ExecuteCheck;
            worker.RunWorkerAsync();
        }

        private static void ExecuteCheck(object s, DoWorkEventArgs args)
        {
            try
            {
                using RedirectClient client = new();

                client.DownloadString(LatestReleaseUrl);
                var latestRelease = client.ResponseUri.ToString().Substring(ReleaseTagUrl.Length);
                if (latestRelease != GetCurrentRelease())
                {
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            var infoText = string.Format(Resources.UpdateQuest, latestRelease);
                            var downloadButton = new PopupWindow.PopupButton(
                                Resources.Download,
                                Download,
                                latestRelease
                            );
                            var laterButton = new PopupWindow.PopupButton(Resources.Later, Later);

                            PopupWindow popup = new(Resources.UpdateAvailable, infoText, downloadButton, laterButton);
                            downloadButton.Window = popup;
                            laterButton.Window    = popup;
                            popup.ShowDialog();
                        }
                    );
                }
            }
            catch (WebException)
            {
            }
        }

        private static void Download(PopupWindow.PopupButton button)
        {
            var latestRelease = (string) button.Extras;

            var isInstalled  = IsInstalled(Resources.AppName);
            var file         = isInstalled ? SetupFile : PortableFile;
            var downloadUrl  = $"{ReleasesUrl}/download/{latestRelease}/{file}";
            var downloadPath = $"{new KnownFolder(KnownFolderType.LocalAppData).Path}/Temp/{file}";

            DownloadUpdateWindow downloadWindow = new(file, latestRelease);
            downloadWindow.Show();

            using WebClient client = new();
            client.DownloadFileAsync(new Uri(downloadUrl), downloadPath);
            client.DownloadProgressChanged += (_, e) => downloadWindow.DownloadProgress.Value = e.ProgressPercentage;
            client.DownloadFileCompleted += (_, _) =>
            {
                Process.Start(downloadPath);
                if (!isInstalled)
                {
                    ExploreFile(AppDomain.CurrentDomain.BaseDirectory);
                }

                Application.Current.Shutdown();
            };

            button.Window.Close();
        }

        private static void Later(PopupWindow.PopupButton button) { button.Window.Close(); }

        private static string GetCurrentRelease()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        private class RedirectClient : WebClient
        {
            public Uri ResponseUri { get; private set; }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                var response = base.GetWebResponse(request);
                ResponseUri = response.ResponseUri;
                return response;
            }
        }
    }
}
