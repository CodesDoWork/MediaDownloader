using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using MediaDownloader.Properties;
using static MediaDownloader.Utils.Globals;

namespace MediaDownloader.Utils
{
    public static class Update
    {
        private const string AssemblyUrl
            = "https://raw.githubusercontent.com/CodesDoWork/MediaDownloader/master/Properties/AssemblyInfo.cs";

        private static readonly Regex VersionRegex
            = new("(?<=\\[assembly: AssemblyVersion\\(\")\\d+\\.\\d+\\.\\d+\\.\\d+(?=\"\\)\\])");

        public static void CheckForUpdates()
        {
            try
            {
                using var client = new WebClient();
                if (VersionRegex.Match(client.DownloadString(AssemblyUrl)).Value !=
                    Assembly.GetExecutingAssembly().GetName().Version.ToString())
                {
                    ShowInfo(Resources.UpdateAvailable);
                }
            }
            catch (WebException)
            {
            }
        }
    }
}
