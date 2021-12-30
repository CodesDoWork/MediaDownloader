using System;
using System.Windows;
using MediaDownloader.Properties;

#nullable enable
namespace MediaDownloader.Utils
{
    public static class Globals
    {
        public static void ShowInfo(string msg) { MessageBox.Show(msg, Resources.Info, MessageBoxButton.OK); }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
