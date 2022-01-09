using MediaDownloader.Utils;

namespace MediaDownloader.Data
{
    public class DownloadSettings
    {
        public DownloadSettings()
        {
            DownloadType.Listeners.Add(value => IsVideoDownload.Value = value == Data.DownloadType.Video);
        }

        public NotifyProperty<DownloadType> DownloadType { get; } = new(Data.DownloadType.Audio);

        public NotifyProperty<bool> IsVideoDownload { get; } = new();

        public NotifyProperty<VideoQuality> VideoQuality { get; } = new(Data.VideoQuality.Best);

        public void CopyFrom(DownloadSettings downloadSettings)
        {
            DownloadType.Value = downloadSettings.DownloadType.Value;
            VideoQuality.Value = downloadSettings.VideoQuality.Value;
        }
    }
}
