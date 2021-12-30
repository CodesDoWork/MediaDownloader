using System.Text;
using System.Text.RegularExpressions;
using MediaDownloader.Data;
using static MediaDownloader.Utils.RegexUtils;

namespace MediaDownloader.Download
{
    public class YtDlp : Downloader
    {
        private const string ExePath = "dl\\yt-dlp_x86.exe";

        private const string DownloadPrefix = "[download] ";

        private const string DownloadArgs = " -r 10G --throttled-rate 100K";

        private const string EmbedArgs = " --embed-thumbnail --embed-metadata --embed-chapters";

        private const string QualityArgs = " --audio-quality 320k";

        private const string AudioArgs = " -x --audio-format mp3 --metadata-from-title \"%(artist)s - %(title)s\"";

        private const string VideoArgs = " --merge-output-format mp4";

        private static readonly Regex AlreadyDownloadedRegex
            = new("(?<=^\\[download\\] ).+(?= has already been downloaded$)");

        private static readonly Regex ConverterRegex = new("^\\[ExtractAudio\\]|\\[Merger\\]|\\[VideoRemuxer\\] ");

        private static readonly Regex DestinationRegex = new("(?<=Destination: ).+");

        private static readonly Regex DownloadPlaylistRegex = new($"(?<={DownloadPrefix} Downloading Playlist: ).+");

        private static readonly Regex DataSplitRegex = new(" of | at | ETA ");

        private static readonly Regex ErrorRegex = new("^ERROR: .*$");

        public YtDlp(Data.Download download) : base(download)
        {
            download.Destination.Listeners.Add(SetTitleFromDestination);
        }

        protected override string GetExePath() { return ExePath; }

        protected override string MakeProcessArguments()
        {
            var formatArguments = Download.DownloadType.Value == DownloadType.Audio ? MakeAudioArgs() : MakeVideoArgs();
            var storageArguments
                = $" -o \"{Download.StoragePath}\\%(artist)s - %(title)s.%(ext)s\" -i \"{Download.Url}\"";

            return new StringBuilder(DownloadArgs).Append(EmbedArgs)
                                                  .Append(QualityArgs)
                                                  .Append(formatArguments)
                                                  .Append(storageArguments)
                                                  .ToString();
        }

        protected override void ReceiveOutputData(string data)
        {
            if (DestinationRegex.SetIfMatches(data, Download.Destination))
            {
                ConverterRegex.SetIfMatches(data, Download.CurrentStatus, DownloadStatus.Converting);
            }
            else if (data.StartsWith(DownloadPrefix) && !DownloadPlaylistRegex.SetIfMatches(data, Download.Title))
            {
                if (data.Contains("100% of "))
                {
                    Download.Percentage.Value    = 100;
                    Download.TimeRemaining.Value = "00:00";
                }
                else if (!AlreadyDownloadedRegex.SetIfMatches(data, Download.Destination))
                {
                    var dataParts = DataSplitRegex.Split(data.Substring(DownloadPrefix.Length).Replace("i", "").Trim());
                    if (dataParts.Length != 4)
                    {
                        return;
                    }

                    Download.CurrentStatus.Value           = DownloadStatus.Downloading;
                    Download.Percentage.Value              = ParseDecimal(dataParts[0]);
                    Download.TotalSize.FormattedSize.Value = dataParts[1];
                    Download.DownloadSpeed.Value           = dataParts[2];
                    Download.TimeRemaining.Value           = dataParts[3];

                    Download.DownloadedSize.FileSizeBytes.Value
                        = (long) (Download.Percentage.Value / 100 * Download.TotalSize.FileSizeBytes.Value);
                }
            }
        }

        protected override void ReceiveErrorData(string data)
        {
            ErrorRegex.SetIfMatches(data, Download.CurrentStatus, DownloadStatus.Error);
        }

        private void SetTitleFromDestination(string destination)
        {
            var lastDotIndex = destination.LastIndexOf('.');
            var startIndex   = Download.StoragePath.Length + 1;
            var length       = lastDotIndex                - Download.StoragePath.Length - 1;
            Download.Title.Value = destination.Substring(startIndex, length);
        }

        private static string ParseMetadata(string from, string to)
        {
            return $" --parse-metadata \"{from}:%({to})s\" --replace-in-metadata \"{to}\" \"^NA$\" \"\"";
        }

        private static string MakeAudioArgs()
        {
            return new StringBuilder(AudioArgs).Append(ParseMetadata("playlist_title", "album"))
                                               .Append(ParseMetadata("playlist_index", "track_number"))
                                               .ToString();
        }

        private string MakeVideoArgs()
        {
            return VideoArgs +
                   (Download.VideoQuality.Value == VideoQuality.Best
                        ? ""
                        : $" -S \"res:{Download.VideoQuality.Value.VideoHeight}\"");
        }
    }
}
