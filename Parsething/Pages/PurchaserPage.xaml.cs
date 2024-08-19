using DatabaseLibrary.Entities.ComponentCalculationProperties;
using DatabaseLibrary.Entities.ProcurementProperties;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using System.Windows.Controls.Primitives;
using Parsething.Classes;
using static DatabaseLibrary.Queries.GET;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для PurchaserPage.xaml
    /// </summary>
    public partial class PurchaserPage : Page
    {
        private Frame MainFrame { get; set; } = null!;
        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }
        private List<ProcurementsEmployee>? ProcurementsEmployees { get; set; }

        public PurchaserPage()
        {
            InitializeComponent();

            WonPartOne.Text = GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState).ToString();
            WonPartTwo.Text = GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState).ToString();
            ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsProblem != null)
            {
                Problem.Text = ComponentCalculationsProblem.Count.ToString();
            }
            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsInWork != null)
            {
                InWork.Text = ComponentCalculationsInWork.Count.ToString();
            }
            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsAgreed != null)
            {
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();
            }
            OnTheFix.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", KindOf.CorrectionDate)); // На исправлении
            Acceptance.Text = GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.ProcurementState).ToString();
            PreviousWeek.Text = GET.Aggregate.ProcurementsCountBy("Предыдущая", GET.KindOf.ShipmentPlane).ToString();
            ThisWeek.Text = GET.Aggregate.ProcurementsCountBy("Текущая", GET.KindOf.ShipmentPlane).ToString();
            NextWeek.Text = GET.Aggregate.ProcurementsCountBy("Следующая", GET.KindOf.ShipmentPlane).ToString();
            AWeekLater.Text = GET.Aggregate.ProcurementsCountBy("Через одну", GET.KindOf.ShipmentPlane).ToString();
            ApprovePurchaseYes.Text = GET.Aggregate.ProcurementsCountBy(true, GET.KindOf.Purchase).ToString();
            ApprovePurchaseNo.Text = GET.Aggregate.ProcurementsCountBy(false, GET.KindOf.Purchase).ToString();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background= Brushes.Transparent;
            NextWeekButton.Background = Brushes.LightGray;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.LightGray;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.LightGray;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent; 
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.LightGray;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.LightGray;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsProblem)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.LightGray;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsInWork)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.LightGray;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsAgreed)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.LightGray;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.LightGray;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.LightGray;
        }
        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.LightGray;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void ApprovePurchaseYesButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy(true, GET.KindOf.Purchase) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void ApprovePurchaseNoButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy(false, GET.KindOf.Purchase) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            OnTheFixButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement,false));
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

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, false, false));
        }

        private void SupplyMonitoringButton_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalUsingValues.Instance.Procurements.Count != 0)
                _ = MainFrame.Navigate(new SupplyMonitoringPage());
            else
                MessageBox.Show("Список тендеров пуст!");
        }

        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true, false));
        }

        private void EmployeeInfoButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Procurement? procurement = button?.DataContext as Procurement;
            if (procurement != null && button != null)
            {
                ProcurementsEmployees = GET.View.ProcurementsEmployeesByProcurement(procurement.Id);
                Popup popup = Functions.FindPopup.FindPopupByProcurementId(procurement.Id, button);

                if (popup != null && ProcurementsEmployees.Count != 0)
                {
                    popup.IsOpen = !popup.IsOpen;
                    TextBlock calculatorTextBlock = popup.FindName("CalculatorTextBlock") as TextBlock;
                    TextBlock managerTextBlock = popup.FindName("ManagerTextBlock") as TextBlock;

                    if (calculatorTextBlock != null)
                        calculatorTextBlock.Text = ProcurementsEmployees.LastOrDefault(pe => pe.Employee.PositionId == 2 || pe.Employee.PositionId == 3 || pe.Employee.PositionId == 4)?.Employee.FullName;
                    if (managerTextBlock != null)
                        managerTextBlock.Text = ProcurementsEmployees.LastOrDefault(pe => pe.Employee.PositionId == 5 || pe.Employee.PositionId == 6 || pe.Employee.PositionId == 8)?.Employee.FullName;
                }
            }
        }
        private void EmployeeInfoButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                Procurement? procurement = button.DataContext as Procurement;
                if (procurement != null)
                {
                    Popup popup = Functions.FindPopup.FindPopupByProcurementId(procurement.Id, button);
                    if (popup != null)
                    {
                        popup.IsOpen = false;
                    }
                }
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

        private void GoToApplicationsButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                List<Procurement> procurements = new List<Procurement>();
                if (procurement.ParentProcurementId != null)
                    GlobalUsingValues.Instance.AddProcurements(GET.View.ApplicationsBy(procurement.ParentProcurementId));
                else
                    GlobalUsingValues.Instance.AddProcurements(GET.View.ApplicationsBy(procurement.DisplayId));
                View.ItemsSource = null;
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;

            }
        }

        
    }
    
}
