using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaDownloader.Views
{
    public class HintTextBox : TextBox
    {
        public HintTextBox()
        {
            Fore = Foreground;
            GotFocus += (_, _) =>
            {
                if (Text == Hint)
                {
                    Clear();
                    SetValue(TextProperty, "");
                    Foreground = Fore;
                }
            };

            LostFocus += SetHint;
            Loaded    += SetHint;
        }

        private Brush Fore { get; }

        public string Hint { get; set; }

        private void SetHint(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = Hint;
                SetValue(TextProperty, Hint);
                Foreground = Brushes.LightGray;
            }
        }
    }
}
