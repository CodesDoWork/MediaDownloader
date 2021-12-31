using MediaDownloader.Utils;

namespace MediaDownloader.Windows
{
    public partial class DownloadUpdateWindow
    {
        public DownloadUpdateWindow(string filename, string releaseVersion)
        {
            Filename       = filename;
            ReleaseVersion = releaseVersion;

            InitializeComponent();
            DataContext = this;
        }

        public string Filename { get; }

        public string ReleaseVersion { get; }

        public NotifyProperty<int> DownloadProgress { get; } = new();
    }
}
