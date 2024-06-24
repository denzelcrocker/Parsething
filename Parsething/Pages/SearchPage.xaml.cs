using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Parsething.Classes;
using Parsething.Windows;
using DatabaseLibrary.Entities.ProcurementProperties;
using DatabaseLibrary.Entities.EmployeeMuchToMany;
using System.Windows.Controls.Primitives;

namespace Parsething.Pages
{
    public partial class SearchPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Law>? Laws { get; set; }
        private List<ProcurementState>? ProcurementStates { get; set; }
        private List<Employee>? Employees { get; set; }
        private List<Procurement> AllProcurements { get; set; } = new List<Procurement>();

        private List<Procurement>? FoundProcurements { get; set; }
        private List<Procurement>? Procurements { get; set; }
        private List<ProcurementsEmployee>? ProcurementsEmployees { get; set; }
        private const int PageSize = 20; // Размер страницы для пагинации
        private int CurrentPage = 1; // Текущая страница


        public SearchPage(List<Procurement>? procurements)
        {
            InitializeComponent();
            Procurements = new List<Procurement>(); // Инициализируем пустой список

            if (procurements != null && procurements.Count > 0)
            {
                AllProcurements = procurements;
                LoadInitialData();
            }
            else
            {
                InitializeData();
            }
        }

        private async void LoadInitialData()
        {
            ShowLoadingIndicator(true);

            // Загружаем данные для выпадающих списков
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;

            Procurements = AllProcurements.Take(PageSize).ToList();
            SearchLV.ItemsSource = Procurements;

            ShowLoadingIndicator(false);

            RestoreSearchCriteria();
        }
        private async void InitializeData()
        {
            ShowLoadingIndicator(true);

            // Загружаем данные для выпадающих списков
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates =  GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;

            // Проверяем, есть ли переданные тендеры
            if (AllProcurements != null && AllProcurements.Count > 0)
            {
                Procurements = AllProcurements.Take(PageSize).ToList();
            }
            else if (!IsSearchCriteriaEmpty())
            {
                Procurements = GET.View.ProcurementsBy(
                    SearchCriteria.Instance.ProcurementId,
                    SearchCriteria.Instance.ProcurementNumber,
                    SearchCriteria.Instance.Law,
                    SearchCriteria.Instance.ProcurementState,
                    SearchCriteria.Instance.INN,
                    SearchCriteria.Instance.Employee,
                    SearchCriteria.Instance.OrganizationName,
                    PageSize,
                    CurrentPage);
            }
            else
            {
                Procurements = GET.View.ProcurementsBy("", "", "", "", "", "", "", PageSize, CurrentPage);
            }

            SearchLV.ItemsSource = Procurements;

            ShowLoadingIndicator(false);

            RestoreSearchCriteria();
        }

