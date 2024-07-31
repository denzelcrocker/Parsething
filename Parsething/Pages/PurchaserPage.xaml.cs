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

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для PurchaserPage.xaml
    /// </summary>
    public partial class PurchaserPage : Page
    {
        private Frame MainFrame { get; set; } = null!;
        private List<Procurement>? ProcurementsWonPartOne { get; set; }
        private List<Procurement>? ProcurementsWonPartTwo { get; set; }
        private List<Procurement>? ProcurementsAcceptance { get; set; }
        private List<Procurement>? ProcurementsPreviousWeek { get; set; }
        private List<Procurement>? ProcurementsThisWeek { get; set; }
        private List<Procurement>? ProcurementsNextWeek { get; set; }
        private List<Procurement>? ProcurementsAWeekLater { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }
        private List<Procurement>? ProcurementsProblems { get; set; }
        private List<Procurement>? ProcurementsInWork { get; set; }
        private List<Procurement>? ProcurementsAgreed  { get; set; }
        private List<Procurement>? ProcurementsApprovePurchaseYes { get; set; }
        private List<Procurement>? ProcurementsApprovePurchaseNo { get; set; }
        private List<ProcurementsEmployee>? ProcurementsEmployees { get; set; }

        public PurchaserPage()
        {
            InitializeComponent();

            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }


            ProcurementsWonPartOne = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState);
            if (ProcurementsWonPartOne != null)
                WonPartOne.Text = ProcurementsWonPartOne.Count.ToString();

            ProcurementsWonPartTwo = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
            if (ProcurementsWonPartTwo != null)
                WonPartTwo.Text = ProcurementsWonPartTwo.Count.ToString();

            ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsProblem != null)
            {
                Problem.Text = ComponentCalculationsProblem.Count.ToString();
                ProcurementsProblems = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsProblem);
            }

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsInWork != null)
            {
                InWork.Text = ComponentCalculationsInWork.Count.ToString();
                ProcurementsInWork = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsInWork);
            }

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsAgreed != null)
            {
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();
                ProcurementsAgreed = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsAgreed);
            }

            ProcurementsAcceptance = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
            if (ProcurementsAcceptance != null)
                Acceptance.Text = ProcurementsAcceptance.Count.ToString();

            ProcurementsPreviousWeek = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane);
            if (ProcurementsPreviousWeek != null)
                PreviousWeek.Text = ProcurementsPreviousWeek.Count.ToString();

            ProcurementsThisWeek = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane);
            if (ProcurementsThisWeek != null)
            {
                GET.View.PopulateComponentStates(ProcurementsThisWeek);
                View.ItemsSource = ProcurementsThisWeek;
                ThisWeek.Text = ProcurementsThisWeek.Count.ToString();
            }

            ProcurementsNextWeek = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane);
            if (ProcurementsNextWeek != null)
                NextWeek.Text = ProcurementsNextWeek.Count.ToString();

            ProcurementsAWeekLater = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane);
            if (ProcurementsAWeekLater != null)
                AWeekLater.Text = ProcurementsAWeekLater.Count.ToString();

            ProcurementsApprovePurchaseYes = GET.View.ProcurementsBy(true, GET.KindOf.Purchase);
            if (ProcurementsApprovePurchaseYes != null)
                ApprovePurchaseYes.Text = ProcurementsApprovePurchaseYes.Count.ToString();

            ProcurementsApprovePurchaseNo = GET.View.ProcurementsBy(false, GET.KindOf.Purchase);
            if (ProcurementsApprovePurchaseNo != null)
                ApprovePurchaseNo.Text = ProcurementsApprovePurchaseNo.Count.ToString();

            ThisWeekButton.Background = Brushes.LightGray;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsNextWeek);
            View.ItemsSource = ProcurementsNextWeek;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background= Brushes.Transparent;
            NextWeekButton.Background = Brushes.LightGray;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsThisWeek);
            View.ItemsSource = ProcurementsThisWeek;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.LightGray;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsAcceptance);
            View.ItemsSource = ProcurementsAcceptance;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.LightGray;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsWonPartTwo);
            View.ItemsSource = ProcurementsWonPartTwo;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.LightGray;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsWonPartOne);
            View.ItemsSource = ProcurementsWonPartOne;
            WonPartOneButton.Background = Brushes.LightGray;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsProblems);
            View.ItemsSource = ProcurementsProblems;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.LightGray;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsInWork);
            View.ItemsSource = ProcurementsInWork;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.LightGray;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsAgreed);
            View.ItemsSource = ProcurementsAgreed;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.LightGray;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsAWeekLater);
            View.ItemsSource = ProcurementsAWeekLater;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.LightGray;
        }
        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsPreviousWeek);
            View.ItemsSource = ProcurementsPreviousWeek;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.LightGray;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }
        private void ApprovePurchaseYesButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsApprovePurchaseYes);
            View.ItemsSource = ProcurementsApprovePurchaseYes;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void ApprovePurchaseNoButton_Click(object sender, RoutedEventArgs e)
        {
            GET.View.PopulateComponentStates(ProcurementsApprovePurchaseNo);
            View.ItemsSource = ProcurementsApprovePurchaseNo;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            AcceptanceButton.Background = Brushes.Transparent;
            ProblemButton.Background = Brushes.Transparent;
            InWorkButton.Background = Brushes.Transparent;
            AgreedButton.Background = Brushes.Transparent;
            PreviousWeekButton.Background = Brushes.Transparent;
            ThisWeekButton.Background = Brushes.Transparent;
            NextWeekButton.Background = Brushes.Transparent;
            AWeekLaterButton.Background = Brushes.Transparent;
        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement,false));
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

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, false, false));
        }

        private void SupplyMonitoringButton_Click(object sender, RoutedEventArgs e)
        {
            List<Procurement> procurements;
            procurements = View.ItemsSource.Cast<Procurement>().ToList();
            if (procurements.Count != 0)
                _ = MainFrame.Navigate(new SupplyMonitoringPage(procurements));
            else
                MessageBox.Show("Список тендеров пуст!");
        }

        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
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
    }
    
}
