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

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SupplyMonitoringPage.xaml
    /// </summary>
    public partial class SupplyMonitoringPage : Page
    {
        private List<Procurement> procurementsList { get; set; }
        public List<SupplyMonitoringList> supplyMonitoringList { get; set; }


        public SupplyMonitoringPage(List<Procurement> procurements)
        {
            InitializeComponent();
            procurementsList = procurements;

        }

        private void CommonListButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };

            supplyMonitoringList = View.GetSupplyMonitoringLists(procurementsList, componentStatuses).OrderBy(x => x.ComponentName).ToList();
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
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });

                        TextBlock textBlockManufacturerName = new TextBlock() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentName = new TextBlock() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentStatus = new TextBlock() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockAveragePrice = new TextBlock() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalCount = new TextBlock() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockSellerName = new TextBlock() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalAmount = new TextBlock() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
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
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            listViewSupplyMonitoring.ItemsSource = stackPanels;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = Entry.ProcurementBy(Convert.ToInt32(((Button)sender).DataContext));
            NavigationService.Navigate(new ComponentCalculationsPage(procurement, null, false, false));
        }

        private void BySuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };

            supplyMonitoringList = View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
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
                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                });
    
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
                            border.BorderBrush = (Brush)Application.Current.FindResource("Text.Foreground");
                            Grid.SetColumn(border, i);
                            grid.Children.Add(border);
                        }
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });
    
                        TextBlock textBlockManufacturerName = new TextBlock() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentName = new TextBlock() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentStatus = new TextBlock() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockAveragePrice = new TextBlock() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalCount = new TextBlock() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockSellerName = new TextBlock() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalAmount = new TextBlock() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
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
                    }
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }
    
            listViewSupplyMonitoring.ItemsSource = stackPanels;
         }
        

        private void WarehouseAndReserveButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "На складе", "В резерве"};

            supplyMonitoringList = View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
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
                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                });

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
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });

                        TextBlock textBlockManufacturerName = new TextBlock() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentName = new TextBlock() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentStatus = new TextBlock() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockAveragePrice = new TextBlock() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalCount = new TextBlock() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockSellerName = new TextBlock() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalAmount = new TextBlock() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
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
                    }
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
        }

        private void OnTheWayButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> {"В пути"};

            supplyMonitoringList = View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
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
                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                });

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
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });

                        TextBlock textBlockManufacturerName = new TextBlock() { Text = supplyMonitoring.ManufacturerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentName = new TextBlock() { Text = supplyMonitoring.ComponentName, TextWrapping = TextWrapping.Wrap, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockComponentStatus = new TextBlock() { Text = supplyMonitoring.ComponentStatus, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockAveragePrice = new TextBlock() { Text = $"{supplyMonitoring.AveragePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalCount = new TextBlock() { Text = supplyMonitoring.TotalCount.ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockSellerName = new TextBlock() { Text = supplyMonitoring.SellerName, Style = (Style)Application.Current.FindResource("TableElements") };
                        TextBlock textBlockTotalAmount = new TextBlock() { Text = $"{supplyMonitoring.TotalAmount:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("TableElements") };
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
                    }
                }
                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExportToExcel.ExportSupplyMonitoringListToExcel(supplyMonitoringList);
        }
    }
}
