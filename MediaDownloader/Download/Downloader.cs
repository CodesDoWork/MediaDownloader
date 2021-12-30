using System;
using System.Diagnostics;
using System.IO;
using MediaDownloader.Data;
using static MediaDownloader.Utils.WindowsUtils;

namespace MediaDownloader.Download
{
    public abstract class Downloader
    {
        protected Data.Download Download;

        protected Downloader(Data.Download download) { Download = download; }

        private Process DownloadProcess { get; set; }

        public void Start()
        {
            Download.CurrentStatus.Value = DownloadStatus.Preparing;

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow         = true,
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                WindowStyle            = ProcessWindowStyle.Hidden,
                FileName               = GetExePath(),
                Arguments              = MakeProcessArguments()
            };

            DownloadProcess                     = Process.Start(startInfo);
            DownloadProcess.EnableRaisingEvents = true;

            DownloadProcess.BeginOutputReadLine();
            DownloadProcess.OutputDataReceived += OnData(ReceiveOutputData);
            DownloadProcess.BeginErrorReadLine();
            DownloadProcess.ErrorDataReceived += OnData(ReceiveErrorData);

            DownloadProcess.Exited += (_, _) =>
            {
                if (Download.CurrentStatus.Value != DownloadStatus.Error &&
                    Download.CurrentStatus.Value != DownloadStatus.Aborted)
                {
                    Download.CurrentStatus.Value = DownloadStatus.Finished;
                    if (File.Exists(Download.Destination.Value))
                    {
                        Download.TotalSize.FileSizeBytes.Value = new FileInfo(Download.Destination.Value).Length;
                    }
                }
            };
        }

        private DataReceivedEventHandler OnData(Action<string> processData)
        {
            return (_, e) =>
            {
                var data = e.Data;
                if (string.IsNullOrEmpty(data) || Download.CurrentStatus.Value == DownloadStatus.Aborted)
                {
                    return;
                }

                if (Debugger.IsAttached)
                {
                    Console.WriteLine(data);
                }

                processData(data);
            };
        }

        public void Abort()
        {
            Download.CurrentStatus.Value = DownloadStatus.Aborted;
            KillProcessAndChildren(DownloadProcess.Id);
        }

        protected abstract string GetExePath();

        protected abstract string MakeProcessArguments();

        protected abstract void ReceiveOutputData(string data);

        protected abstract void ReceiveErrorData(string data);

        public static Downloader GetDownloaderFor(Data.Download download) { return new YtDlp(download); }
    }
}
