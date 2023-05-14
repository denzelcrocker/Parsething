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
    /// Логика взаимодействия для HeadsOfCalculatorsPage.xaml
    /// </summary>
    public partial class HeadsOfCalculatorsPage : Page
    {
        private Frame MainFrame { get; set; } = null!;
        public HeadsOfCalculatorsPage()
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

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CalculatedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CalculationsOverAllButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RetreatCalculateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DrawUpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IssuedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForSendButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OverdueIssuedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UnsortedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RetreatButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CalculationQueueButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WonByApplicationsTwoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WonByOverAllButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
