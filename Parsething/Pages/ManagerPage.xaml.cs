using DatabaseLibrary.Entities.EmployeeMuchToMany;
using DatabaseLibrary.Entities.ProcurementProperties;
using Microsoft.EntityFrameworkCore;
using Parsething.Classes;
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
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

        private DateTime StartDate = new DateTime();

        private List<ProcurementsEmployee>? ProcurementsEmployees = new List<ProcurementsEmployee>();

        public ManagerPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try{ MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            var globalUsingValues = GlobalUsingValues.Instance;
            StartDate = globalUsingValues.StartDate;

            WonPartOne.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Выигран 1ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Выигран 1ч

            WonPartTwo.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Выигран 2ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Выигран 2ч

            WonByApplications.Text = GET.Aggregate.ProcurementsEmployeesCountBy("", GET.KindOf.Applications, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // По заявкам

            ContractYes.Text = GET.Aggregate.ProcurementsEmployeesCountBy("", true, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Контракт Подписан

            ContractNo.Text = GET.Aggregate.ProcurementsEmployeesCountBy("", false, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Контракт Не подписан

            Acceptance.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Приемка", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Приемка

            // Частичная отправка

            OnTheFix.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Приемка", GET.KindOf.CorrectionDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// На исправлении

            NotPaidOnTime.Text = GET.Aggregate.ProcurementsEmployeesCountBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // В срок

            NotPaidDelay.Text = GET.Aggregate.ProcurementsEmployeesCountBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Просрочка

            NotPaid.Text = (GET.Aggregate.ProcurementsEmployeesCountBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint") + GET.Aggregate.ProcurementsEmployeesCountBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint")).ToString(); // Не оплачены

            Judgement.Text = GET.Aggregate.ProcurementsEmployeesCountBy(GET.KindOf.Judgement, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Суд

            FAS.Text = GET.Aggregate.ProcurementsEmployeesCountBy(GET.KindOf.FAS, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // ФАС

            ExecutionState.Text = GET.Aggregate.ProcurementsEmployeesCountBy(null, GET.KindOf.ExecutionState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // БГ исполнения

            WarrantyState.Text = GET.Aggregate.ProcurementsEmployeesCountBy(null, GET.KindOf.WarrantyState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // БГ гарантии
            
            ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема", ((Employee)Application.Current.MainWindow.DataContext).Id).Distinct(new Functions.MyClassComparer()).ToList(); // Проблема
            if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка", ((Employee)Application.Current.MainWindow.DataContext).Id).Distinct(new Functions.MyClassComparer()).ToList(); // В работе
            if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано", ((Employee)Application.Current.MainWindow.DataContext).Id).Distinct(new Functions.MyClassComparer()).ToList(); // Согласовано
            if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

            PreviousWeek.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Предыдущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Предыдущая неделя отгрузки

            ThisWeek.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Текущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Текущая неделя отгрузки

            NextWeek.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Следующая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Следующая неделя отгрузки

            AWeekLater.Text = GET.Aggregate.ProcurementsEmployeesCountBy("Через одну", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString();// Отгрузка через неделю

            Received.Text = GET.Aggregate.ProcurementsCountBy("Принят", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Принят

            ApproveCalculatingYes.Text = GET.Aggregate.ProcurementsEmployeesCountBy(true, KindOf.Calculating, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Проверка расчета проведена

            ApproveCalculatingNo.Text = GET.Aggregate.ProcurementsEmployeesCountBy(false, KindOf.Calculating, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Проверка расчета не проведена

            ApprovePurchaseYes.Text = GET.Aggregate.ProcurementsEmployeesCountBy(true, KindOf.Purchase, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Проверка закупки проведена

            ApprovePurchaseNo.Text = GET.Aggregate.ProcurementsEmployeesCountBy(false, KindOf.Purchase, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint").ToString(); // Проверка закупки не проведена
        }
       
        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 2ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void WonByApplicationsTwoButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("", GET.KindOf.Applications, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ContractYesButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("", true, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ContractNoButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("", false, GET.KindOf.ContractConclusion, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void PartialAcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке");
        }

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.CorrectionDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesNotPaid(((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(GET.KindOf.Judgement, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(GET.KindOf.FAS, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsProblem)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsInWork)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsAgreed)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Текущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Предыдущая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Следующая", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();  
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Через одну", GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ExecutionStateButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(null, GET.KindOf.ExecutionState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void WarrantyStateButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(null, GET.KindOf.WarrantyState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ReceivedButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy("Принят", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApproveCalculatingYesButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(true, GET.KindOf.Calculating, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApproveCalculatingNoButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(false, GET.KindOf.Calculating, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApprovePurchaseYesButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(true, GET.KindOf.Purchase, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApprovePurchaseNoButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            ProcurementsEmployees = GET.View.ProcurementsEmployeesBy(false, GET.KindOf.Purchase, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
            foreach (ProcurementsEmployee procurementsEmployee in ProcurementsEmployees)
            {
                GlobalUsingValues.Instance.AddProcurement(procurementsEmployee.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as FrameworkElement;

            if (image != null)
            {
                var parameter = image.Tag as string;
                ToolTipHelper.SetToolTipProcurementEmployee(image, parameter);
            }
        }
    }
}
