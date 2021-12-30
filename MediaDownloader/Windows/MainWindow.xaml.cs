using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MediaDownloader.Data;
using MediaDownloader.Utils;

namespace MediaDownloader.Windows
{
    /// <summary>
    ///     Interaction logic for AssetOverviewWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public NotifyProperty<Download> Download { get; } = new() {Value = new Download()};

        //public ObservableCollection<SavedDownload> SavedDownloads { get; } = GetLocalSavedDownloads();

        private void StartDownload(object sender, RoutedEventArgs e)
        {
            Downloader.Download(Download.Value);

            var downloadType = Download.Value.DownloadType.Value;
            var videoQuality = Download.Value.VideoQuality.Value;
            Download.Value = new Download(Download.Value.Url)
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
