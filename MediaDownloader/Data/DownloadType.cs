using MediaDownloader.Properties;

namespace MediaDownloader.Data
{
    public class DownloadType : EnumClass
    {
        public static readonly DownloadType   Audio            = new(Resources.Audio);
        public static readonly DownloadType   Video            = new(Resources.Video);
        public static readonly DownloadType[] AllDownloadTypes = {Audio, Video};

        private DownloadType(string displayName) : base(displayName) { }
    }
}
