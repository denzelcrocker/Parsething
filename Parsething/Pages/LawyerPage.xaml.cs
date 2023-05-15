using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private List<Procurement>? Procurements = new List<Procurement>();

        public LawyerPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try  {   MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            Procurements = GET.View.ProcurementsNotPaid();
            if (Procurements != null)
                View.ItemsSource = Procurements;

            NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

            NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

            NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

            Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

            FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС
        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsNotPaid();
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(false);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(true);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(GET.KindOf.Judgement);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(GET.KindOf.FAS);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }
        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement, null, false));
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                string url = procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }

        }
    }
}
