using System;
using System.Windows.Input;

namespace MediaDownloader.Windows
{
    public partial class PopupWindow
    {
        public PopupWindow(string title, string infoText, params Button[] buttons)
        {
            Title    = title;
            InfoText = infoText;
            Buttons  = buttons;

            foreach (var button in buttons)
            {
                button.Window = this;
            }

            InitializeComponent();
            DataContext = this;
        }

        public string InfoText { get; }

        public Button[] Buttons { get; }

        public class Button
        {
            public Button(string text, Action<Button> onClick, object extras = null)
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
            public ClickCommand(Action<Button> action) { Action = action; }

            public Action<Button> Action { get; }

            public bool CanExecute(object parameter) { return true; }

            public void Execute(object parameter) { Action((Button) parameter); }

            public event EventHandler CanExecuteChanged;
        }
    }
}
