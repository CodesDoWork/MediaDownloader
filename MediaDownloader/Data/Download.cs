using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MediaDownloader.Utils;
using Syroot.Windows.IO;
using static MediaDownloader.Utils.RegexUtils;

namespace MediaDownloader.Data
{
    public class Download
    {
        private const string DownloadPrefix = "[download] ";

        private const string ErrorPrefix = "ERROR: ";

        private const string BasicYtDlpArguments
            = " -r 10G --throttled-rate 100K --audio-quality 320k --embed-thumbnail --embed-metadata --embed-chapters";

        private readonly Regex _alreadyDownloadedRegex
            = new("(?<=^\\[download\\] ).+(?= has already been downloaded$)");

        private readonly Regex _converterRegex = new("^\\[ExtractAudio\\]|\\[Merger\\]|\\[VideoRemuxer\\] ");

        private readonly Regex _destinationRegex = new("(?<=Destination: ).+");

        private readonly Regex _downloadPlaylistRegex = new($"(?<={DownloadPrefix} Downloading Playlist: ).+");

        public Download(string url = null, string storagePath = null)
        {
            Url         = url;
            StoragePath = storagePath ?? new KnownFolder(KnownFolderType.Downloads).Path;
            Destination.Listeners.Add(SetTitleFromDestination);
        }

        public string Url { get; set; }

        public string StoragePath { get; }

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

        public Process Start()
        {
            var    storageArguments = $"-o \"{StoragePath}\\%(artist)s - %(title)s.%(ext)s\" -i \"{Url}\"";
            string formatArguments;
            if (DownloadType.Value == Data.DownloadType.Audio)
            {
                formatArguments =  "-x --audio-format mp3 --metadata-from-title \"%(artist)s - %(title)s\"";
                formatArguments += " --parse-metadata \"playlist_title:%(album)s\"";
                formatArguments += " --parse-metadata \"playlist_index:%(track_number)s\"";
            }
            else
            {
                formatArguments = "--merge-output-format mp4";
                if (VideoQuality.Value != Data.VideoQuality.Best)
                {
                    var height = VideoQuality.Value.VideoHeight;
                    formatArguments += $" -S \"res:{height}\"";
                }
            }

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow         = true,
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                WindowStyle            = ProcessWindowStyle.Hidden,
                FileName               = "dl\\yt-dlp_x86.exe",
                Arguments              = $"{BasicYtDlpArguments} {formatArguments} {storageArguments}"
            };

            var downloadProcess = Process.Start(startInfo);
            downloadProcess.EnableRaisingEvents = true;

            downloadProcess.BeginOutputReadLine();
            downloadProcess.OutputDataReceived += (_, e) => AddDetails(e.Data);
            downloadProcess.BeginErrorReadLine();
            downloadProcess.ErrorDataReceived += (_, e) => OnError(e.Data);

            return downloadProcess;
        }

        private void AddDetails(string details)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine(details);
            }

            if (string.IsNullOrEmpty(details) || CurrentStatus.Value == DownloadStatus.Aborted)
            {
                return;
            }

            if (_destinationRegex.IsMatch(details))
            {
                Destination.Value = _destinationRegex.Match(details).Value;
                if (_converterRegex.IsMatch(details))
                {
                    CurrentStatus.Value = DownloadStatus.Converting;
                }
            }
            else if (details.StartsWith(DownloadPrefix))
            {
                if (_downloadPlaylistRegex.IsMatch(details))
                {
                    Title.Value = _downloadPlaylistRegex.Match(details).Value;
                }
                else
                {
                    if (details.Contains("100% of "))
                    {
                        Percentage.Value    = 100;
                        TimeRemaining.Value = "00:00";
                    }
                    else if (_alreadyDownloadedRegex.IsMatch(details))
                    {
                        Destination.Value = _alreadyDownloadedRegex.Match(details).Value;
                    }
                    else
                    {
                        var data = new Regex(" of | at | ETA ").Split(
                            details.Substring(DownloadPrefix.Length).Replace("i", "").Trim()
                        );

                        if (data.Length != 4)
                        {
                            return;
                        }

                        CurrentStatus.Value           = DownloadStatus.Downloading;
                        Percentage.Value              = ParseDecimal(data[0]);
                        TotalSize.FormattedSize.Value = data[1];
                        DownloadSpeed.Value           = data[2];
                        TimeRemaining.Value           = data[3];

                        DownloadedSize.FileSizeBytes.Value
                            = (long) (Percentage.Value / 100 * TotalSize.FileSizeBytes.Value);
                    }
                }
            }
        }

        private void OnError(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                return;
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine(error);
            }

            if (error.StartsWith(ErrorPrefix))
            {
                CurrentStatus.Value = DownloadStatus.Error;
            }
        }

        private void SetTitleFromDestination(string destination)
        {
            var lastDotIndex = destination.LastIndexOf('.');
            var startIndex   = StoragePath.Length + 1;
            var length       = lastDotIndex       - StoragePath.Length - 1;
            Title.Value = destination.Substring(startIndex, length);
        }
    }
}
