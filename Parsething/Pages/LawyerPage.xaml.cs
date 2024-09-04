using Parsething.Classes;
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

        public LawyerPage()
        {
            InitializeComponent();

            NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

            NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

            NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

            Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

            FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            NavigationState.AddLastSelectedProcurement(View);
        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsNotPaid() ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy(false) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy(true) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy(GET.KindOf.Judgement) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy(GET.KindOf.FAS) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        }
        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            {
                NavigationState.LastSelectedProcurement = procurement;
                _ = MainFrame.Navigate(new CardOfProcurement(procurement));
            }
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            {
                string url = procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }

        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as FrameworkElement;

            if (image != null)
            {
                var parameter = image.Tag as string;
                ToolTipHelper.SetToolTip(image, parameter);
            }
        }

        private void ServiceId_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock? textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                Procurement? procurement = textBlock.DataContext as Procurement;
                if (procurement != null)
                {
                    Clipboard.SetText(procurement.DisplayId.ToString());
                    AutoClosingMessageBox.ShowAutoClosingMessageBox("Данные скопированы в буфер обмена", "Оповещение", 900);
                }
            }
        }
    }
}
