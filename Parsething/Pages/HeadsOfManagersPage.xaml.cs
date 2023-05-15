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
    /// Логика взаимодействия для HeadsOfManagersPage.xaml
    /// </summary>
    public partial class HeadsOfManagersPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesManagersGroupings { get; set; }

        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

        private List<Procurement>? Procurements = new List<Procurement>();

        public HeadsOfManagersPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try{ MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            int countOfManagers = 0;

            WonPartOne.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState));// Выигран 1ч

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState));// Выигран 2ч

            WonByApplications.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)); // По заявкам

            ProcurementsEmployeesManagersGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист тендерного отдела", "Руководитель тендерного отдела", "Заместитель руководителя тендреного отдела", "Выигран 1ч", "Выигран 2ч", "Приемка");
            foreach (var item in ProcurementsEmployeesManagersGroupings)
            {
                countOfManagers += item.CountOfProcurements;
            }
            ManagersOverAll.Text = countOfManagers.ToString(); // Расчет (общее количество)
            foreach (var item in ProcurementsEmployeesManagersGroupings)
            {
                ManagersCombobox.Items.Add(item); // Расчет (по сотрудникам)
            }// Менеджеры выпадающий список

            ContractYes.Text = GET.Aggregate.ProcurementsCountBy("", true, GET.KindOf.ContractConclusion).ToString(); // Контракт Подписан

            ContractNo.Text = GET.Aggregate.ProcurementsCountBy("", false, GET.KindOf.ContractConclusion).ToString();// Контракт Не подписан

            Acceptance.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.ProcurementState)); // Приемка

            // Частичная отправка

            // На исправлении

            NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

            NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

            NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

            Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

            FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС

            ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList(); // Проблема
            if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList(); // В работе
            if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList(); // Согласовано
            if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

            PreviousWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Предыдущая", GET.KindOf.ShipmentPlane));// Предыдущая неделя отгрузки

            ThisWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Текущая", GET.KindOf.ShipmentPlane));// Текущая неделя отгрузки

            NextWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Следующая", GET.KindOf.ShipmentPlane));// Следующая неделя отгрузки

            AWeekLater.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Через одну", GET.KindOf.ShipmentPlane));// Отгрузка через неделю
        }

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsProblem)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsInWork)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsAgreed)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
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

        private void ContractYesButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("", true, GET.KindOf.ContractConclusion);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ContractNoButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("", false, GET.KindOf.ContractConclusion);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsNotPaid();
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(false);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(true);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(GET.KindOf.Judgement);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy(GET.KindOf.FAS);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PartialAcceptanceButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
