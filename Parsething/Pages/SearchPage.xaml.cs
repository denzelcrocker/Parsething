﻿using Microsoft.EntityFrameworkCore;
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
using System.ComponentModel;

namespace Parsething.Pages
{
    public partial class SearchPage : Page, INotifyPropertyChanged
    {
        private Frame MainFrame { get; set; } = null!;
        private List<Law>? Laws { get; set; }
        private List<ProcurementState>? ProcurementStates { get; set; }
        private List<Employee>? Employees { get; set; }
        private List<LegalEntity>? LegalEntities { get; set; }
        private List<ShipmentPlan>? ShipmentPlans { get; set; }
        private List<ProcurementsEmployee>? ProcurementsEmployees { get; set; }
        private const int PageSize = 35;
        private int _currentPage = 1;
        private const int VisiblePagesCount = 5; // Количество видимых страниц

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isAscending = false;
        private string _currentSortingField = "";

        public SearchPage()
        {
            InitializeComponent();
            DataContext = this;

            if (GlobalUsingValues.Instance.Procurements != null && GlobalUsingValues.Instance.Procurements.Count > 0)
            {
                LoadInitialData();
            }
            else
            {
                InitializeData();
            }
            UpdateProcurements();
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateProcurements();
            }
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            // Предположим, что можно узнать общее количество страниц из ответа или другой логики.
            var totalPages = (GlobalUsingValues.Instance.Procurements?.Count ?? 0) / PageSize + 1;
            if (CurrentPage < totalPages)
            {
                CurrentPage++;
                UpdateProcurements();
            }
        }
        private void UpdatePagesPanel()
        {
            // Очистить существующие элементы в панели страниц
            PagesPanel.Children.Clear();

            int totalPages = GetTotalPages(GlobalUsingValues.Instance.Procurements?.Count ?? 0, PageSize);
            int startPage = Math.Max(1, CurrentPage - VisiblePagesCount / 2);
            int endPage = Math.Min(totalPages, CurrentPage + VisiblePagesCount / 2);

            // Убедитесь, что отображается нужное количество страниц
            if (endPage - startPage + 1 < VisiblePagesCount)
            {
                if (startPage > 1)
                    startPage = Math.Max(1, endPage - VisiblePagesCount + 1);
                else
                    endPage = Math.Min(totalPages, startPage + VisiblePagesCount - 1);
            }

            // Добавить кнопку предыдущей страницы, если это не первая страница
            if (CurrentPage > 1)
            {
                Button prevPageButton = new Button
                {
                    Content = "‹",
                    Tag = CurrentPage - 1
                };
                prevPageButton.Style = (Style)FindResource("Button.SearchNavigation");
                prevPageButton.Click += PageButton_Click;
                PagesPanel.Children.Add(prevPageButton);
            }

            // Добавить кнопки для каждой страницы в видимом диапазоне
            for (int i = startPage; i <= endPage; i++)
            {
                Button pageButton = new Button
                {
                    Content = i.ToString(),
                    Tag = i
                };
                pageButton.Style = (Style)FindResource("Button.SearchNavigation");

                if (i == CurrentPage)
                {
                    pageButton.FontWeight = FontWeights.Bold;
                }

                pageButton.Click += PageButton_Click;
                PagesPanel.Children.Add(pageButton);
            }

            // Добавить кнопку следующей страницы, если это не последняя страница
            if (CurrentPage < totalPages)
            {
                Button nextPageButton = new Button
                {
                    Content = "›",
                    Tag = CurrentPage + 1
                };
                nextPageButton.Style = (Style)FindResource("Button.SearchNavigation");
                nextPageButton.Click += PageButton_Click;
                PagesPanel.Children.Add(nextPageButton);
            }
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int pageNumber)
            {
                if (pageNumber != CurrentPage)
                {
                    CurrentPage = pageNumber;
                    UpdateProcurements();
                }
            }
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage = 1;
                UpdateProcurements();
            }
        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (GlobalUsingValues.Instance.Procurements?.Count ?? 0 + PageSize - 1) / PageSize;
            if (CurrentPage < totalPages)
            {
                CurrentPage = totalPages + 1;
                UpdateProcurements();
            }
        }
        private int GetTotalPages(int itemCount, int pageSize)
        {
            return (int)Math.Ceiling((double)itemCount / pageSize);
        }
        private void UpdateProcurements()
        {
            var procurements = GetProcurementsForPage(GlobalUsingValues.Instance.Procurements, CurrentPage, PageSize);
            SearchLV.ItemsSource = procurements;
            UpdatePagesPanel();
            PageNumberTextBlock.Text = $"Страница {CurrentPage}";
        }
        private void LoadInitialData()
        {

            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;
            ProcurementStateSecond.ItemsSource = ProcurementStates;

            LegalEntities = GET.View.LegalEntities();
            LegalEntity.ItemsSource = LegalEntities;

            ShipmentPlans = GET.View.ShipmentPlans();
            ShipmentPlan.ItemsSource = ShipmentPlans;

            var procurements = GetProcurementsForPage(GlobalUsingValues.Instance.Procurements, CurrentPage, PageSize);
            GET.View.PopulateComponentStates(procurements);

            SearchLV.ItemsSource = procurements;


            RestoreSearchCriteria();
        }
        private void InitializeData()
        {

            // Загружаем данные для выпадающих списков
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            Employees = GET.View.Employees().Where(e => e.IsAvailable != false).ToList();
            Employee.ItemsSource = Employees;

            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;
            ProcurementStateSecond.ItemsSource = ProcurementStates;

            LegalEntities = GET.View.LegalEntities();
            LegalEntity.ItemsSource = LegalEntities;

            ShipmentPlans = GET.View.ShipmentPlans();
            ShipmentPlan.ItemsSource = ShipmentPlans;
        }

        private bool IsSearchCriteriaEmpty()
        {
            return string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementId) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementNumber) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.Law) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementState) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ProcurementStateSecond) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.INN) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.Employee) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.OrganizationName) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.LegalEntity) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.DateType) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.StartDate) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.EndDate) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ComponentCalculation) &&
                   string.IsNullOrEmpty(SearchCriteria.Instance.ShipmentPlan);
        }

        private void RestoreSearchCriteria()
        {
            SearchId.Text = SearchCriteria.Instance.ProcurementId;
            SearchNumber.Text = SearchCriteria.Instance.ProcurementNumber;
            Law.SelectedItem = Laws?.FirstOrDefault(l => l.Number == SearchCriteria.Instance.Law);
            ProcurementState.SelectedItem = ProcurementStates?.FirstOrDefault(ps => ps.Kind == SearchCriteria.Instance.ProcurementState);
            ProcurementStateSecond.SelectedItem = ProcurementStates?.FirstOrDefault(ps => ps.Kind == SearchCriteria.Instance.ProcurementStateSecond);
            SearchINN.Text = SearchCriteria.Instance.INN;
            Employee.SelectedItem = Employees?.FirstOrDefault(e => e.FullName == SearchCriteria.Instance.Employee);
            OrganizationName.Text = SearchCriteria.Instance.OrganizationName;
            LegalEntity.SelectedItem = LegalEntities?.FirstOrDefault(l => l.Name == SearchCriteria.Instance.LegalEntity);

            var dateType = SearchCriteria.Instance.DateType;
            DateType.SelectedValue = dateType;

            if (DateTime.TryParse(SearchCriteria.Instance.StartDate, out DateTime startDate))
                StartDate.SelectedDate = startDate;
            else
                StartDate.SelectedDate = null;

            if (DateTime.TryParse(SearchCriteria.Instance.EndDate, out DateTime endDate))
                EndDate.SelectedDate = endDate;
            else
                EndDate.SelectedDate = null;

            ShipmentPlan.SelectedItem = ShipmentPlans?.FirstOrDefault(s => s.Kind == SearchCriteria.Instance.ShipmentPlan);
            SearchComponentCalculation.Text = SearchCriteria.Instance.ComponentCalculation;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
            }
            catch { }

            NavigationState.AddLastSelectedProcurement(SearchLV);
        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = null;

            if (sender is MenuItem menuItem)
                procurement = menuItem.DataContext as Procurement;
            else if (sender is Button button)
                procurement = button.DataContext as Procurement;

            if (procurement != null)
            {
                NavigationState.LastSelectedProcurement = procurement;

                _ = MainFrame.Navigate(new CardOfProcurement(procurement));
            }
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;

            SearchCriteria.Instance.ClearData();
            SearchLV.ItemsSource = null;

            string id = SearchId.Text;
            string number = SearchNumber.Text;
            string law = (Law.SelectedItem as Law)?.Number ?? string.Empty;
            string procurementState = (ProcurementState.SelectedItem as ProcurementState)?.Kind ?? string.Empty;
            string procurementStateSecond = (ProcurementStateSecond.SelectedItem as ProcurementState)?.Kind ?? string.Empty;
            string inn = SearchINN.Text;
            string employee = (Employee.SelectedItem as Employee)?.FullName ?? string.Empty;
            string organizationName = OrganizationName.Text;
            string legalEntity = (LegalEntity.SelectedItem as LegalEntity)?.Name ?? string.Empty;
            string dateType = (DateType.SelectedItem as ComboBoxItem)?.Tag as string ?? string.Empty;
            string startDate = StartDate.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            string endDate = EndDate.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            string componentCalculation = SearchComponentCalculation.Text;
            string shipmentPlan = (ShipmentPlan.SelectedItem as ShipmentPlan)?.Kind ?? string.Empty;

            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(number) || !string.IsNullOrEmpty(law) ||
                !string.IsNullOrEmpty(procurementState) || !string.IsNullOrEmpty(procurementStateSecond) ||
                !string.IsNullOrEmpty(inn) || !string.IsNullOrEmpty(employee) ||
                !string.IsNullOrEmpty(organizationName) || !string.IsNullOrEmpty(legalEntity) ||
                !string.IsNullOrEmpty(dateType) || !string.IsNullOrEmpty(startDate) ||
                !string.IsNullOrEmpty(endDate) || !string.IsNullOrEmpty(componentCalculation) ||
                !string.IsNullOrEmpty(shipmentPlan))
            {
                var procurements = GET.View.ProcurementsBy(id, number, law, procurementState, procurementStateSecond, inn, employee, organizationName, legalEntity, dateType, startDate, endDate, _currentSortingField, _isAscending, componentCalculation, shipmentPlan) ?? new List<Procurement>();
                GlobalUsingValues.Instance.AddProcurements(procurements);

                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);

                SearchLV.ItemsSource = GetProcurementsForPage(GlobalUsingValues.Instance.Procurements, CurrentPage, PageSize);
            }

            SaveSearchCriteria(id, number, law, procurementState, procurementStateSecond, inn, employee, organizationName, legalEntity, dateType, startDate, endDate, componentCalculation, shipmentPlan);
            UpdatePagesPanel();
        }
        private void SaveSearchCriteria(string id, string number, string law, string procurementState, string procurementStateSecond, string inn, string employee, string organizationName, string legalEntity, string dateType, string startDate, string endDate, string componentCalculation, string shipmentPlan)
        {
            SearchCriteria.Instance.ProcurementId = id;
            SearchCriteria.Instance.ProcurementNumber = number;
            SearchCriteria.Instance.Law = law;
            SearchCriteria.Instance.ProcurementState = procurementState;
            SearchCriteria.Instance.ProcurementStateSecond = procurementStateSecond;
            SearchCriteria.Instance.INN = inn;
            SearchCriteria.Instance.Employee = employee;
            SearchCriteria.Instance.OrganizationName = organizationName;
            SearchCriteria.Instance.LegalEntity = legalEntity;
            SearchCriteria.Instance.DateType = dateType;
            SearchCriteria.Instance.StartDate = startDate;
            SearchCriteria.Instance.EndDate = endDate;
            SearchCriteria.Instance.ComponentCalculation = componentCalculation;
            SearchCriteria.Instance.ShipmentPlan = shipmentPlan;
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
            {
                NavigationState.LastSelectedProcurement = procurement;
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true));
            }
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
            {
                NavigationState.LastSelectedProcurement = procurement;
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, false));
            }
        }

        private void OverallInfo_Click(object sender, RoutedEventArgs e)
        {
            decimal? overallInitialPrice = 0;
            decimal? overallAmount = 0;
            decimal? overallAmountCalculate = 0;
            decimal? profitCalculate = 0;
            decimal? profitReal = 0;
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;

            OverallInfoPopUp.IsOpen = !OverallInfoPopUp.IsOpen;
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                OverallCount.Text = GlobalUsingValues.Instance.Procurements.Count.ToString();
                foreach (Procurement procurement in GlobalUsingValues.Instance.Procurements)
                {
                    CalculateOverallInfo(procurement, ref overallInitialPrice, ref overallAmount, ref profitCalculate, ref profitReal, ref calculatingAmount, ref purchaseAmount, ref overallAmountCalculate);
                }
                DisplayOverallInfo(overallInitialPrice, overallAmount, overallAmountCalculate, calculatingAmount, purchaseAmount, profitCalculate, profitReal);
            }
        }

        private void CalculateOverallInfo(Procurement procurement, ref decimal? overallInitialPrice, ref decimal? overallAmount, ref decimal? profitCalculate, ref decimal? profitReal, ref decimal? calculatingAmount, ref decimal? purchaseAmount, ref decimal? overallAmountCalculate)
        {
            decimal initialPrice = procurement.InitialPrice;
            decimal contractAmount = procurement.ContractAmount ?? 0m;
            decimal reserveContractAmount = procurement.ReserveContractAmount ?? 0m;
            decimal calculatingAmountVal = procurement.CalculatingAmount ?? 0m;
            decimal purchaseAmountVal = procurement.PurchaseAmount ?? 0m;

            overallInitialPrice += initialPrice;
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

        private void DisplayOverallInfo(decimal? overallInitialPrice, decimal? overallAmount, decimal? overallAmountCalculate, decimal? calculatingAmount, decimal? purchaseAmount, decimal? profitCalculate, decimal? profitReal)
        {
            decimal overallAmountVal = overallAmount ?? 0m;
            decimal overallInitialPriceVal = overallInitialPrice ?? 0m;
            decimal overallAmountCalculateVal = overallAmountCalculate ?? 0m;
            decimal calculatingAmountVal = calculatingAmount ?? 0m;
            decimal purchaseAmountVal = purchaseAmount ?? 0m;
            decimal profitCalculateVal = profitCalculate ?? 0m;
            decimal profitRealVal = profitReal ?? 0m;


            OverallAmount.Text = overallAmountVal.ToString("N2") + " р.";
            OverallInitialPrice.Text = overallInitialPriceVal.ToString("N2") + " р.";
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
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
            {
                _ = MainFrame.Navigate(new SupplyMonitoringPage());
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
                MainFrame.Navigate(new SearchPage());
            }
        }
        private void SortByField(object sender, MouseButtonEventArgs e)
        {

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

                GlobalUsingValues.Instance.AddProcurements(SortProcurements(GlobalUsingValues.Instance.Procurements, field, _isAscending));
                var procurements = GetProcurementsForPage(GlobalUsingValues.Instance.Procurements, CurrentPage, PageSize);
                GET.View.PopulateComponentStates(procurements);
                SearchLV.ItemsSource = procurements;
                UpdateSortingArrow(label, _isAscending);
            }
        }

        private List<Procurement> SortProcurements(List<Procurement> procurements, string field, bool isAscending)
        {
            switch (field)
            {
                case "Law":
                    return isAscending ? procurements.OrderBy(p => p.Law.Number).ToList() : procurements.OrderByDescending(p => p.Law.Number).ToList();
                case "Deadline":
                    return isAscending ? procurements.OrderBy(p => p.Deadline).ToList() : procurements.OrderByDescending(p => p.Deadline).ToList();
                case "ResultDate":
                    return isAscending ? procurements.OrderBy(p => p.ResultDate).ToList() : procurements.OrderByDescending(p => p.ResultDate).ToList();
                case "Signing":
                    return isAscending
                        ? procurements.OrderBy(p => p.SigningDeadline == null && p.SigningDate == null && p.ConclusionDate == null ? 0
                                           : p.SigningDeadline != null && p.SigningDate == null && p.ConclusionDate == null ? 1
                                           : p.SigningDeadline != null && p.SigningDate != null && p.ConclusionDate == null ? 2
                                           : 3)
                                       .ThenBy(p => p.SigningDeadline)
                                       .ThenBy(p => p.SigningDate)
                                       .ThenBy(p => p.ConclusionDate)
                                       .ToList()
                        : procurements.OrderByDescending(p => p.SigningDeadline == null && p.SigningDate == null && p.ConclusionDate == null ? 0
                                                 : p.SigningDeadline != null && p.SigningDate == null && p.ConclusionDate == null ? 1
                                                 : p.SigningDeadline != null && p.SigningDate != null && p.ConclusionDate == null ? 2
                                                 : 3)
                                       .ThenByDescending(p => p.SigningDeadline)
                                       .ThenByDescending(p => p.SigningDate)
                                       .ThenByDescending(p => p.ConclusionDate)
                                       .ToList();
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
        public static List<Procurement> GetProcurementsForPage(List<Procurement> procurements, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page number and page size must be greater than zero.");
            }

            // Разбиваем список на страницы
            var pagedProcurements = procurements
                .Skip((pageNumber - 1) * pageSize) // Пропускаем элементы до нужной страницы
                .Take(pageSize) // Берем элементы для текущей страницы
                .ToList();

            // Применяем метод подгрузки статусов
            GET.View.PopulateComponentStates(pagedProcurements);

            return pagedProcurements;
        }

        private void DisplayId_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock? textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                Procurement? procurement = textBlock.DataContext as Procurement;
                if (procurement != null)
                {
                    Clipboard.SetText(procurement.DisplayId.ToString());
                    AutoClosingMessageBox.ShowAutoClosingMessageBox("Данные скопированы в буфер обмена", "Оповещение", 900);
                }
            }
        }

        private void DateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateType.SelectedValue != null && DateType.SelectedValue.ToString() == "HistoryDate")
            {
                ProcurementStateSecondLabel.Visibility = Visibility.Visible;
                ProcurementStateSecond.Visibility = Visibility.Visible;
            }
            else
            {
                ProcurementStateSecondLabel.Visibility = Visibility.Collapsed;
                ProcurementStateSecond.Visibility = Visibility.Collapsed;
            }
        }
    }
}