using System;

namespace MediaDownloader.Utils
{
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : Enum
        {
            var arr = (T[]) Enum.GetValues(src.GetType());
            var j   = Array.IndexOf(arr, src) + 1;
            return arr.Length == j ? arr[0] : arr[j];
        }

        public static T Previous<T>(this T src) where T : Enum
        {
            var arr = (T[]) Enum.GetValues(src.GetType());
            var j   = Array.IndexOf(arr, src) - 1;
            return j == -1 ? arr[arr.Length - 1] : arr[j];
        }
    }
}
