﻿using DatabaseLibrary.Queries;
using Parsething.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для EPlatformSpecialistPage.xaml
    /// </summary>
    public partial class EPlatformSpecialistPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsMethodsGroupings { get; set; }

        private DateTime StartDate = new DateTime();
        private bool _isAscendingDeadline = true;
        private bool _isAscendingLaw = true;
        private bool _isAscendingResultDate = true;
        SolidColorBrush buttonBrush = new SolidColorBrush();

        public EPlatformSpecialistPage()
        {
            InitializeComponent();

            string theme = UserConfig.LoadTheme();
            Color defaultColor = (Color)ColorConverter.ConvertFromString(theme == "Dark" ? "#383838" : "#D9D9D9");
            buttonBrush = new SolidColorBrush(defaultColor);

            var globalUsingValues = Classes.GlobalUsingValues.Instance;
            StartDate = globalUsingValues.StartDate;

            int countOfMethods = 0;

            New.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Новый", GET.KindOf.ProcurementState)); // Новый

            Calculated.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Посчитан", GET.KindOf.ProcurementState)); // Посчитан

            RetreatCalculate.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", StartDate)); // Отбой

            DrawUp.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформить", GET.KindOf.ProcurementState)); // Оформить

            Issued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", GET.KindOf.ProcurementState)); // Оформллены

            ForSend.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", false, GET.KindOf.Deadline));  // К отправке

            OverdueIssued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", true, GET.KindOf.Deadline));// Просрочены

            Bargaining.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", false, GET.KindOf.ResultDate)); // Торги

            QuotesCombobox.Items.Clear();
            QuotesCombobox.Text = "Сп-бы опр-я:";
            ProcurementsMethodsGroupings = GET.View.ProcurementsGroupByMethod();
            foreach (var item in ProcurementsMethodsGroupings)
            {
                countOfMethods += item.CountOfProcurements;
            }
            Quotes.Text = countOfMethods.ToString(); // Котировки (общее количество)
            foreach (var item in ProcurementsMethodsGroupings)
            {
                QuotesCombobox.Items.Add(item); // Котировки (по методам)
            }// Котировки

            OverdueSended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", true, GET.KindOf.ResultDate)); // Просрочены

            Cancellation.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отмена", StartDate)); // Отменены

            Rejected.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отклонен", StartDate)); // Отклонены

            Lost.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Проигран", StartDate)); // Проиграны

            WonPartOne.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState)); // Выиграны 1ч

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState)); // Выиграны 2ч
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            NavigationState.AddLastSelectedProcurement(ProcurementsListView);
        }
        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
            {
                var procurements = selectedGrouping.Procurements ?? new List<Procurement>();
                GlobalUsingValues.Instance.AddProcurements(procurements);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }
        private void SortByDeadline(object sender, MouseButtonEventArgs e)
        {
            ClearSortingArrows();
            if (_isAscendingDeadline)
            {
                ProcurementsListView.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline).ToList();
                _isAscendingDeadline = false;
            }
            else
            {
                ProcurementsListView.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderByDescending(p => p.Deadline).ToList();
                _isAscendingDeadline = true;
            }
            UpdateSortingArrow((Label)sender, _isAscendingDeadline);
        }

        private void SortByResultDate(object sender, MouseButtonEventArgs e)
        {
            ClearSortingArrows();
            if (_isAscendingResultDate)
            {
                ProcurementsListView.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.ResultDate).ToList();
                _isAscendingResultDate = false;
            }
            else
            {
                ProcurementsListView.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderByDescending(p => p.ResultDate).ToList();
                _isAscendingResultDate = true;
            }
            UpdateSortingArrow((Label)sender, _isAscendingResultDate);
        }

        private void SortByLaw(object sender, MouseButtonEventArgs e)
        {
            ClearSortingArrows();
            if (_isAscendingLaw)
            {
                ProcurementsListView.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => ExtractLawNumber(p.Law.Number)).ToList();
                _isAscendingLaw = false;
            }
            else
            {
                ProcurementsListView.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderByDescending(p => ExtractLawNumber(p.Law.Number)).ToList();
                _isAscendingLaw = true;
            }
            UpdateSortingArrow((Label)sender, _isAscendingLaw);
        }

        private int ExtractLawNumber(string law)
        {
            var match = Regex.Match(law, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        private void UpdateSortingArrow(Label label, bool isAscending)
        {
            string arrow = isAscending ? "↑" : "↓";
            label.Content = label.Content.ToString().TrimEnd('↑', '↓') + "" + arrow;
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
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Новый", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = buttonBrush;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void CalculatedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Посчитан", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = buttonBrush;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void RetreatCalculateButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отбой", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = buttonBrush;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void DrawUpButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Оформить", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = buttonBrush;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void IssuedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Оформлен", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = buttonBrush;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void ForSendButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Оформлен", false, GET.KindOf.Deadline) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = buttonBrush;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void OverdueIssuedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Оформлен", true, GET.KindOf.Deadline) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = buttonBrush;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void BargainingButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отправлен", false, GET.KindOf.ResultDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = buttonBrush;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отправлен", true, GET.KindOf.ResultDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = buttonBrush;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void CancellationButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отмена", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = buttonBrush;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void RejectedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отклонен", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = buttonBrush;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void LostButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Проигран", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = buttonBrush;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = buttonBrush;
            WonPartTwoButton.Background = Brushes.Transparent;
            ClearSortingArrows();
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (procurements != null)
                ProcurementsListView.ItemsSource = procurements;

            NewButton.Background = Brushes.Transparent;
            CalculatedButton.Background = Brushes.Transparent;
            RetreatCalculateButton.Background = Brushes.Transparent;
            DrawUpButton.Background = Brushes.Transparent;
            IssuedButton.Background = Brushes.Transparent;
            ForSendButton.Background = Brushes.Transparent;
            OverdueIssuedButton.Background = Brushes.Transparent;
            BargainingButton.Background = Brushes.Transparent;
            OverdueSendedButton.Background = Brushes.Transparent;
            CancellationButton.Background = Brushes.Transparent;
            RejectedButton.Background = Brushes.Transparent;
            LostButton.Background = Brushes.Transparent;
            WonPartOneButton.Background = Brushes.Transparent;
            WonPartTwoButton.Background = buttonBrush;
            ClearSortingArrows();
        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            {
                NavigationState.LastSelectedProcurement = procurement;

                _ = MainFrame.Navigate(new CardOfProcurement(procurement));
            }
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
        private void NavigateToProcurementEPlatform_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null && procurement.Platform.Address != null)
            {
                string url = procurement.Platform.Address.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void BetCalculatingButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Procurement procurement = button?.DataContext as Procurement;

            if (procurement != null)
            {
                Popup popup = Functions.FindPopup.FindPopupByProcurementId(procurement.Id, button);

                if (popup != null)
                {
                    popup.IsOpen = !popup.IsOpen;
                    TextBox calculatingAmountTextBox = popup.FindName("CalculatingAmountPopUp") as TextBox;
                    TextBox betTextBox = popup.FindName("BetPopUp") as TextBox;
                    TextBox percentageTextBox = popup.FindName("PercentagePopUp") as TextBox;

                    if (calculatingAmountTextBox != null && betTextBox != null && percentageTextBox != null)
                    {
                        calculatingAmountTextBox.Text = procurement.CalculatingAmount.ToString();
                        betTextBox.Text = procurement.Bet.ToString();
                        if (procurement.Bet != null && procurement.CalculatingAmount != null)
                        percentageTextBox.Text = (((decimal)procurement.Bet - (decimal)procurement.CalculatingAmount) / (decimal)procurement.CalculatingAmount * 100).ToString("N1") + "%";
                    }
                }
            }

        }

        private void SaveBetPopUp_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Procurement procurement = button?.DataContext as Procurement;

            if (procurement != null)
            {
                Grid parentGrid = button.Parent as Grid;

                if (parentGrid != null)
                {
                    Border parentBorder = parentGrid.Parent as Border;

                    if (parentBorder != null)
                    {
                        Popup popup = parentBorder.Parent as Popup;

                        if (popup != null)
                        {
                            TextBox betTextBox = popup.FindName("BetPopUp") as TextBox;

                            if (betTextBox != null)
                            {
                                if (decimal.TryParse(betTextBox.Text, out decimal betValue))
                                {
                                    procurement.Bet = betValue;
                                    PULL.Procurement(procurement);
                                    popup.IsOpen = !popup.IsOpen;
                                    MessageBox.Show("Ставка успешно сохранена! Обновите страницу!");

                                }
                            }
                        }
                    }
                }
            }

        }

        private void BetPopUp_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox betTextBox = sender as TextBox;

            Grid parentGrid = betTextBox.Parent as Grid;

            if (parentGrid != null)
            {
                Border parentBorder = parentGrid.Parent as Border;

                if (parentBorder != null)
                {
                    Popup popup = parentBorder.Parent as Popup;

                    if (popup != null)
                    {
                        TextBox percentageTextBox = popup.FindName("PercentagePopUp") as TextBox;

                        if (percentageTextBox != null)
                        {
                            if (decimal.TryParse(betTextBox.Text, out decimal betValue))
                            {
                                Procurement procurement = popup.DataContext as Procurement;

                                if (procurement != null && procurement.CalculatingAmount != null)
                                {
                                    decimal percentage = ((betValue - (decimal)procurement.CalculatingAmount) / (decimal)procurement.CalculatingAmount * 100);
                                    percentageTextBox.Text = percentage.ToString("N1") + "%";
                                }
                            }
                        }
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

        private void ServiceId_MouseDown(object sender, MouseButtonEventArgs e)
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
    }
}
