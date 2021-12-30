using System;
using System.Globalization;
using MediaDownloader.Annotations;
using MediaDownloader.Utils;
using static MediaDownloader.Utils.RegexUtils;

namespace MediaDownloader.Data
{
    public class FileSize
    {
        public FileSize()
        {
            FileSizeBytes.Listeners.Add(value => FormattedSize.Value = GetFormattedFileSize(value));
            FormattedSize.Listeners.Add(value => FileSizeBytes.Value = ToBytes(value));
        }

        public NotifyProperty<long> FileSizeBytes { get; } = new();

        public NotifyProperty<string> FormattedSize { get; } = new();

        public static string GetFormattedFileSize(long bytes)
        {
            decimal fileSize = bytes;
            var     unit     = FileSizeUnit.B;
            while (fileSize >= 1024)
            {
                fileSize /= 1024;
                unit     =  unit.Next();
            }

            return $"{Math.Round(fileSize, 2)}{unit}";
        }

        public static long ToBytes(string sizeString)
        {
            sizeString = sizeString.Replace("i", "");
            var size = ParseDecimal(sizeString);
            Enum.TryParse(
                sizeString.Substring(size.ToString(CultureInfo.InvariantCulture).Length),
                out FileSizeUnit unit
            );

            while (unit != FileSizeUnit.B)
            {
                size *= 1024;
                unit =  unit.Previous();
            }

            return (long) Math.Round(size);
        }

        private enum FileSizeUnit
        {
            B,
            [UsedImplicitly] KB,
            [UsedImplicitly] MB,
            [UsedImplicitly] GB,
            [UsedImplicitly] TB,
            [UsedImplicitly] PB
        }
    }
}
