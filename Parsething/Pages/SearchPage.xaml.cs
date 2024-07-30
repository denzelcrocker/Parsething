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
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Parsething.Pages
{
    public partial class SearchPage : Page
    {
        private Frame MainFrame { get; set; } = null!;
        private List<Law>? Laws { get; set; }
        private List<ProcurementState>? ProcurementStates { get; set; }
        private List<Employee>? Employees { get; set; }
        private List<LegalEntity>? LegalEntities { get; set; }
        private List<Procurement>? AllProcurements { get; set; } = new List<Procurement>();
        private List<Procurement>? FoundProcurements { get; set; }
        private List<Procurement>? Procurements { get; set; }
        private List<ProcurementsEmployee>? ProcurementsEmployees { get; set; }
        private const int PageSize = 20;
        private int CurrentPage = 1;
        private bool _isAscending = false; 
        private string _currentSortingField = "";

        public SearchPage(List<Procurement>? procurements)
        {
            InitializeComponent();
            Procurements = new List<Procurement>();


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
        private async Task RefreshData()
        {
            if (!IsSearchCriteriaEmpty())
            {
                Procurements = GET.View.ProcurementsBy(
                    SearchCriteria.Instance.ProcurementId,
                    SearchCriteria.Instance.ProcurementNumber,
                    SearchCriteria.Instance.Law,
                    SearchCriteria.Instance.ProcurementState,
                    SearchCriteria.Instance.INN,
                    SearchCriteria.Instance.Employee,
                    SearchCriteria.Instance.OrganizationName,
                    SearchCriteria.Instance.LegalEntity,
                    SearchCriteria.Instance.DateType,
                    SearchCriteria.Instance.StartDate,
                    SearchCriteria.Instance.EndDate,
                    PageSize,
                    CurrentPage,
                    _currentSortingField,
                    _isAscending);
            }
            else if (AllProcurements != null && AllProcurements.Count > 0)
            {
                Procurements = AllProcurements.Take(PageSize).ToList();
            }
            else
            {
                Procurements = GET.View.ProcurementsBy("", "", "", "", "", "", "", "", "", "", "", PageSize, CurrentPage, _currentSortingField, _isAscending);
            }

            GET.View.PopulateComponentStates(Procurements);
            SearchLV.ItemsSource = Procurements;
            RestoreSearchCriteria();
        }
        private async void LoadInitialData()
        {

            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;

            LegalEntities = GET.View.LegalEntities();
            LegalEntity.ItemsSource = LegalEntities;


            Procurements = AllProcurements.Take(PageSize).ToList();
            GET.View.PopulateComponentStates(Procurements);
            SearchLV.ItemsSource = Procurements;


            RestoreSearchCriteria();
        }
        private async void InitializeData()
        {

            // Загружаем данные для выпадающих списков
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates =  GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;

            LegalEntities = GET.View.LegalEntities();
            LegalEntity.ItemsSource = LegalEntities;


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
                    SearchCriteria.Instance.LegalEntity,
                    SearchCriteria.Instance.DateType,
                    SearchCriteria.Instance.StartDate,
                    SearchCriteria.Instance.EndDate,
                    PageSize,
                    CurrentPage,
                    _currentSortingField,
                    _isAscending);
            }
            else
            {
                Procurements = GET.View.ProcurementsBy("", "", "", "", "", "", "","","", "", "", PageSize, CurrentPage, _currentSortingField, _isAscending);
            }
            GET.View.PopulateComponentStates(Procurements);

            SearchLV.ItemsSource = Procurements;


            RestoreSearchCriteria();
        }

        private bool IsSearchCriteriaEmpty()
        {
            return string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementId) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementNumber) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.Law) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementState) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.INN) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.Employee) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.OrganizationName) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.LegalEntity) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.DateType) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.StartDate) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.EndDate);
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
            LegalEntity.SelectedItem = LegalEntities?.FirstOrDefault(l => l.Name == SearchCriteria.Instance.LegalEntity);
            DateType.SelectedItem = SearchCriteria.Instance.DateType;

            if (DateTime.TryParse(SearchCriteria.Instance.StartDate, out DateTime startDate))
                StartDate.SelectedDate = startDate;
            else
                StartDate.SelectedDate = null;

            if (DateTime.TryParse(SearchCriteria.Instance.EndDate, out DateTime endDate))
                EndDate.SelectedDate = endDate;
            else
                EndDate.SelectedDate = null;
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
            Procurement procurement = null;

            if (sender is MenuItem menuItem)
                procurement = menuItem.DataContext as Procurement;
            else if (sender is Button button)
                procurement = button.DataContext as Procurement;

            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement, Procurements, true));
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is Procurement procurement)
            {
                if (procurement.RequestUri != null)
                {
                    string url = procurement.RequestUri.ToString();
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;

            SearchCriteria.Instance.ClearData();
            SearchLV.ItemsSource = null;
            FoundProcurements?.Clear();

            string id = SearchId.Text;
            string number = SearchNumber.Text;
            string law = (Law.SelectedItem as Law)?.Number ?? string.Empty;
            string procurementState = (ProcurementState.SelectedItem as ProcurementState)?.Kind ?? string.Empty;
            string inn = SearchINN.Text;
            string employee = (Employee.SelectedItem as Employee)?.FullName ?? string.Empty;
            string organizationName = OrganizationName.Text;
            string legalEntity = (LegalEntity.SelectedItem as LegalEntity)?.Name ?? string.Empty;
            string dateType = (DateType.SelectedItem as ComboBoxItem)?.Tag as string ?? string.Empty;
            string startDate = StartDate.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            string endDate = EndDate.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;

            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(number) || !string.IsNullOrEmpty(law) ||
                !string.IsNullOrEmpty(procurementState) || !string.IsNullOrEmpty(inn) ||
                !string.IsNullOrEmpty(employee) || !string.IsNullOrEmpty(organizationName) ||
                !string.IsNullOrEmpty(legalEntity) || !string.IsNullOrEmpty(dateType) ||
                !string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate))
            {
                FoundProcurements = GET.View.ProcurementsBy(id, number, law, procurementState, inn, employee, organizationName, legalEntity, dateType, startDate, endDate, PageSize, CurrentPage, _currentSortingField, _isAscending);
                
                GET.View.PopulateComponentStates(FoundProcurements);
                
                SearchLV.ItemsSource = FoundProcurements;
                Procurements = FoundProcurements;
            }

            SaveSearchCriteria(id, number, law, procurementState, inn, employee, organizationName, legalEntity, dateType, startDate, endDate);

        }
        private void SaveSearchCriteria(string id, string number, string law, string procurementState, string inn, string employee, string organizationName, string legalEntity, string dateType, string startDate, string endDate)
        {
            SearchCriteria.Instance.ProcurementId = id;
            SearchCriteria.Instance.ProcurementNumber = number;
            SearchCriteria.Instance.Law = law;
            SearchCriteria.Instance.ProcurementState = procurementState;
            SearchCriteria.Instance.INN = inn;
            SearchCriteria.Instance.Employee = employee;
            SearchCriteria.Instance.OrganizationName = organizationName;
            SearchCriteria.Instance.LegalEntity = legalEntity;
            SearchCriteria.Instance.DateType = dateType;
            SearchCriteria.Instance.StartDate = startDate;
            SearchCriteria.Instance.EndDate = endDate;
        }

        private void Search_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, Procurements, true, true));
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
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
            decimal contractAmount = procurement.ContractAmount ?? 0m;
            decimal reserveContractAmount = procurement.ReserveContractAmount ?? 0m;
            decimal calculatingAmountVal = procurement.CalculatingAmount ?? 0m;
            decimal purchaseAmountVal = procurement.PurchaseAmount ?? 0m;

            if (procurement.ContractAmount != null && procurement.ReserveContractAmount == null && procurement.CalculatingAmount != null)
            {
                overallAmount += contractAmount;
                profitReal += contractAmount - purchaseAmountVal;
                profitCalculate += contractAmount - calculatingAmountVal;
                calculatingAmount += calculatingAmountVal;
                purchaseAmount += purchaseAmountVal;
                overallAmountCalculate += contractAmount;
            }
            else if (procurement.ReserveContractAmount != null && procurement.CalculatingAmount != null)
            {
                overallAmount += reserveContractAmount;
                profitReal += reserveContractAmount - purchaseAmountVal;
                profitCalculate += contractAmount - calculatingAmountVal;
                calculatingAmount += calculatingAmountVal;
                purchaseAmount += purchaseAmountVal;
                overallAmountCalculate += contractAmount;
            }
        }

        private void DisplayOverallInfo(decimal? overallAmount, decimal? overallAmountCalculate, decimal? calculatingAmount, decimal? purchaseAmount, decimal? profitCalculate, decimal? profitReal)
        {
            decimal overallAmountVal = overallAmount ?? 0m;
            decimal overallAmountCalculateVal = overallAmountCalculate ?? 0m;
            decimal calculatingAmountVal = calculatingAmount ?? 0m;
            decimal purchaseAmountVal = purchaseAmount ?? 0m;
            decimal profitCalculateVal = profitCalculate ?? 0m;
            decimal profitRealVal = profitReal ?? 0m;

            OverallAmount.Text = overallAmountVal.ToString("N2") + " р.";
            if (calculatingAmountVal != 0 && purchaseAmountVal != 0)
            {
                AvgCalculationProfit.Text = $"{profitCalculateVal:N2} р. ({((overallAmountCalculateVal - calculatingAmountVal) / calculatingAmountVal * 100):N1} %)";
                AvgPurchaseProfit.Text = $"{profitRealVal:N2} р. ({((overallAmountVal - purchaseAmountVal) / purchaseAmountVal * 100):N1} %)";
            }
            else
            {
                AvgCalculationProfit.Text = "0 р. (0.0 %)";
                AvgPurchaseProfit.Text = "0 р. (0.0 %)";
            }
        }

        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Procurement procurement = menuItem.DataContext as Procurement;
                if (procurement != null)
                {
                    AssemblyMap assemblyMap = new AssemblyMap(procurement);
                    assemblyMap.Show();
                }
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
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
            {
                if (procurement.Platform?.Address != null)
                {
                    string url = procurement.Platform.Address.ToString();
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
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

            if (AllProcurements != null && AllProcurements.Count > 0) // обязательно посмотреть!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                var newItems = AllProcurements.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                if (newItems.Count > 0)
                {
                    foreach (var item in newItems)
                    {
                        Procurements.Add(item);
                    }

                    SearchLV.ItemsSource = null;
                    SearchLV.ItemsSource = Procurements;
                }
            }
            else 
            {
                var newItems = GET.View.ProcurementsBy(
                        SearchCriteria.Instance.ProcurementId,
                        SearchCriteria.Instance.ProcurementNumber,
                        SearchCriteria.Instance.Law,
                        SearchCriteria.Instance.ProcurementState,
                        SearchCriteria.Instance.INN,
                        SearchCriteria.Instance.Employee,
                        SearchCriteria.Instance.OrganizationName,
                        SearchCriteria.Instance.LegalEntity,
                        SearchCriteria.Instance.DateType,
                        SearchCriteria.Instance.StartDate,
                        SearchCriteria.Instance.EndDate,
                        PageSize,
                        CurrentPage,
                        _currentSortingField,
                        _isAscending);

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
        private async void SearchLV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == e.ExtentHeight - e.ViewportHeight)
            {
                await LoadMoreItems();
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
                    procurements = GET.View.ApplicationsBy(procurement.DisplayId);
                MainFrame.Navigate(new SearchPage(procurements));
            }
        }
        private void SortByField(object sender, MouseButtonEventArgs e)
        {
            CurrentPage = 1;

            if (sender is Label label && label.Tag is string field)
            {
                ClearSortingArrows();

                if (_currentSortingField == field)
                {
                    _isAscending = !_isAscending;
                }
                else
                {
                    _isAscending = true;
                }

                _currentSortingField = field;
                
                if (!IsSearchCriteriaEmpty())
                {
                    AllProcurements = GET.View.ProcurementsBy(
                        SearchCriteria.Instance.ProcurementId,
                        SearchCriteria.Instance.ProcurementNumber,
                        SearchCriteria.Instance.Law,
                        SearchCriteria.Instance.ProcurementState,
                        SearchCriteria.Instance.INN,
                        SearchCriteria.Instance.Employee,
                        SearchCriteria.Instance.OrganizationName,
                        SearchCriteria.Instance.LegalEntity,
                        SearchCriteria.Instance.DateType,
                        SearchCriteria.Instance.StartDate,
                        SearchCriteria.Instance.EndDate,
                        PageSize,
                        CurrentPage,
                        _currentSortingField,
                        _isAscending);

                    Procurements = AllProcurements.Take(PageSize).ToList();
                }
                else if (AllProcurements != null && AllProcurements.Count > 0)
                {
                    AllProcurements = SortProcurements(AllProcurements, field, _isAscending);
                    Procurements = AllProcurements.Take(PageSize).ToList();
                }
                else
                {
                    AllProcurements = GET.View.ProcurementsBy("", "", "", "", "", "", "", "", "", "", "", int.MaxValue, 1, _currentSortingField, _isAscending);
                    Procurements = AllProcurements.Take(PageSize).ToList();
                }

                GET.View.PopulateComponentStates(Procurements);
                SearchLV.ItemsSource = Procurements;

                UpdateSortingArrow(label, _isAscending);
            }
        }

        private List<Procurement> SortProcurements(List<Procurement> procurements, string field, bool isAscending)
        {
            switch (field)
            {
                case "Law":
                    return isAscending ? procurements.OrderBy(p => p.Law.Number).ToList() : procurements.OrderByDescending(p => p.Law.Number).ToList();
                case "ResultDate":
                    return isAscending ? procurements.OrderBy(p => p.ReserveContractAmount).ToList() : procurements.OrderByDescending(p => p.ResultDate).ToList();
                case "SigningDeadline":
                    return isAscending ? procurements.OrderBy(p => p.SigningDeadline).ToList() : procurements.OrderByDescending(p => p.SigningDeadline).ToList();
                case "ActualDeliveryDate":
                    return isAscending ? procurements.OrderBy(p => p.ActualDeliveryDate).ToList() : procurements.OrderByDescending(p => p.ActualDeliveryDate).ToList();
                case "MaxAcceptanceDate":
                    return isAscending ? procurements.OrderBy(p => p.MaxAcceptanceDate).ToList() : procurements.OrderByDescending(p => p.MaxAcceptanceDate).ToList();
                default:
                    return procurements;
            }
        }

        private int ExtractLawNumber(string law)
        {
            var match = Regex.Match(law, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        private void UpdateSortingArrow(Label label, bool isAscending)
        {
            ClearSortingArrows();
            string arrow = isAscending ? "↑" : "↓";
            label.Content = label.Content.ToString().TrimEnd('↑', '↓') + arrow;
        }

        private void ClearSortingArrows()
        {
            foreach (var child in SortingHeadersGrid.Children)
            {
                if (child is Label label)
                {
                    label.Content = label.Content.ToString().TrimEnd('↑', '↓');
                }
            }
        }
    }
}