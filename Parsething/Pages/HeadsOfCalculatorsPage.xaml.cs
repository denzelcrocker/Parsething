using DatabaseLibrary.Entities.ProcurementProperties;
using Parsething.Windows;
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
using static DatabaseLibrary.Queries.GET;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для HeadsOfCalculatorsPage.xaml
    /// </summary>
    public partial class HeadsOfCalculatorsPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupingsNew { get; set; }
        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupingsDrawUp { get; set; }
        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesEPSpecialistGroupings { get; set; }
        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsMethodsGroupings { get; set; }


        private List<Procurement>? Procurements = new List<Procurement>();

        public HeadsOfCalculatorsPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try{ MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            int countOfCalculationsNew = 0;
            int countOfCalculationsDrawUp = 0;
            int countOfSended = 0;
            int countOfMethods = 0;

            Parsed.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Получен", GET.KindOf.ProcurementState)); // Спаршены

            Unsorted.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Неразобранный", GET.KindOf.ProcurementState)); // Неразобранные

            Retreat.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState)); // Отбой

            CalculationQueue.Text = Convert.ToString(GET.Aggregate.ProcurementsQueueCount());// Очередь расчета

            New.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Новый", GET.KindOf.ProcurementState)); // Новый

            Calculated.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Посчитан", GET.KindOf.ProcurementState)); // Посчитан

            CalculationsCombobox.Items.Clear();
            CalculationsCombobox.Text = "Расчет:";
            ProcurementsEmployeesCalculatorsGroupingsNew = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов", "Новый", "", "", "");
            foreach (var item in ProcurementsEmployeesCalculatorsGroupingsNew)
            {
                countOfCalculationsNew += item.CountOfProcurements;
            }
            CalculationsOverAll.Text = countOfCalculationsNew.ToString(); // Расчет (общее количество)
            foreach (var item in ProcurementsEmployeesCalculatorsGroupingsNew)
            {
                CalculationsCombobox.Items.Add(item); // Расчет (по сотрудникам)
            }

            DrawUp.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформить", GET.KindOf.ProcurementState)); // Оформить

            DrawUpCombobox.Items.Clear();
            DrawUpCombobox.Text = "Оформление:";
            ProcurementsEmployeesCalculatorsGroupingsDrawUp = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов", "Оформить", "", "", "");
            foreach (var item in ProcurementsEmployeesCalculatorsGroupingsDrawUp)
            {
                countOfCalculationsDrawUp += item.CountOfProcurements;
            }
            DrawUpOverAll.Text = countOfCalculationsDrawUp.ToString(); // Оформление (общее количество)
            foreach (var item in ProcurementsEmployeesCalculatorsGroupingsDrawUp)
            {
                DrawUpCombobox.Items.Add(item); // Оформление (по сотрудникам)
            }// Оформление

            Issued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", GET.KindOf.ProcurementState)); // Оформллены

            ForSend.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", false, GET.KindOf.Deadline));  // К отправке

            OverdueIssued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", true, GET.KindOf.Deadline));// Просрочены

            SendingCombobox.Items.Clear();
            SendingCombobox.Text = "Отправка:";
            ProcurementsEmployeesEPSpecialistGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист по работе с электронными площадками", "", "", "Отправлен", "", "", "");
            foreach (var item in ProcurementsEmployeesEPSpecialistGroupings)
            {
                countOfSended += item.CountOfProcurements;
            }
            SendingOverAll.Text = countOfSended.ToString(); // Подачицы (общее количество)
            foreach (var item in ProcurementsEmployeesEPSpecialistGroupings)
            {
                SendingCombobox.Items.Add(item); // Подачицы (по сотрудникам)
            } // Отправка

            WonPartOne.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState));// Выигран 1ч

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState));// Выигран 2ч

            WonByApplications.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)); // По заявкам

            WonByOverAll.Text = (GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState) + GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState) + GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)).ToString(); // Выиграны всег

            ApproveCalculatingYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Calculating).ToString(); // Проверка расчета проведена

            ApproveCalculatingNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Calculating).ToString(); // Проверка расчета не проведена

            ApprovePurchaseYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Purchase).ToString(); // Проверка закупки проведена

            ApprovePurchaseNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Purchase).ToString(); // Проверка закупки не проведена

            Sended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", GET.KindOf.ProcurementState)); // Отправлены

            Bargaining.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", false, GET.KindOf.Deadline)); // Торги

            QuotesCombobox.Items.Clear();
            QuotesCombobox.Text = "Сп-бы опр-я:";
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

            PreviousWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Предыдущая", GET.KindOf.ShipmentPlane));// Предыдущая неделя отгрузки

            ThisWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Текущая", GET.KindOf.ShipmentPlane));// Текущая неделя отгрузки

            NextWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Следующая", GET.KindOf.ShipmentPlane));// Следующая неделя отгрузки

            AWeekLater.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Через одну", GET.KindOf.ShipmentPlane));// Отгрузка через неделю
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Новый", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void CalculatedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Посчитан", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void DrawUpButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформить", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void IssuedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформлен", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ForSendButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформлен", false, GET.KindOf.StartDate);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void OverdueIssuedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Оформлен", true, GET.KindOf.StartDate);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void UnsortedButton_Click(object sender, RoutedEventArgs e) // неразобранные 
        {
            Procurements = GET.View.ProcurementsBy("Неразобранный", GET.KindOf.ProcurementState);
            if (Procurements.Count != 0)
            {
                SortWindow sortWindow = new SortWindow(Procurements, true);
                sortWindow.Show();
            }
        }

        private void RetreatButton_Click(object sender, RoutedEventArgs e) // отбой
        {
            Procurements = GET.View.ProcurementsBy("Отбой", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void CalculationQueueButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsQueue();
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WonByApplicationsButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("", GET.KindOf.Applications);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WonByOverAllButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ApproveCalculatingYesButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(true, GET.KindOf.Calculating);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ApproveCalculatingNoButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(false, GET.KindOf.Calculating);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ApprovePurchaseYesButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(true, GET.KindOf.Purchase);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ApprovePurchaseNoButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(false, GET.KindOf.Purchase);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }
        private void SendedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void BargainingButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", false, GET.KindOf.Deadline);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", true, GET.KindOf.Deadline);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void CancellationButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отмена", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void RejectedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отклонен", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void LostButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Проигран", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
            {
                MainFrame.Navigate(new SearchPage(selectedGrouping.Procurements));
            }
        }

        private void AddProcurementButton_Click(object sender, RoutedEventArgs e)
        {
            SortWindow sortWindow = new SortWindow(null, false);
            sortWindow.Show();
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }
    }
}
