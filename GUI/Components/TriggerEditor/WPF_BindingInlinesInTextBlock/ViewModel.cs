using System.ComponentModel;

namespace GUI.Components.TriggerEditor.WPF_BindingInlinesInTextBlock
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string text;

        public ViewModel(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                FirePropertyChanged("Text");
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
