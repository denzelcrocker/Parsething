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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для LawyerPage.xaml
    /// </summary>
    public partial class LawyerPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        public LawyerPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
            }
            catch { }
        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
