using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MediaDownloader.Utils;
using static MediaDownloader.Data.DatabaseContext;

namespace MediaDownloader.Windows
{
    public partial class MainWindow
    {
        private const double DefaultIconPadding = 9;

        public MainWindow()
        {
            InitializeComponent();
            DataContext =  this;
            Closed      += (_, _) => Application.Current.Shutdown();

            InitDatabase();
            new UpdateChecker().Start();
        }

        public NotifyProperty<Data.Download> Download { get; } = new(new Data.Download());

        public NotifyProperty<double> DownloadIconPadding { get; } = new(DefaultIconPadding);

        //public ObservableCollection<SavedDownload> SavedDownloads { get; } = GetLocalSavedDownloads();

        private void StartDownload(object sender, RoutedEventArgs e)
        {
            if (Download.Value.Start())
            {
                Download.Value = Download.Value.New();

                double animationDuration = 200;
                DispatcherTimer timer = new()
                {
                    Tag      = DateTime.Now,
                    Interval = TimeSpan.FromMilliseconds(1)
                };
                timer.Tick += (_, _) =>
                {
                    var elapsed = DateTime.Now - (DateTime) timer.Tag;
                    if (elapsed.TotalMilliseconds >= 2 * animationDuration)
                    {
                        DownloadIconPadding.Value = DefaultIconPadding;
                        timer.Stop();
                        return;
                    }

                    var amp = 1 - Math.Pow((elapsed.TotalMilliseconds - animationDuration) / animationDuration, 2);
                    DownloadIconPadding.Value = DefaultIconPadding * (1 - amp * 0.2);
                    DownloadImage.Opacity     = 1 - amp * 0.67;
                };
                timer.Start();
            }
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

        private void ModifyTitle(object sender, RoutedEventArgs e) { new TitleModifiersWindow().ShowDialog(); }
    }
}
