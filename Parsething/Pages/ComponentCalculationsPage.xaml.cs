using DatabaseLibrary.Entities.ProcurementProperties;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using Parsething.Classes;
using Parsething.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
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
using static Parsething.Functions.ListViewInitialization;


namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для ComponentCalculationsPage.xaml
    /// </summary>
    public partial class ComponentCalculationsPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Comment>? Comments = new List<Comment>();

        private List<ComponentCalculation>? ComponentCalculations = new List<ComponentCalculation>();

        private List<ComponentState>? ComponentStates = new List<ComponentState>();


        private Procurement? Procurement { get; set; }

        private bool IsSearch;

        private bool IsCalculation;

        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));

        static List<string> ProcurementStates = new List<string>() { "Новый", "Посчитан", "Оформить", "Оформлен", "Отправлен", "Отмена", "Отклонен", "Проверка" };


        public ComponentCalculationsPage(Procurement procurement, bool isCalculation)
        {
            InitializeComponent();
            procurement = GET.View.ProcurementBy(procurement.Id);
            IsCalculation = isCalculation;
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;
            if (procurement != null)
            {
                Procurement = procurement;
                Id.Text = Procurement.DisplayId.ToString();
                int actualProcurementId = GET.Aggregate.GetActualProcurementId(procurement.Id, procurement.ParentProcurementId);
                Comments = GET.View.CommentsBy(actualProcurementId);
                CommentsListView.ItemsSource = Comments;
                ScrollToBottom();
                ComponentCalculations = GET.View.ComponentCalculationsBy(procurement.Id);
                if (isCalculation)
                {
                    PurchaseOrCalculatiing.Text = "Расчет";
                    CalculatingPanel.Visibility = Visibility.Visible;
                    ColumnsNamesCalculating.Visibility = Visibility.Visible;
                    PurchasePanel.Visibility = Visibility.Hidden;
                    ColumnsNamesPurchase.Visibility = Visibility.Hidden;


                    foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                    {
                        if (componentCalculation.Price != null && componentCalculation.Count != null)
                            calculatingAmount += (componentCalculation.Price * componentCalculation.Count);
                    }

                    if (calculatingAmount > Procurement.InitialPrice)
                        CalculationPrice.Foreground = Red;
                    MaxPrice.Text = Procurement.InitialPrice.ToString("N2", CultureInfo.CurrentCulture);
                    CalculationPrice.Text = calculatingAmount.HasValue ? calculatingAmount.Value.ToString("N2") : "0.00";
                    var visibleEmployeeIds = new[] { 1, 2, 3, 4 };
                    var currentPositionId = ((Employee)Application.Current.MainWindow.DataContext).PositionId;

                    if (visibleEmployeeIds.Contains(currentPositionId))
                    {
                        if (Procurement.ProcurementState.Kind == "Новый")
                        {
                            CalculatedButton.Visibility = Visibility.Visible;
                            ForCheckButton.Visibility = Visibility.Visible;
                        }
                        else if (Procurement.ProcurementState.Kind == "Оформить")
                        {
                            IssuedButton.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    PurchaseOrCalculatiing.Text = "Закупка";
                    PurchasePanel.Visibility = Visibility.Visible;
                    ColumnsNamesPurchase.Visibility = Visibility.Visible;
                    CalculatingPanel.Visibility = Visibility.Hidden;
                    ColumnsNamesCalculating.Visibility = Visibility.Hidden;
                    ComponentStates = GET.View.ComponentStates();
                    SameComponentState.ItemsSource = ComponentStates;

                    foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                    {
                        if (componentCalculation.PricePurchase != null && componentCalculation.Count != null)
                            purchaseAmount += (componentCalculation.PricePurchase * componentCalculation.Count);
                    }

                    if (Procurement.ReserveContractAmount != null)
                    {
                        ContractPrice.Text = Procurement.ReserveContractAmount.HasValue ? Procurement.ReserveContractAmount.Value.ToString("N2") : "0.00";
                        if (purchaseAmount > Procurement.ReserveContractAmount)
                            PurchasePrice.Foreground = Red;
                    }
                    else
                    {
                        ContractPrice.Text = Procurement.ContractAmount.HasValue ? Procurement.ContractAmount.Value.ToString("N2") : "0.00";
                        if (purchaseAmount > Procurement.ContractAmount)
                            PurchasePrice.Foreground = Red;
                    }
                    PurchasePrice.Text = purchaseAmount.HasValue ? purchaseAmount.Value.ToString("N2") : "0.00";
                }
                ComponentCalculationsListViewInitialization(isCalculation, ComponentCalculations, ComponentCalculationsListView, CalculationPrice, PurchasePrice, Procurement);
            }
            GoToComments.Background = Brushes.LightGray;
            GoToPassports.Background = Brushes.Transparent;
            CommentsGrid.Visibility = Visibility.Visible;
            PassportsListView.Visibility = Visibility.Hidden;
            SavePassportButton.Visibility = Visibility.Hidden;
        }
        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Получаем ListView
            var listView = sender as ListView;

            if (listView != null)
            {
                // Получаем текущий ScrollViewer, связанный с ListView
                var scrollViewer = GetScrollViewer(listView);

                if (scrollViewer != null)
                {
                    // Прокручиваем ListView на величину, пропорциональную дельте события
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
                    e.Handled = true;
                }
            }
        }

        // Метод для получения ScrollViewer из ListView
        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj == null)
                return null;

            // Проходим по визуальному дереву, чтобы найти ScrollViewer
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is ScrollViewer scrollViewer)
                    return scrollViewer;

                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommentsTextBox.Text != "")
            {
                Comment? comment = new Comment { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = GET.Aggregate.GetActualProcurementId(Procurement.Id, Procurement.ParentProcurementId), Text = CommentsTextBox.Text, IsTechnical = IsTechnical.IsChecked };
                CommentsTextBox.Clear();
                IsTechnical.IsChecked = false;
                PUT.Comment(comment);
                CommentsListView.ItemsSource = null;
                Comments.Clear();
                int actualProcurementId = GET.Aggregate.GetActualProcurementId(Procurement.Id, Procurement.ParentProcurementId);
                Comments = GET.View.CommentsBy(actualProcurementId);
                CommentsListView.ItemsSource = Comments;
                ScrollToBottom();
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            var currentUserId = ((Employee)Application.Current.MainWindow.DataContext).Id;

            if (Procurement.IsCalculationBlocked == true && Procurement.CalculatingUserId == currentUserId && IsCalculation == true)
            {
                Procurement.IsCalculationBlocked = false;
                Procurement.CalculatingUserId = null;
                Procurement.CalculatingAmount = Convert.ToDecimal(CalculationPrice.Text);
                PULL.Procurement(Procurement);
            }
            else if (Procurement.IsPurchaseBlocked == true && Procurement.PurchaseUserId == currentUserId && IsCalculation == false)
            {
                Procurement.IsPurchaseBlocked = false;
                Procurement.PurchaseUserId = null;
                Procurement.PurchaseAmount = Convert.ToDecimal(PurchasePrice.Text);
                PULL.Procurement(Procurement);
            }
            Procurement existingProcurement = GlobalUsingValues.Instance.Procurements.FirstOrDefault(p => p.Id == Procurement.Id);
            if (existingProcurement != null)
            {
                existingProcurement.CalculatingAmount = Procurement.CalculatingAmount;
                existingProcurement.PurchaseAmount = Procurement.PurchaseAmount;
                existingProcurement.ContractAmount = Procurement.ContractAmount;
                existingProcurement.ReserveContractAmount = Procurement.ReserveContractAmount;
                existingProcurement.ProcurementStateId = Procurement.ProcurementStateId;
                existingProcurement.ProcurementState = Procurement.ProcurementState;

            }
            GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
            MainFrame.GoBack();
        }

        private void AddDivisionCalculating_Click(object sender, RoutedEventArgs e)
        {
            ButtonAddDivision_Click(sender, e, Procurement);
        }

        private void AddDivisionPurchase_Click(object sender, RoutedEventArgs e)
        {
            ButtonAddDivision_Click(sender, e, Procurement);
        }

        private void GoToPassports_Click(object sender, RoutedEventArgs e)
        {
            GoToComments.Background = Brushes.Transparent;
            GoToPassports.Background = Brushes.LightGray;
            CommentsGrid.Visibility = Visibility.Hidden;
            PassportsListView.Visibility = Visibility.Visible;
            SavePassportButton.Visibility = Visibility.Visible;

            MonitorPassportTextBox.Text = Procurement.PassportOfMonitor;
            PcPassportTextBox.Text = Procurement.PassportOfPc;
            MonoblockPassportTextBox.Text = Procurement.PassportOfMonoblock;
            NotebookPassportTextBox.Text = Procurement.PassportOfNotebook;
            AwPassportTextBox.Text = Procurement.PassportOfAw;
            UpsPassportTextBox.Text = Procurement.PassportOfUps;
        }

        private void GoToComments_Click(object sender, RoutedEventArgs e)
        {
            GoToComments.Background = Brushes.LightGray;
            GoToPassports.Background = Brushes.Transparent;
            CommentsGrid.Visibility = Visibility.Visible;
            PassportsListView.Visibility = Visibility.Hidden;
            SavePassportButton.Visibility = Visibility.Hidden;
        }

        private void SavePassportButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement.PassportOfMonitor = MonitorPassportTextBox.Text;
            Procurement.PassportOfPc = PcPassportTextBox.Text;
            Procurement.PassportOfMonoblock = MonoblockPassportTextBox.Text;
            Procurement.PassportOfNotebook = NotebookPassportTextBox.Text;
            Procurement.PassportOfUps = UpsPassportTextBox.Text;
            Procurement.PassportOfAw = AwPassportTextBox.Text;
            PULL.Procurement(Procurement);
            AutoClosingMessageBox.ShowAutoClosingMessageBox("Паспорта успешно сохранены", "Информация", 1000);
        }

        private void SavePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            if (Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id || Procurement.PurchaseUserId == null)
            {
                if (UpdateComponentCalculationListView(SameDate, SameComponentState))
                    AutoClosingMessageBox.ShowAutoClosingMessageBox($"Успешно сохранено!", "Информация", 1000);
            }
            else
                MessageBox.Show($"Закупка сейчас редактируется пользователем: \n{GET.View.Employees().Where(e => e.Id == Procurement.PurchaseUserId).First().FullName}");
            SameDate.SelectedDate = null;
            SameComponentState.Text = null;
        }

        private void SaveCalculatingButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            if (ProcurementStates.Contains(Procurement.ProcurementState.Kind))
            {
                MessageBoxResult result = MessageBox.Show("Сохранение также перезапишет данные в закупке. Продолжить?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (Procurement.CalculatingUserId == ((Employee)Application.Current.MainWindow.DataContext).Id || Procurement.CalculatingUserId == null)
                    {
                        if (UpdateComponentCalculationListView(null, null))
                            AutoClosingMessageBox.ShowAutoClosingMessageBox($"Успешно сохранено!", "Информация", 1000);
                    }
                    else
                        MessageBox.Show($"Расчет сейчас редактируется пользователем: \n{GET.View.Employees().Where(e => e.Id == Procurement.CalculatingUserId).First().FullName}");
                }
                else { }
            }
        }
        private void ScrollToBottom()
        {
            if (CommentsListView.Items.Count > 0)
            {
                var lastItem = CommentsListView.Items[0];
                CommentsListView.ScrollIntoView(lastItem);
            }
        }

        private void CalculatedButton_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null)
            {
                Procurement.ProcurementStateId = 2;
                PULL.Procurement(Procurement);
                ForCheckButton.Visibility = Visibility.Hidden;
                CalculatedButton.Visibility = Visibility.Hidden;
                History? history = new History { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = "Посчитан" };
                PUT.History(history);
                AutoClosingMessageBox.ShowAutoClosingMessageBox($"Отправлено в посчитанные", "Информация", 900);
            }
        }

        private void ForCheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null)
            {
                Procurement.ProcurementStateId = 16;
                PULL.Procurement(Procurement);
                ForCheckButton.Visibility = Visibility.Hidden;
                CalculatedButton.Visibility = Visibility.Hidden;
                History? history = new History { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = "Проверка" };
                PUT.History(history);
                AutoClosingMessageBox.ShowAutoClosingMessageBox($"Отправлено на проверку", "Информация", 900);
            }
        }

        private void IssuedButton_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null)
            {
                Procurement.ProcurementStateId = 4;
                PULL.Procurement(Procurement);
                IssuedButton.Visibility = Visibility.Hidden;
                History? history = new History { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = "Оформлен" };
                PUT.History(history);
                AutoClosingMessageBox.ShowAutoClosingMessageBox($"Отправлено в оформленные", "Информация", 900);
                GoToProcurementFolderButton.Visibility = Visibility.Hidden;
            }
        }

        private void GoToProcurementFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string networkPath = $@"\\192.168.1.128\Parsething\Tender_files\{Procurement.DisplayId}";

            try
            {
                Process.Start("explorer.exe", networkPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при попытке открыть папку: {ex.Message}");
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Id.Text);
            AutoClosingMessageBox.ShowAutoClosingMessageBox("Данные скопированы в буфер обмена", "Оповещение", 900);
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null && Procurement.RequestUri != null)
            {
                string url = Procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void GoToEditProcurement_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(Procurement));
        }
    }
}
