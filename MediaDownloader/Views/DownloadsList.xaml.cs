using System.Windows;
using System.Windows.Controls;
using MediaDownloader.Data;
using MediaDownloader.Utils;

namespace MediaDownloader.Views
{
    public partial class DownloadsList
    {
        public DownloadsList()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnDownloadAction(object sender, RoutedEventArgs e)
        {
            if (sender is Button {Tag: Download download})
            {
                Downloader.OnDownloadAction(download);
            }
        }
    }
}
