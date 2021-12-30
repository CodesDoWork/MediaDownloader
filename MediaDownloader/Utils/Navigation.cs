using System.Windows;

namespace MediaDownloader.Utils
{
    public static class Navigation
    {
        private static bool IsBackPressed { get; set; }

        public static void OpenWindow(this Window root, Window next)
        {
            root.Hide();
            next.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            next.Show();

            next.Closing += (s, e) =>
            {
                if (IsBackPressed)
                {
                    root.Show();
                }
                else
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        try
                        {
                            window.Close();
                        }
                        catch
                        {
                        }
                    }
                }

                IsBackPressed = false;
            };
        }

        public static void Back(this Window window)
        {
            IsBackPressed = true;
            window.Close();
        }
    }
}
