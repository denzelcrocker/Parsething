using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для AutoCloseMessageBox.xaml
    /// </summary>
    public partial class AutoCloseMessageBox : Window
    {
        private DispatcherTimer _timer;

        public AutoCloseMessageBox(string message, string title, int timeout)
        {
            InitializeComponent();
            TitleTextBox.Text = title;
            this.MessageTextBlock.Text = message;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(timeout); // Время в миллисекундах
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            this.Close();
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            Close();
    }
}
