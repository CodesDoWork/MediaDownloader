using System.Collections.Generic;
using System.ComponentModel;

namespace MediaDownloader.Utils
{
    public class NotifyProperty<T> : INotifyPropertyChanged
    {
        public delegate void OnChange(T value);

        private T _value;

        public List<OnChange> Listeners { get; } = new();

        public T Value { get => _value; set => SetAndNotify(value); }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetAndNotify(T value)
        {
            if (_value != null && _value.Equals(value))
            {
                return;
            }

            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));

            foreach (var listener in Listeners)
            {
                listener(value);
            }
        }
    }
}
