using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MediaDownloader.Download;
using MediaDownloader.Utils;

namespace MediaDownloader.Windows
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public NotifyProperty<Data.Download> Download { get; } = new() {Value = new Data.Download()};

        //public ObservableCollection<SavedDownload> SavedDownloads { get; } = GetLocalSavedDownloads();

        private void StartDownload(object sender, RoutedEventArgs e)
        {
            DownloadHelper.Download(Download.Value);

            var downloadType = Download.Value.DownloadType.Value;
            var videoQuality = Download.Value.VideoQuality.Value;
            Download.Value = new Data.Download(Download.Value.Url)
            {
                DownloadType =
                {
                    Value = downloadType
                },
                VideoQuality =
                {
                    Value = videoQuality
                }
            };
        }

        private void ShowDownloadsMenu(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var menu = button.ContextMenu;
                menu.PlacementTarget  = button;
                menu.Placement        = PlacementMode.Bottom;
                menu.IsOpen           = true;
                menu.HorizontalOffset = (menu.ActualWidth - button.Width) / -2;
            }

            e.Handled = true;
        }

        private void ShowAboutInfo(object sender, RoutedEventArgs e) { new AboutWindow().ShowDialog(); }
    }
}
