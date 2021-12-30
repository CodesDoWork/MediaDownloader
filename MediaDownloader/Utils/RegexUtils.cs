using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaDownloader.Utils
{
    public static class RegexUtils
    {
        public static Regex NumRegex = new("[\\d|\\.]+");

        public static decimal ParseDecimal(string s)
        {
            return decimal.Parse(NumRegex.Match(s.Replace(',', '.')).Value, CultureInfo.InvariantCulture);
        }
    }
}
