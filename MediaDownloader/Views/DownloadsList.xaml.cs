using System.Windows;
using System.Windows.Controls;
using MediaDownloader.Download;

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
            if (sender is Button {Tag: Data.Download download})
            {
                DownloadHelper.OnDownloadAction(download);
            }
        }
    }
}
