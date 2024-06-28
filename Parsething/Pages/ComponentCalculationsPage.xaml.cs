using DatabaseLibrary.Entities.ProcurementProperties;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using Parsething.Functions;
using System;
using System.Collections.Generic;
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

        private List<Procurement>? Procurements = new List<Procurement>();

        private Procurement? Procurement { get; set; }

        private bool IsSearch;

        private bool IsCalculation;

        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));

        static List<string> ProcurementStates = new List<string>() { "Новый", "Посчитан", "Оформить", "Оформлен", "Отправлен", "Отмена", "Отклонен" };


        public ComponentCalculationsPage(Procurement procurement, List<Procurement> procurements, bool isCalculation, bool isSearch)
        {
            InitializeComponent();
            IsSearch = isSearch;
            IsCalculation = isCalculation;
            Procurements = procurements;
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;
            if (procurement != null)
            {
                Procurement = procurement;
                Id.Text = Procurement.Id.ToString();

                Comments = GET.View.CommentsBy(procurement.Id);
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
                    MaxPrice.Text = Procurement.InitialPrice.ToString();
                    CalculationPrice.Text = calculatingAmount.ToString();
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
                        ContractPrice.Text = Procurement.ReserveContractAmount.ToString();
                        if (purchaseAmount > Procurement.ReserveContractAmount)
                            PurchasePrice.Foreground = Red;
                    }
                    else
                    {
                        ContractPrice.Text = Procurement.ContractAmount.ToString();
                        if (purchaseAmount > Procurement.ContractAmount)
                            PurchasePrice.Foreground = Red;
                    }
                    PurchasePrice.Text = purchaseAmount.ToString();
                }
                ComponentCalculationsListViewInitialization(isCalculation, ComponentCalculations, ComponentCalculationsListView, CalculationPrice, PurchasePrice, Procurement);
            }
            GoToComments.Background = Brushes.LightGray;
            GoToPassports.Background = Brushes.Transparent;
            CommentsGrid.Visibility = Visibility.Visible;
            PassportsListView.Visibility = Visibility.Hidden;
            SavePassportButton.Visibility = Visibility.Hidden;
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
                Comment? comment = new Comment { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = CommentsTextBox.Text, IsTechnical = IsTechnical.IsChecked };
                CommentsTextBox.Clear();
                IsTechnical.IsChecked = false;
                PUT.Comment(comment);
                CommentsListView.ItemsSource = null;
                Comments.Clear();
                Comments = GET.View.CommentsBy(Procurement.Id);
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
                PULL.Procurement(Procurement);
            }
            else if (Procurement.IsPurchaseBlocked == true && Procurement.PurchaseUserId == currentUserId && IsCalculation == false)
            {
                Procurement.IsPurchaseBlocked = false;
                Procurement.PurchaseUserId = null;
                PULL.Procurement(Procurement);
            }

            if (IsSearch)
            {
                _ = MainFrame.Navigate(new SearchPage(Procurements));
                return;
            }

            var employee = (Employee)Application.Current.MainWindow.DataContext;
            var positionKind = employee.Position.Kind;

            var navigationMap = new Dictionary<string, Page>
                {
                    { "Администратор", new Pages.AdministratorPage() },
                    { "Руководитель отдела расчетов", new Pages.HeadsOfCalculatorsPage() },
                    { "Заместитель руководителя отдела расчетов", new Pages.HeadsOfCalculatorsPage() },
                    { "Специалист отдела расчетов", new Pages.CalculatorPage() },
                    { "Руководитель тендерного отдела", new Pages.HeadsOfManagersPage() },
                    { "Заместитель руководителя тендреного отдела", new Pages.HeadsOfManagersPage() },
                    { "Специалист по работе с электронными площадками", new Pages.EPlatformSpecialistPage() },
                    { "Специалист тендерного отдела", new Pages.ManagerPage() },
                    { "Руководитель отдела закупки", new Pages.PurchaserPage() },
                    { "Заместитель руководителя отдела закупок", new Pages.PurchaserPage() },
                    { "Специалист закупки", new Pages.PurchaserPage() },
                    { "Руководитель отдела производства", new Pages.AssemblyPage() },
                    { "Заместитель руководителя отдела производства", new Pages.AssemblyPage() },
                    { "Специалист по производству", new Pages.AssemblyPage() },
                    { "Юрист", new Pages.LawyerPage() }
                };

            if (navigationMap.TryGetValue(positionKind, out var page))
            {
                _ = MainFrame.Navigate(page);
            }
            else
            {
            }
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
            PCPassportTextBox.Text = Procurement.PassportOfPC;
            MonoblockPassportTextBox.Text = Procurement.PassportOfMonoblock;
            NotebookPassportTextBox.Text = Procurement.PassportOfNotebook;
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
            Procurement.PassportOfPC = PCPassportTextBox.Text;
            Procurement.PassportOfMonoblock = MonoblockPassportTextBox.Text;
            Procurement.PassportOfNotebook = NotebookPassportTextBox.Text;
            PULL.Procurement(Procurement);
        }

        private void SavePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            if (Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id || Procurement.PurchaseUserId == null)
                UpdateComponentCalculationListView(SameDate, SameComponentState);
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
                        UpdateComponentCalculationListView(null, null);
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
    }
}
