using Microsoft.EntityFrameworkCore;
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
using Parsething.Classes;
using Parsething.Windows;
using DatabaseLibrary.Entities.ProcurementProperties;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Law>? Laws { get; set; }
        private List<ProcurementState>? ProcurementStates { get; set; }
        private List<Employee>? Employees { get; set; }

        private List<Procurement>? FoundProcurements { get; set; }

        private List<Procurement>? Procurements { get; set; }

        public SearchPage(List<Procurement>? procurements)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementId) ||
                !string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementNumber) ||
                !string.IsNullOrEmpty(SearchCriteria.Instance.Law) ||
                !string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementState) ||
                !string.IsNullOrEmpty(SearchCriteria.Instance.INN) ||
                !string.IsNullOrEmpty(SearchCriteria.Instance.Employee) ||
                !string.IsNullOrEmpty(SearchCriteria.Instance.OrganizationName))
            {
                procurements = GET.View.ProcurementsBy(
                    SearchCriteria.Instance.ProcurementId,
                    SearchCriteria.Instance.ProcurementNumber,
                    SearchCriteria.Instance.Law,
                    SearchCriteria.Instance.ProcurementState,
                    SearchCriteria.Instance.INN,
                    SearchCriteria.Instance.Employee,
                    SearchCriteria.Instance.OrganizationName);
                GET.View.PopulateComponentStates(procurements);
                SearchLV.ItemsSource = procurements;
            }
            else if (procurements != null)
            {
                SearchLV.ItemsSource = procurements;
            }
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;
            Procurements = procurements;
            RestoreSearchCriteria();
        }
        private void RestoreSearchCriteria()
        {
            SearchId.Text = SearchCriteria.Instance.ProcurementId;
            SearchNumber.Text = SearchCriteria.Instance.ProcurementNumber;
            Law.SelectedItem = Laws?.FirstOrDefault(l => l.Number == SearchCriteria.Instance.Law);
            ProcurementState.SelectedItem = ProcurementStates?.FirstOrDefault(ps => ps.Kind == SearchCriteria.Instance.ProcurementState);
            SearchINN.Text = SearchCriteria.Instance.INN;
            Employee.SelectedItem = Employees?.FirstOrDefault(e => e.FullName  == SearchCriteria.Instance.Employee);
            OrganizationName.Text = SearchCriteria.Instance.OrganizationName;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement, Procurements, true));
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                string url = procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка предыдущих критериев поиска
            SearchCriteria.Instance.ClearData();

            SearchLV.ItemsSource = null;
            FoundProcurements?.Clear();

            string id = SearchId.Text;
            string number = SearchNumber.Text;
            string law = Law.Text;
            string procurementState = ProcurementState.Text;
            string inn = SearchINN.Text;
            string employee = Employee.Text;
            string organizationName = OrganizationName.Text;

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(number) && string.IsNullOrEmpty(law) &&
                string.IsNullOrEmpty(procurementState) && string.IsNullOrEmpty(inn) &&
                string.IsNullOrEmpty(employee) && string.IsNullOrEmpty(organizationName))
            {
                // Ничего не делать, если все поля пустые
            }
            else
            {
                FoundProcurements = GET.View.ProcurementsBy(id, number, law, procurementState, inn, employee, organizationName);
                GET.View.PopulateComponentStates(FoundProcurements);
                SearchLV.ItemsSource = FoundProcurements;
                Procurements = FoundProcurements;
            }

            // Сохранение введенных критериев поиска
            SearchCriteria.Instance.ProcurementId = id;
            SearchCriteria.Instance.ProcurementNumber = number;
            SearchCriteria.Instance.Law = law;
            SearchCriteria.Instance.ProcurementState = procurementState;
            SearchCriteria.Instance.INN = inn;
            SearchCriteria.Instance.Employee = employee;
            SearchCriteria.Instance.OrganizationName = organizationName;
        }

        private void Search_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, Procurements, true, true));
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, Procurements, false, true));
        }

        private void OverallInfo_Click(object sender, RoutedEventArgs e)
        {
            decimal? overallAmount = 0;
            decimal? overallAmountCalculate = 0;
            decimal? profitCalculate = 0;
            decimal? profitReal = 0;
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;

            OverallInfoPopUp.IsOpen = !OverallInfoPopUp.IsOpen;
            if (Procurements != null)
            {
                OverallCount.Text = Procurements.Count.ToString();
                foreach (Procurement procurement in Procurements)
                {
                    if (procurement.ContractAmount != null && procurement.ReserveContractAmount == null && procurement.PurchaseAmount != null && procurement.CalculatingAmount != null)
                    {
                        overallAmount += procurement.ContractAmount;
                        profitReal += procurement.ContractAmount - procurement.PurchaseAmount;
                        profitCalculate += procurement.ContractAmount - procurement.CalculatingAmount;
                        calculatingAmount += procurement.CalculatingAmount;
                        purchaseAmount += procurement.PurchaseAmount;
                        overallAmountCalculate += procurement.ContractAmount;
                    }
                    else if (procurement.ReserveContractAmount != null && procurement.PurchaseAmount != null && procurement.CalculatingAmount != null)
                    {
                        overallAmount += procurement.ReserveContractAmount;
                        profitReal += procurement.ReserveContractAmount - procurement.PurchaseAmount;
                        profitCalculate += procurement.ContractAmount - procurement.CalculatingAmount;
                        calculatingAmount += procurement.CalculatingAmount;
                        purchaseAmount += procurement.PurchaseAmount;
                        overallAmountCalculate += procurement.ContractAmount;
                    }
                }
                OverallAmount.Text = ((decimal)overallAmount).ToString("N2") + " р.";
                if (calculatingAmount != 0 && purchaseAmount != 0)
                {
                    AvgCalculationProfit.Text = $"{profitCalculate} р. ({(double?)((overallAmountCalculate - calculatingAmount) / calculatingAmount * 100):N1} %)";
                    AvgPurchaseProfit.Text = $"{profitReal} р. ({(double?)((overallAmount - purchaseAmount) / purchaseAmount * 100):N1} %)";
                }
            }
        }

        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                AssemblyMap assemblyMap = new AssemblyMap(procurement);
                assemblyMap.Show();
            }
        }

        private void SupplyMonitoringButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchLV.Items.Count > 0)
            {
                var procurements = SearchLV.ItemsSource.Cast<Procurement>().ToList();
                _ = MainFrame.Navigate(new SupplyMonitoringPage(procurements));
            }
            else
            {
                MessageBox.Show("Список тендеров пуст!");
            }
        }

        private void NavigateToEPlatformURL_Click(object sender, RoutedEventArgs e)
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