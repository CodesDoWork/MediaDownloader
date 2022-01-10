using System.Diagnostics;
using MediaDownloader.Utils;
using static MediaDownloader.Data.DatabaseContext;

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

            InitDatabase();
            new UpdateChecker().Start();
        }
    }
}