        private void ShowLoadingIndicator(bool show)
        {
            LoadingGrid.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool IsSearchCriteriaEmpty()
        {
            return string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementId) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementNumber) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.Law) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementState) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.INN) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.Employee) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.OrganizationName);
        }

        private void RestoreSearchCriteria()
        {
            SearchId.Text = SearchCriteria.Instance.ProcurementId;
            SearchNumber.Text = SearchCriteria.Instance.ProcurementNumber;
            Law.SelectedItem = Laws?.FirstOrDefault(l => l.Number == SearchCriteria.Instance.Law);
            ProcurementState.SelectedItem = ProcurementStates?.FirstOrDefault(ps => ps.Kind == SearchCriteria.Instance.ProcurementState);
            SearchINN.Text = SearchCriteria.Instance.INN;
            Employee.SelectedItem = Employees?.FirstOrDefault(e => e.FullName == SearchCriteria.Instance.Employee);
            OrganizationName.Text = SearchCriteria.Instance.OrganizationName;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
            }
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

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadingIndicator(true);

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

            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(number) || !string.IsNullOrEmpty(law) ||
                !string.IsNullOrEmpty(procurementState) || !string.IsNullOrEmpty(inn) ||
                !string.IsNullOrEmpty(employee) || !string.IsNullOrEmpty(organizationName))
            {
                FoundProcurements = GET.View.ProcurementsBy(id, number, law, procurementState, inn, employee, organizationName, PageSize, CurrentPage);
                GET.View.PopulateComponentStates(FoundProcurements);
                SearchLV.ItemsSource = FoundProcurements;
                Procurements = FoundProcurements;
            }

            SaveSearchCriteria(id, number, law, procurementState, inn, employee, organizationName);

            ShowLoadingIndicator(false);
        }

        private void SaveSearchCriteria(string id, string number, string law, string procurementState, string inn, string employee, string organizationName)
        {
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
                    CalculateOverallInfo(procurement, ref overallAmount, ref profitCalculate, ref profitReal, ref calculatingAmount, ref purchaseAmount, ref overallAmountCalculate);
                }
                DisplayOverallInfo(overallAmount, overallAmountCalculate, calculatingAmount, purchaseAmount, profitCalculate, profitReal);
            }
        }

        private void CalculateOverallInfo(Procurement procurement, ref decimal? overallAmount, ref decimal? profitCalculate, ref decimal? profitReal, ref decimal? calculatingAmount, ref decimal? purchaseAmount, ref decimal? overallAmountCalculate)
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

        private void DisplayOverallInfo(decimal? overallAmount, decimal? overallAmountCalculate, decimal? calculatingAmount, decimal? purchaseAmount, decimal? profitCalculate, decimal? profitReal)
        {
            OverallAmount.Text = ((decimal)overallAmount).ToString("N2") + " р.";
            if (calculatingAmount != 0 && purchaseAmount != 0)
            {
                AvgCalculationProfit.Text = $"{profitCalculate} р. ({(double?)((overallAmountCalculate - calculatingAmount) / calculatingAmount * 100):N1} %)";
                AvgPurchaseProfit.Text = $"{profitReal} р. ({(double?)((overallAmount - purchaseAmount) / purchaseAmount * 100):N1} %)";
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
                    SetEmployeePopupText(popup, "CalculatorTextBlock", 2, 3, 4);
                    SetEmployeePopupText(popup, "ManagerTextBlock", 5, 6, 8);
                }
            }
        }

        private void SetEmployeePopupText(Popup popup, string textBlockName, params int[] positionIds)
        {
            TextBlock textBlock = popup.FindName(textBlockName) as TextBlock;
            if (textBlock != null)
            {
                textBlock.Text = ProcurementsEmployees.LastOrDefault(pe => positionIds.Contains(pe.Employee.PositionId))?.Employee.FullName;
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

        private async Task LoadMoreItems()
        {
            CurrentPage++;

            // Проверяем, есть ли переданные тендеры
            if (AllProcurements != null && AllProcurements.Count > 0)
            {
                var newItems = AllProcurements.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                if (newItems.Count > 0)
                {
                    foreach (var item in newItems)
                    {
                        Procurements.Add(item);
                    }

                    SearchLV.ItemsSource = null; // Обновляем источник данных
                    SearchLV.ItemsSource = Procurements;
                }
            }
            else
            {
                var newItems = await Task.Run(() => GET.View.ProcurementsBy(
                    SearchCriteria.Instance.ProcurementId,
                    SearchCriteria.Instance.ProcurementNumber,
                    SearchCriteria.Instance.Law,
                    SearchCriteria.Instance.ProcurementState,
                    SearchCriteria.Instance.INN,
                    SearchCriteria.Instance.Employee,
                    SearchCriteria.Instance.OrganizationName,
                    PageSize,
                    CurrentPage));

                if (newItems != null && newItems.Count > 0)
                {
                    foreach (var item in newItems)
                    {
                        Procurements.Add(item);
                    }

                    SearchLV.ItemsSource = null; // Обновляем источник данных
                    SearchLV.ItemsSource = Procurements;
                }
            }
        }
        private void SearchLV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == e.ExtentHeight - e.ViewportHeight)
            {
                LoadMoreItems();
            }
        }
        private void GoToApplicationsButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                List<Procurement> procurements = new List<Procurement> ();
                if (procurement.ParentProcurementId != null) 
                    procurements = GET.View.ApplicationsBy(procurement.ParentProcurementId);
                else
                    procurements = GET.View.ApplicationsBy(procurement.Id);
                MainFrame.Navigate(new SearchPage(procurements));
            }
        }
    }
}