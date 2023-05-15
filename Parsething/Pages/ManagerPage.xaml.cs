using Microsoft.EntityFrameworkCore;
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

        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

        private List<Procurement>? Procurements = new List<Procurement>();
        private List<ProcurementsEmployee>? ProcurementsEmployees = new List<ProcurementsEmployee>();

        public ManagerPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try{ MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            WonPartOne.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Выигран 1ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Выигран 1ч

            WonPartTwo.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Выигран 2ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Выигран 2ч

            WonByApplications.Text = GET.Aggregate.ProcurementsEmployeesCountBy("", GET.KindOf.Applications, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // По заявкам

            ContractYes.Text = GET.Aggregate.ProcurementsEmployeesCountBy("", true, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // Контракт Подписан

            ContractNo.Text = GET.Aggregate.ProcurementsEmployeesCountBy("", false, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Контракт Не подписан

            Acceptance.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Приемка", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // Приемка

            // Частичная отправка

            // На исправлении

            NotPaidOnTime.Text = GET.Aggregate.ProcurementsEmployeesCountBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // В срок

            NotPaidDelay.Text = GET.Aggregate.ProcurementsEmployeesCountBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Просрочка

            NotPaid.Text = (GET.Aggregate.ProcurementsEmployeesCountBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id) + GET.Aggregate.ProcurementsEmployeesCountBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id)).ToString(); // Не оплачены

            Judgement.Text = GET.Aggregate.ProcurementsEmployeesCountBy(GET.KindOf.Judgement, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // Суд

            FAS.Text = GET.Aggregate.ProcurementsEmployeesCountBy(GET.KindOf.FAS, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // ФАС

            ExecutionState.Text = GET.Aggregate.ProcurementsEmployeesCountBy(null, GET.KindOf.ExecutionState, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // БГ исполнения

            WarrantyState.Text = GET.Aggregate.ProcurementsEmployeesCountBy(null, GET.KindOf.WarrantyState, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString(); // БГ гарантии
            
            ComponentCalculationsProblem = ComponentCalculationsBy("Проблема", ((Employee)Application.Current.MainWindow.DataContext).Id).Distinct(new Functions.MyClassComparer()).ToList(); // Проблема
            if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка", ((Employee)Application.Current.MainWindow.DataContext).Id).Distinct(new Functions.MyClassComparer()).ToList(); // В работе
            if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано", ((Employee)Application.Current.MainWindow.DataContext).Id).Distinct(new Functions.MyClassComparer()).ToList(); // Согласовано
            if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

            PreviousWeek.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Предыдущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Предыдущая неделя отгрузки

            ThisWeek.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Текущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Текущая неделя отгрузки

            NextWeek.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Следующая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Следующая неделя отгрузки

            AWeekLater.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Через одну", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id).ToString();// Отгрузка через неделю
        }
        public static List<ComponentCalculation>? ComponentCalculationsBy(string kind, int employeeId)
        {
            using ParsethingContext db = new();
            List<ComponentCalculation> componentCalculations = null;

            try
            {
                componentCalculations = db.ComponentCalculations
                    .Include(cc => cc.ComponentState)
                    .Include(cc => cc.Procurement)
                    .Include(cc => cc.Procurement.Law)
                    .Include(cc => cc.Procurement.ProcurementState)
                    .Include(cc => cc.Procurement.Region)
                    .Include(cc => cc.Procurement.Method)
                    .Include(cc => cc.Procurement.Platform)
                    .Include(cc => cc.Procurement.TimeZone)
                    .Include(cc => cc.Procurement.Organization)
                    .Include(cc => cc.Manufacturer)
                    .Include(cc => cc.ComponentType)
                    .Include(cc => cc.Seller)
                    .Where(cc => cc.Procurement.EmployeeId == employeeId)
                    .Where(cc => cc.ComponentState.Kind == kind)
                    .ToList();
            }
            catch { }

            return componentCalculations;
        }
        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 2ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WonByApplicationsTwoButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("", GET.KindOf.Applications, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ContractYesButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("", true, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ContractNoButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("", false, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void PartialAcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesNotPaid(((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(GET.KindOf.Judgement, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(GET.KindOf.FAS, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
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
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Текущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Предыдущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Следующая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Через одну", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void ExecutionStateButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(null, GET.KindOf.ExecutionState, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void WarrantyStateButton_Click(object sender, RoutedEventArgs e)
        {
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(null, GET.KindOf.WarrantyState, ((Employee)Application.Current.MainWindow.DataContext).Id);
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                Procurements.Add(procurementsEmployee.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }
    }
}
