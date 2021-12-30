using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace MediaDownloader.Utils
{
    public static class WindowsUtils
    {
        public static void KillProcessAndChildren(int pid)
        {
            var processSearcher
                = new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={pid}");
            var processCollection = processSearcher.Get();

            // We must kill child processes first!
            foreach (var o in processCollection)
            {
                var mo = (ManagementObject) o;
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }

            try
            {
                var proc = Process.GetProcessById(pid);
                if (!proc.HasExited)
                {
                    proc.Kill();
                    proc.Close();
                    proc.Dispose();
                }
            }
            catch (ArgumentException)
            {
            }
        }

        public static void ExploreFile(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return;
            }

            fullPath = Path.GetFullPath(fullPath);

            var pidlList = NativeMethods.ILCreateFromPathW(fullPath);
            if (pidlList != IntPtr.Zero)
            {
                try
                {
                    Marshal.ThrowExceptionForHR(NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                }
                finally
                {
                    NativeMethods.ILFree(pidlList);
                }
            }
        }

        private static class NativeMethods
        {
            [DllImport("shell32.dll", ExactSpelling = true)]
            public static extern void ILFree(IntPtr pidlList);

            [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern IntPtr ILCreateFromPathW(string pszPath);

            [DllImport("shell32.dll", ExactSpelling = true)]
            public static extern int SHOpenFolderAndSelectItems(
                IntPtr pidlList,
                uint   cild,
                IntPtr children,
                uint   dwFlags);
        }
    }
}
