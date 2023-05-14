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
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        public ManagerPage()
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

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WonByApplicationsTwoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ContractYesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ContractNoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PartialAcceptanceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {

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

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
