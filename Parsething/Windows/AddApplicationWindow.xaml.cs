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

namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddApplicationWindow.xaml
    /// </summary>
    public partial class AddApplicationWindow : Window
    {
        private Frame MainFrame { get; set; } = null!;

        public AddApplicationWindow(Procurement procurement)
        {
            InitializeComponent();

            if (procurement.IsUnitPrice == true)
            {

            }
            else
            { 
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            Close();
    }
}
