using DatabaseLibrary.Entities.ProcurementProperties;
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
using DatabaseLibrary.Queries;
using static DatabaseLibrary.Queries.GET;
using System.Windows.Controls.Primitives;
using Parsething.Classes;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SupplyMonitoringPage.xaml
    /// </summary>
    public partial class SupplyMonitoringPage : Page
    {
        private List<SupplyMonitoringList> supplyMonitoringList { get; set; }
        private List<ComponentState> componentStates { get; set; }
        private List<Seller> sellers { get; set; }
        private decimal? overAllPrice = 0;
        private Frame MainFrame { get; set; } = null!;


        public SupplyMonitoringPage()
        {
            InitializeComponent();
            componentStates = GET.View.ComponentStates();
            sellers = GET.View.Sellers();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void CommonListButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
            overAllPrice = 0;

            supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses).OrderBy(x => x.ComponentName).ToList();
            List<StackPanel> stackPanels = new();
            StackPanel stackPanel = new();
            
            ListView list = new();
            list.Style = (Style)Application.Current.FindResource("ListView");
            foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
            {
                Grid grid = new Grid();
                double[] columnWidths = { 150, 830, 100, 150, 100, 160, 160 };

                for (int i = 0; i < columnWidths.Length; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new GridLength(columnWidths[i]);
                    grid.ColumnDefinitions.Add(columnDefinition);
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    Grid.SetColumn(border, i);
                    grid.Children.Add(border);
                }
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });

                TextBox textBlockManufacturerName = new TextBox() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockComponentName = new TextBox() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockComponentStatus = new TextBox() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockAveragePrice = new TextBox() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockTotalCount = new TextBox() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockSellerName = new TextBox() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockTotalAmount = new TextBox() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                Button button = new Button() { Content = supplyMonitoring.TenderNumber, Style = (Style)Application.Current.FindResource("GoToAddEditComponents") };
                button.Click += Button_Click;
                button.DataContext = supplyMonitoring.TenderNumber;

                Grid.SetColumn(textBlockManufacturerName, 0);
                Grid.SetColumn(textBlockComponentName, 1);
                Grid.SetColumn(textBlockComponentStatus, 2);
                Grid.SetColumn(textBlockAveragePrice, 3);
                Grid.SetColumn(textBlockTotalCount, 4);
                Grid.SetColumn(textBlockSellerName, 5);
                Grid.SetColumn(textBlockTotalAmount, 6);
                Grid.SetColumn(button, 7);

                grid.Children.Add(textBlockManufacturerName);
                grid.Children.Add(textBlockComponentName);
                grid.Children.Add(textBlockComponentStatus);
                grid.Children.Add(textBlockAveragePrice);
                grid.Children.Add(textBlockTotalCount);
                grid.Children.Add(textBlockSellerName);
                grid.Children.Add(textBlockTotalAmount);
                grid.Children.Add(button);

                if (supplyMonitoring.ComponentStatus == "Купить")
                {
                    textBlockComponentStatus.Foreground = new SolidColorBrush(Colors.Red);
                }
                list.Items.Add(grid);
                overAllPrice += supplyMonitoring.TotalAmount;
            }
            stackPanel.Children.Add(list);
            stackPanels.Add(stackPanel);
            listViewSupplyMonitoring.ItemsSource = stackPanels;
            OverAllInfoTextBlock.Text = overAllPrice.ToString() + " p.";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = Entry.ProcurementBy(Convert.ToInt32(((Button)sender).DataContext));
            NavigationService.Navigate(new ComponentCalculationsPage(procurement, false, false));
        }

        private void BySuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
            overAllPrice = 0;

            supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
            List<string> headers = new();
            List<StackPanel> stackPanels = new();
            foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
            {
                if (!headers.Contains(supplyMonitoring.SupplierName))
                {
                    headers.Add(supplyMonitoring.SupplierName);
                }
            }

            foreach (string header in headers)
            {
                StackPanel stackPanel = new();
                DockPanel dockPanel = new();

                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }
                stackPanel.Children.Add(dockPanel);
                dockPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                    Width = 1710
                });

                Button saveButton = new Button()
                {
                    Content = "↓",
                    Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                    DataContext = header
                };
                saveButton.Click += (s, args) =>
                {
                    string header = (string)((Button)s).DataContext;
                    Popup popup = CreatePopup(saveButton, supplyMonitoringList, header);
                    popup.IsOpen = true;
                };
                dockPanel.Children.Add(saveButton);
                Button сopyToClipBoardButton = new Button()
                {
                    Content = "Copy",
                    Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                };
                сopyToClipBoardButton.Click += (sender, e) =>
                {
                    CopyToClipBoardButton_Click(sender, e, header, totalAmount);
                };
                dockPanel.Children.Add(сopyToClipBoardButton);

                ListView list = new ListView() { Style = (Style)Application.Current.FindResource("ListView") };
                foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
                {
                    if (supplyMonitoring.SupplierName == header)
                    {
                        Grid grid = new Grid();
                        double[] columnWidths = { 150, 830, 100, 150, 100, 160, 170 };

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                            Border border = new Border();
                            border.BorderThickness = new Thickness(1);
                            border.BorderBrush = (Brush)Application.Current.FindResource("Text.Foreground");
                            Grid.SetColumn(border, i);
                            grid.Children.Add(border);
                        }
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });

                        TextBox textBlockManufacturerName = new TextBox() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockComponentName = new TextBox() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockComponentStatus = new TextBox() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockAveragePrice = new TextBox() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockTotalCount = new TextBox() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockSellerName = new TextBox() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockTotalAmount = new TextBox() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        Button button = new Button() { Content = GET.Aggregate.DisplayId(supplyMonitoring.TenderNumber).ToString(), Style = (Style)Application.Current.FindResource("GoToAddEditComponents") };
                        button.Click += Button_Click;
                        button.DataContext = supplyMonitoring.TenderNumber;

                        Grid.SetColumn(textBlockManufacturerName, 0);
                        Grid.SetColumn(textBlockComponentName, 1);
                        Grid.SetColumn(textBlockComponentStatus, 2);
                        Grid.SetColumn(textBlockAveragePrice, 3);
                        Grid.SetColumn(textBlockTotalCount, 4);
                        Grid.SetColumn(textBlockSellerName, 5);
                        Grid.SetColumn(textBlockTotalAmount, 6);
                        Grid.SetColumn(button, 7);

                        grid.Children.Add(textBlockManufacturerName);
                        grid.Children.Add(textBlockComponentName);
                        grid.Children.Add(textBlockComponentStatus);
                        grid.Children.Add(textBlockAveragePrice);
                        grid.Children.Add(textBlockTotalCount);
                        grid.Children.Add(textBlockSellerName);
                        grid.Children.Add(textBlockTotalAmount);
                        grid.Children.Add(button);

                        if (supplyMonitoring.ComponentStatus == "Купить")
                        {
                            textBlockComponentStatus.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        list.Items.Add(grid);
                        overAllPrice += supplyMonitoring.TotalAmount;
                    }
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
            OverAllInfoTextBlock.Text = overAllPrice.ToString() + " р.";
        }


        private Popup CreatePopup(Button targetButton, List<SupplyMonitoringList> supplyMonitoringList, string header)
        {
            var datePicker = new DatePicker
            {
                Name = "DatePicker",
                Style = (Style)Application.Current.FindResource("ComponentCalculations.DatePickerStyle"),
                Width = 150,
                Height = 40
            };

            var comboBox = new ComboBox
            {
                Name = "ComboBox",
                DisplayMemberPath = "Kind",
                Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem"),
                ItemsSource = componentStates,
                Width = 150,
                Height = 35
            };

            var saveButton = new Button
            {
                Content = "Save",
                Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                Width = 150,
                Height = 35,
                Tag = header,
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel
            {
                Children =
                {
                    datePicker,
                    comboBox,
                    saveButton
                }
            };

            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Child = stackPanel
            };

            var popup = new Popup
            {
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                Placement = PlacementMode.Bottom,
                StaysOpen = false,
                PlacementTarget = targetButton,
                Child = border
            };

            saveButton.Tag = popup;
            saveButton.Click += (s, args) => SaveButton_Click(s, args, supplyMonitoringList, datePicker, comboBox);

            return popup;
        }

        private void WarehouseAndReserveButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "На складе", "В резерве"};
            overAllPrice = 0;

            supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
            List<string> headers = new();
            List<StackPanel> stackPanels = new();
            foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
            {
                if (!headers.Contains(supplyMonitoring.SupplierName))
                {
                    headers.Add(supplyMonitoring.SupplierName);
                }
            }

            foreach (string header in headers)
            {
                DockPanel dockPanel = new();
                StackPanel stackPanel = new();
                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }
                stackPanel.Children.Add(dockPanel);
                dockPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                    Width = 1710
                });

                Button saveButton = new Button()
                {
                    Content = "↓",
                    Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                    DataContext = header
                };
                saveButton.Click += (s, args) =>
                {
                    string header = (string)((Button)s).DataContext;
                    Popup popup = CreatePopup(saveButton, supplyMonitoringList, header);
                    popup.IsOpen = true;
                };
                dockPanel.Children.Add(saveButton);
                Button сopyToClipBoardButton = new Button()
                {
                    Content = "Copy",
                    Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                };
                сopyToClipBoardButton.Click += (sender, e) =>
                {
                    CopyToClipBoardButton_Click(sender, e, header, totalAmount);
                };
                dockPanel.Children.Add(сopyToClipBoardButton);

                ListView list = new();
                list.Style = (Style)Application.Current.FindResource("ListView");
                foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
                {
                    if (supplyMonitoring.SupplierName == header)
                    {
                        Grid grid = new Grid();
                        double[] columnWidths = { 150, 830, 100, 150, 100, 160, 160 };

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                            Border border = new Border();
                            border.BorderThickness = new Thickness(1);
                            border.BorderBrush = Brushes.Black;
                            Grid.SetColumn(border, i);
                            grid.Children.Add(border);
                        }
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });

                        TextBox textBlockManufacturerName = new TextBox() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockComponentName = new TextBox() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockComponentStatus = new TextBox() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockAveragePrice = new TextBox() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockTotalCount = new TextBox() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockSellerName = new TextBox() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockTotalAmount = new TextBox() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        Button button = new Button() { Content = supplyMonitoring.TenderNumber, Style = (Style)Application.Current.FindResource("GoToAddEditComponents") };
                        button.Click += Button_Click;
                        button.DataContext = supplyMonitoring.TenderNumber;

                        Grid.SetColumn(textBlockManufacturerName, 0);
                        Grid.SetColumn(textBlockComponentName, 1);
                        Grid.SetColumn(textBlockComponentStatus, 2);
                        Grid.SetColumn(textBlockAveragePrice, 3);
                        Grid.SetColumn(textBlockTotalCount, 4);
                        Grid.SetColumn(textBlockSellerName, 5);
                        Grid.SetColumn(textBlockTotalAmount, 6);
                        Grid.SetColumn(button, 7);

                        grid.Children.Add(textBlockManufacturerName);
                        grid.Children.Add(textBlockComponentName);
                        grid.Children.Add(textBlockComponentStatus);
                        grid.Children.Add(textBlockAveragePrice);
                        grid.Children.Add(textBlockTotalCount);
                        grid.Children.Add(textBlockSellerName);
                        grid.Children.Add(textBlockTotalAmount);
                        grid.Children.Add(button);

                        if (supplyMonitoring.ComponentStatus == "Купить")
                        {
                            textBlockComponentStatus.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        list.Items.Add(grid);
                        overAllPrice += supplyMonitoring.TotalAmount;
                    }
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
            OverAllInfoTextBlock.Text = overAllPrice.ToString() + " р.";
        }

        private void OnTheWayButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> {"В пути"};
            overAllPrice = 0;

            supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
            List<string> headers = new();
            List<StackPanel> stackPanels = new();
            foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
            {
                if (!headers.Contains(supplyMonitoring.SupplierName))
                {
                    headers.Add(supplyMonitoring.SupplierName);
                }
            }

            foreach (string header in headers)
            {
                DockPanel dockPanel = new();
                StackPanel stackPanel = new();
                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }
                stackPanel.Children.Add(dockPanel);
                dockPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                    Width = 1710
                });

                Button saveButton = new Button()
                {
                    Content = "↓",
                    Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                    DataContext = header
                };
                saveButton.Click += (s, args) =>
                {
                    string header = (string)((Button)s).DataContext;
                    Popup popup = CreatePopup(saveButton, supplyMonitoringList, header);
                    popup.IsOpen = true;
                };
                dockPanel.Children.Add(saveButton);
                Button сopyToClipBoardButton = new Button()
                {
                    Content = "Copy",
                    Style = (Style)Application.Current.FindResource("SaveSupplyMonitoringButton"),
                };
                сopyToClipBoardButton.Click += (sender, e) =>
                {
                    CopyToClipBoardButton_Click(sender, e, header, totalAmount);
                };
                dockPanel.Children.Add(сopyToClipBoardButton);

                ListView list = new();
                list.Style = (Style)Application.Current.FindResource("ListView");
                foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
                {
                    if (supplyMonitoring.SupplierName == header)
                    {
                        Grid grid = new Grid();
                        double[] columnWidths = { 150, 830, 100, 150, 100, 160, 160 };

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                            Border border = new Border();
                            border.BorderThickness = new Thickness(1);
                            border.BorderBrush = Brushes.Black;
                            Grid.SetColumn(border, i);
                            grid.Children.Add(border);
                        }
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });

                        TextBox textBlockManufacturerName = new TextBox() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockComponentName = new TextBox() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockComponentStatus = new TextBox() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockAveragePrice = new TextBox() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockTotalCount = new TextBox() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockSellerName = new TextBox() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        TextBox textBlockTotalAmount = new TextBox() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                        Button button = new Button() { Content = supplyMonitoring.TenderNumber, Style = (Style)Application.Current.FindResource("GoToAddEditComponents") };
                        button.Click += Button_Click;
                        button.DataContext = supplyMonitoring.TenderNumber;

                        Grid.SetColumn(textBlockManufacturerName, 0);
                        Grid.SetColumn(textBlockComponentName, 1);
                        Grid.SetColumn(textBlockComponentStatus, 2);
                        Grid.SetColumn(textBlockAveragePrice, 3);
                        Grid.SetColumn(textBlockTotalCount, 4);
                        Grid.SetColumn(textBlockSellerName, 5);
                        Grid.SetColumn(textBlockTotalAmount, 6);
                        Grid.SetColumn(button, 7);

                        grid.Children.Add(textBlockManufacturerName);
                        grid.Children.Add(textBlockComponentName);
                        grid.Children.Add(textBlockComponentStatus);
                        grid.Children.Add(textBlockAveragePrice);
                        grid.Children.Add(textBlockTotalCount);
                        grid.Children.Add(textBlockSellerName);
                        grid.Children.Add(textBlockTotalAmount);
                        grid.Children.Add(button);

                        if (supplyMonitoring.ComponentStatus == "Купить")
                        {
                            textBlockComponentStatus.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        list.Items.Add(grid);
                        overAllPrice += supplyMonitoring.TotalAmount;
                    }
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
            OverAllInfoTextBlock.Text = overAllPrice.ToString() + " р.";
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e, List<SupplyMonitoringList> supplyMonitoringList, DatePicker datePicker, ComboBox comboBox)
        {
            if (supplyMonitoringList == null) return;

            var procurementIds = supplyMonitoringList.Select(s => s.TenderNumber).Distinct().ToList();
            string header = (string)((Button)sender).DataContext;

            var componentStatuses = supplyMonitoringList.Select(s => s.ComponentStatus).Distinct().ToList();

            var componentCalculations = GET.View.ComponentCalculationsBy(procurementIds, componentStatuses);

            DateTime? selectedDate = datePicker.SelectedDate;
            string selectedStatus = ((comboBox.SelectedItem as ComponentState)?.Kind) ?? string.Empty;

            var sellerId = sellers.FirstOrDefault(s => s.Name == header)?.Id;

            var filteredComponentCalculations = componentCalculations.Where(cc => cc.SellerIdPurchase == sellerId) .ToList();

            UpdateComponentCalculations(filteredComponentCalculations, selectedDate, selectedStatus);

            if (sender is Button button && button.Tag is Popup popup)
            {
                popup.IsOpen = false;
            }
        }
        private void UpdateComponentCalculations(List<ComponentCalculation> componentCalculations, DateTime? selectedDate, string selectedStatus)
        {
            foreach (var calculation in componentCalculations)
            {
                if (selectedDate.HasValue)
                {
                    calculation.Date = selectedDate.Value;
                }

                if (!string.IsNullOrEmpty(selectedStatus))
                {
                    using ParsethingContext db = new();
                    var state = db.ComponentStates.FirstOrDefault(s => s.Kind == selectedStatus);
                    if (state != null)
                    {
                        calculation.ComponentStateId = state.Id;
                    }
                }

                PULL.ComponentCalculation(calculation);
            }
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExportToExcel.ExportSupplyMonitoringListToExcel(supplyMonitoringList);
        }

        private void CopyToClipBoardButton_Click(object sender, RoutedEventArgs e, string header, decimal? totalAmount)
        {
            if (supplyMonitoringList == null) return;

            var recordsForHeader = supplyMonitoringList.Where(s => s.SupplierName == header).ToList();

            StringBuilder clipboardText = new StringBuilder();

            clipboardText.AppendLine($"{header} - {totalAmount}");
            foreach (var record in recordsForHeader)
            {
                clipboardText.AppendLine($"{record.ComponentName}        {record.AveragePrice}      {record.TotalCount}");
            }

            Clipboard.SetText(clipboardText.ToString());
        }

        private void OverallInfoButton_Click(object sender, RoutedEventArgs e)
        {
            OverAllInfoPopUp.IsOpen = !OverAllInfoPopUp.IsOpen;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }
    }
}
