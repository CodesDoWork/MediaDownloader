using System.Diagnostics;
using MediaDownloader.Utils;
using static MediaDownloader.Utils.Update;

namespace MediaDownloader
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            if (!Debugger.IsAttached)
            {
                DispatcherUnhandledException += (_, e) =>
                {
                    Globals.ShowInfo(MediaDownloader.Properties.Resources.UnhandledExceptionMessage);
                    e.Handled = true;
                };
            }

            CheckForUpdates();
        }
    }
}
