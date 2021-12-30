using MediaDownloader.Properties;

namespace MediaDownloader.Data
{
    public class VideoQuality : EnumClass
    {
        public static readonly VideoQuality   Best         = new(Resources.BestQuality, -1);
        public static readonly VideoQuality   _4k          = new(Resources._4kQuality, 2160);
        public static readonly VideoQuality   _1080p       = new(Resources._1080pQuality, 1080);
        public static readonly VideoQuality   _720p        = new(Resources._720pQuality, 720);
        public static readonly VideoQuality   _480p        = new(Resources._480pQuality, 480);
        public static readonly VideoQuality   _360p        = new(Resources._360pQuality, 360);
        public static readonly VideoQuality[] AllQualities = {Best, _4k, _1080p, _720p, _480p, _360p};

        private VideoQuality(string displayName, int videoHeight) : base(displayName) { VideoHeight = videoHeight; }

        public int VideoHeight { get; }
    }
}
