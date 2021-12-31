using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MediaDownloader.Utils
{
    public static class WindowsUtils
    {
        // https://stackoverflow.com/a/30313207
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

        // https://stackoverflow.com/a/12262552
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

        // https://stackoverflow.com/a/16392220
        public static bool IsInstalled(string programName)
        {
            string displayName;

            var registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var key         = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (var subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(programName))
                    {
                        return true;
                    }
                }

                key.Close();
            }

            registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            key         = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (var subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(programName))
                    {
                        return true;
                    }
                }

                key.Close();
            }

            return false;
        }

        // https://stackoverflow.com/a/12262552
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
