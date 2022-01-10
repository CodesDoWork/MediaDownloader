using System.Diagnostics;
using MediaDownloader.Utils;

namespace MediaDownloader
{
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

            new UpdateChecker().Start();
        }
    }
}
