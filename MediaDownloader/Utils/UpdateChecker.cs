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
    public class UpdateChecker
    {
        public const string ReleasesUrl = "https://github.com/CodesDoWork/MediaDownloader/releases";

        private const string LatestReleaseUrl = ReleasesUrl + "/latest";

        private const string ReleaseTagUrl = ReleasesUrl + "/tag/";

        public const string SetupFile = "MediaDownloader.msi";

        public const string PortableFile = "MediaDownloader-Portable.zip";

        private readonly UpdateClient Client;

        public UpdateChecker() { Client = new UpdateClient(); }

        public void Start()
        {
            BackgroundWorker worker = new();
            worker.DoWork += ExecuteCheck;
            worker.RunWorkerAsync();
        }

        private void ExecuteCheck(object s, DoWorkEventArgs args)
        {
            try
            {
                var latestRelease = Client.GetLatestRelease();
                if (latestRelease != GetCurrentRelease())
                {
                    Application.Current.Dispatcher.Invoke(
                        () => new PopupWindow(
                            Resources.UpdateAvailable,
                            string.Format(Resources.UpdateQuest, latestRelease),
                            new PopupWindow.Button(Resources.Download, Download, latestRelease),
                            new PopupWindow.Button(Resources.Later, Later)
                        ).ShowDialog()
                    );
                }
            }
            catch (WebException)
            {
            }
        }

        private void Download(PopupWindow.Button button)
        {
            var latestRelease = (string) button.Extras;
            var filename      = Client.DownloadInstallationAsset(latestRelease);

            DownloadUpdateWindow downloadWindow = new(filename, latestRelease);
            downloadWindow.Show();
            button.Window.Close();

            Client.DownloadProgressChanged += (_, e) => downloadWindow.DownloadProgress.Value = e.ProgressPercentage;
        }

        private void Later(PopupWindow.Button button) { button.Window.Close(); }

        private string GetCurrentRelease()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        private class UpdateClient : WebClient
        {
            private Uri _responseUri;

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                var response = base.GetWebResponse(request);
                _responseUri = response.ResponseUri;
                return response;
            }

            public string GetLatestRelease()
            {
                DownloadString(LatestReleaseUrl);
                return _responseUri.ToString().Substring(ReleaseTagUrl.Length);
            }

            public string DownloadInstallationAsset(string releaseVersion)
            {
                var isInstalled  = IsInstalled(Resources.AppName);
                var file         = isInstalled ? SetupFile : PortableFile;
                var downloadUrl  = $"{ReleasesUrl}/download/{releaseVersion}/{file}";
                var downloadPath = $"{new KnownFolder(KnownFolderType.LocalAppData).Path}/Temp/{file}";

                DownloadFileAsync(new Uri(downloadUrl), downloadPath);
                DownloadFileCompleted += (_, _) =>
                {
                    Process.Start(downloadPath);
                    if (!isInstalled)
                    {
                        ExploreFile(AppDomain.CurrentDomain.BaseDirectory);
                    }

                    Application.Current.Shutdown();
                };

                return file;
            }
        }
    }
}
