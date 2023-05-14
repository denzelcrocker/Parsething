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
    /// Логика взаимодействия для EPlatformSpecialistPage.xaml
    /// </summary>
    public partial class EPlatformSpecialistPage : Page
    {
        private Frame MainFrame { get; set; } = null!;
        public EPlatformSpecialistPage()
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

        private void BargainingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QuotesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancellationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RejectedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LostButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
