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

        public static bool SetIfMatches(this Regex regex, string input, NotifyProperty<string> property)
        {
            if (regex.IsMatch(input))
            {
                property.Value = regex.Match(input).Value;
                return true;
            }

            return false;
        }

        public static bool SetIfMatches<T>(this Regex regex, string input, NotifyProperty<T> property, T value)
        {
            if (regex.IsMatch(input))
            {
                property.Value = value;
                return true;
            }

            return false;
        }
    }
}
