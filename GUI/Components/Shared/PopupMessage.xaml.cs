using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GUI.Components.Shared
{
    public partial class PopupMessage : UserControl
    {
        public enum PopupType
        {
            Normal,
            Warning,
        }


        public event Action<PopupMessage> OnFaded;

        private int _displayTimeSeconds;
        private Timer _timer;
        private Storyboard _sb;
        private DoubleAnimation _animation;
        private bool _startTimerImmediately;
        private bool isClosed;

        public PopupMessage(PopupType type, int displayTimeSeconds, string message)
        {
            InitializeComponent();

            _sb = new Storyboard();
            _displayTimeSeconds = displayTimeSeconds;
            textblockMessage.Text = message;
            _startTimerImmediately = MainWindow.GetMainWindow().IsActive;

            SolidColorBrush background = null;
            SolidColorBrush border = null;
            SolidColorBrush foreground = null;
            switch (type)
            {
                case PopupType.Normal:
                    background = (SolidColorBrush)Application.Current.Resources["Popup.Background"];
                    border = (SolidColorBrush)Application.Current.Resources["Popup.Border"];
                    foreground = (SolidColorBrush)Application.Current.Resources["Popup.Foreground"];
                    break;
                case PopupType.Warning:
                    background = (SolidColorBrush)Application.Current.Resources["Popup.Background.Warning"];
                    border = (SolidColorBrush)Application.Current.Resources["Popup.Border.Warning"];
                    foreground = (SolidColorBrush)Application.Current.Resources["Popup.Foreground.Warning"];
                    break;
                default:
                    break;
            }

            Background = background;
            BorderBrush = border;
            textblockMessage.Foreground = foreground;

            Loaded += PopupMessage_Loaded;
            MouseEnter += PopupMessage_MouseEnter;
            MouseLeave += PopupMessage_MouseLeave;
            if (_startTimerImmediately == false)
            {
                MainWindow.GetMainWindow().Activated += MainWindow_Activated;
            }
        }

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            MainWindow.GetMainWindow().Activated -= MainWindow_Activated;
            PrepareFade();
        }

        private void PopupMessage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_startTimerImmediately)
            {
                PrepareFade();
            }
        }

        private void PrepareFade()
        {
            if(isClosed)
            {
                return;
            }

            if (_timer != null)
            {
                _timer.Dispose();
            }

            _timer = new Timer();
            _timer.Interval = _displayTimeSeconds * 1000;
            _timer.AutoReset = false;
            _timer.Elapsed += StartFade;
            _timer.Start();
        }

        private void ClosePopup()
        {
            isClosed = true;
            OnFaded?.Invoke(this);
            Visibility = Visibility.Collapsed;
            _timer.Elapsed -= StartFade;
            _timer.Dispose();
        }

        private void StartFade(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _animation = new DoubleAnimation();
                _animation.To = 0;
                _animation.From = 1;
                _animation.Duration = TimeSpan.FromMilliseconds(2000);
                _animation.EasingFunction = new QuadraticEase();

                _sb.Children.Add(_animation);

                Opacity = 1;
                Visibility = Visibility.Visible;

                Storyboard.SetTarget(_sb, this);
                Storyboard.SetTargetProperty(_sb, new PropertyPath(Control.OpacityProperty));

                _sb.Completed += delegate
                {
                    ClosePopup();
                };

                _sb.Begin();
            });
        }

        private void PopupMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MainWindow.GetMainWindow().IsActive)
            {
                _sb.Stop();
                _sb.Children.Remove(_animation);
            }
        }

        private void PopupMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            PrepareFade();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }
    }
}
