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
    /// Логика взаимодействия для EPlatformSpecialistPage.xaml
    /// </summary>
    public partial class EPlatformSpecialistPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsMethodsGroupings { get; set; }

        private List<Procurement>? Procurements = new List<Procurement>();

        public EPlatformSpecialistPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            int countOfMethods = 0;

            Procurements = GET.View.ProcurementsBy("Оформлен", false, GET.KindOf.Deadline);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;
            ForSendButton.Background = Brushes.LightGray;

            New.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Новый", GET.KindOf.ProcurementState)); // Новый

            Calculated.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Посчитан", GET.KindOf.ProcurementState)); // Посчитан

            RetreatCalculate.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState)); // Посчитан

            DrawUp.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформить", GET.KindOf.ProcurementState)); // Оформить

            Issued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", GET.KindOf.ProcurementState)); // Оформллены

            ForSend.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", false, GET.KindOf.Deadline));  // К отправке

            OverdueIssued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", true, GET.KindOf.Deadline));// Просрочены

            Bargaining.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", false, GET.KindOf.Deadline)); // Торги

            ProcurementsMethodsGroupings = GET.View.ProcurementsGroupByMethod();
            foreach (var item in ProcurementsMethodsGroupings)
            {
                countOfMethods += item.CountOfProcurements;
            }
            Quotes.Text = countOfMethods.ToString(); // Котировки (общее количество)
            foreach (var item in ProcurementsMethodsGroupings)
            {
                QuotesCombobox.Items.Add(item); // Котировки (по методам)
            }// Котировки

            OverdueSended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", true, GET.KindOf.Deadline)); // Просрочены

            Cancellation.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отмена", GET.KindOf.ProcurementState)); // Отменены

            Rejected.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отклонен", GET.KindOf.ProcurementState)); // Отклонены

            Lost.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Проигран", GET.KindOf.ProcurementState)); // Проиграны
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Новый", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.LightGray;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void CalculatedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Посчитан", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.LightGray;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void RetreatCalculateButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отбой", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.LightGray;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void DrawUpButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформить", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.LightGray;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void IssuedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформлен", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.LightGray;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void ForSendButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформлен", false, GET.KindOf.Deadline);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.LightGray;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void OverdueIssuedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформлен", true, GET.KindOf.Deadline);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.LightGray;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void BargainingButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", false, GET.KindOf.Deadline);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.LightGray;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", true, GET.KindOf.Deadline);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.LightGray;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void CancellationButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отмена", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.LightGray;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
        }

        private void RejectedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отклонен", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.LightGray;
            LostButton.Background = Brushes.Transparent;
        }

        private void LostButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Проигран", GET.KindOf.ProcurementState);
            if (Procurements != null)
                ProcurementsListView.ItemsSource = Procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.LightGray;
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
        private void NavigateToProcurementEPlatform_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null && procurement.Platform.Address != null)
            {
                string url = procurement.Platform.Address.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
    }
}
