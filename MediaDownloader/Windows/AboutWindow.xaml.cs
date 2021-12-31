using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace MediaDownloader.Windows
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            var language  = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var aboutText = Properties.Resources.ResourceManager.GetString($"About_{language}");
            if (string.IsNullOrEmpty(aboutText))
            {
                aboutText = Properties.Resources.About;
            }

            Regex linkTagRegex = new("<a href=\"[^<]+\">[^<]+<\\/a>");
            Regex hrefRegex    = new("(?<=href=\")[^<]+(?=\")");
            Regex nameRegex    = new("(?<=>)[^<]+(?=<)");

            var lastIndex = 0;
            foreach (Match match in linkTagRegex.Matches(aboutText))
            {
                AboutText.Inlines.Add(aboutText.Substring(lastIndex, match.Index - lastIndex));
                lastIndex = match.Index + match.Length;

                var       linkTag = match.Value;
                var       name    = nameRegex.Match(linkTag).Value;
                Uri       href    = new(hrefRegex.Match(linkTag).Value);
                Hyperlink link    = new(new Run(name)) {NavigateUri = href};
                link.RequestNavigate += RequestNavigate;

                AboutText.Inlines.Add(link);
            }

            AboutText.Inlines.Add(aboutText.Substring(lastIndex));
        }

        private static void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
