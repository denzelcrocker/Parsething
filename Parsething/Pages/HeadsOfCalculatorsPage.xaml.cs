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

        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupingsNew { get; set; }
        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupingsDrawUp { get; set; }
        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesEPSpecialistGroupings { get; set; }

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

            Parsed.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Получен", GET.KindOf.ProcurementState)); // Спаршены

            Unsorted.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Неразобранный", GET.KindOf.ProcurementState)); // Неразобранные

            Retreat.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState)); // Отбой

            // Очередь расчета

            New.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Новый", GET.KindOf.ProcurementState)); // Новый

            Calculated.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Посчитан", GET.KindOf.ProcurementState)); // Посчитан

            ProcurementsEmployeesCalculatorsGroupingsNew = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов", "Новый", "", "");
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

            ProcurementsEmployeesCalculatorsGroupingsDrawUp = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов", "Оформить", "", "");
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

            ForSend.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", false, GET.KindOf.StartDate));  // К отправке

            OverdueIssued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", true, GET.KindOf.StartDate));// Просрочены

            ProcurementsEmployeesEPSpecialistGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист по работе с электронными площадками", "", "", "Отправлен", "", "");
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

            WonByOverAll.Text = (GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState) + GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState)).ToString(); // Выиграны всего
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
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void RetreatButton_Click(object sender, RoutedEventArgs e) // отбой
        {
            Procurements = GET.View.ProcurementsBy("Отбой", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void CalculationQueueButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
