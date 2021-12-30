using System.Windows;

namespace MediaDownloader.Windows
{
    public abstract class BaseWindow : Window
    {
        protected BaseWindow()
        {
            Title                 = Properties.Resources.AppName + (string.IsNullOrEmpty(Title) ? "" : Title);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
