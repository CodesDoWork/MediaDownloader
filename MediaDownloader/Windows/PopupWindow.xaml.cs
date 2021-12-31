using System;
using System.Windows.Input;

namespace MediaDownloader.Windows
{
    public partial class PopupWindow
    {
        public PopupWindow(string title, string infoText, params PopupButton[] buttons)
        {
            Title    = title;
            InfoText = infoText;
            Buttons  = buttons;

            InitializeComponent();
            DataContext = this;
        }

        public string InfoText { get; }

        public PopupButton[] Buttons { get; }

        public class PopupButton
        {
            public PopupButton(string text, Action<PopupButton> onClick, object extras = null)
            {
                Text    = text;
                OnClick = new ClickCommand(onClick);
                Extras  = extras;
            }

            public string Text { get; }

            public ClickCommand OnClick { get; }

            public object Extras { get; }

            public PopupWindow Window { get; set; }
        }

        public class ClickCommand : ICommand
        {
            public ClickCommand(Action<PopupButton> action) { Action = action; }

            public Action<PopupButton> Action { get; }

            public bool CanExecute(object parameter) { return true; }

            public void Execute(object parameter) { Action((PopupButton) parameter); }

            public event EventHandler CanExecuteChanged;
        }
    }
}
