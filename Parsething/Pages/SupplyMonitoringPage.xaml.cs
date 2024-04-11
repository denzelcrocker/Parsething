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

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SupplyMonitoringPage.xaml
    /// </summary>
    public partial class SupplyMonitoringPage : Page
    {
        private List<Procurement> procurementsList { get; set; }
        public List<SupplyMonitoringList> supplyMonitoringListCommon { get; set; }
        private List<GET.SupplyMonitoringList> SupplyMonitoringListBySuppliers { get; set; }
        private List<GET.SupplyMonitoringList> supplyMonitoringListWarehouseAndReserve { get; set; }
        private List<GET.SupplyMonitoringList> supplyMonitoringListOnTheWay { get; set; }


        public SupplyMonitoringPage(List<Procurement> procurements)
        {
            InitializeComponent();
            procurementsList = procurements;

        }

        private void CommonListButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };

            supplyMonitoringListCommon = new List<SupplyMonitoringList>
            {
                new SupplyMonitoringList
                    {
                         SupplierName = "Тест1",
                         ManufacturerName = "Тест",
                         ComponentName = "Тест",
                         ComponentStatus = "Тест",
                         TenderNumber = 5216
                     },
                new SupplyMonitoringList
                    {
                         SupplierName = "Тест1",
                         ManufacturerName = "Тест",
                         ComponentName = "Тест",
                         ComponentStatus = "Тест",
                         TenderNumber = 5215
                     },
                new SupplyMonitoringList
                    {
                         SupplierName = "Тест2",
                         ManufacturerName = "Тест",
                         ComponentName = "Тест",
                         ComponentStatus = "Тест",
                         TenderNumber = 5217
                     },
            };

            List<string> supchegi = new();
            List<StackPanel> stackPanels = new();
            foreach (SupplyMonitoringList supcheg in supplyMonitoringListCommon)
            {
                if (!supchegi.Contains(supcheg.SupplierName))
                {
                    supchegi.Add(supcheg.SupplierName);
                }
            }

            foreach (string supcheg in supchegi)
            {
                StackPanel brotherImStuck = new();
                brotherImStuck.Orientation = Orientation.Horizontal;
                brotherImStuck.Children.Add(new TextBlock()
                {
                    Text = supcheg,
                    Style = (Style)Application.Current.FindResource("TableElements")
                });

                

                ListView list = new();
                list.Style = (Style)Application.Current.FindResource("ListView");
                foreach (SupplyMonitoringList brother in supplyMonitoringListCommon)
                {
                    if (brother.SupplierName == supcheg)
                    {
                        TextBlock textBlock = new TextBlock()
                        {
                            Text = $"{brother.ManufacturerName}\t{brother.ComponentName}\t{brother.ComponentStatus}"
                        };
                        if (brother.ComponentStatus == "Купить")
                        {
                            textBlock.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        list.Items.Add(textBlock);

                        Button button = new Button()
                        {
                            Content = brother.TenderNumber
                        };
                        list.Items.Add(button);
                        button.Click += Button_Click;
                        button.DataContext = brother.TenderNumber;
                    }
                }
                brotherImStuck.Children.Add(list);
                stackPanels.Add(brotherImStuck);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
            //var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
            //supplyMonitoringListCommon = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
            //supplyMonitoringListCommon.Add(new GET.SupplyMonitoringList {SupplierName = "Тестовый заголовок"});
            //SupplyMonitoringListView.ItemsSource = supplyMonitoringListCommon;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //((ListView)((StackPanel)((Button)sender).Parent).Children[2]).Items.Add("ЯЕБУСОБАК");
            //NavigationService.Navigate(new ComponentCalculationsPage( Convert.ToInt32(((Button)sender).DataContext));
        }

        public class SupplyMonitoringList
        {
            public string? SupplierName { get; set; }

            public string? ManufacturerName { get; set; }

            public string? ComponentName { get; set; }

            public string? ComponentStatus { get; set; }

            public decimal? AveragePrice { get; set; }

            public int? TotalCount { get; set; }

            public string? SellerName { get; set; }

            public int? TenderNumber { get; set; }

            public decimal? TotalAmount { get; set; }
        }

        private void BySuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            //var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
            //SupplyMonitoringListBySuppliers = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
            //var groupedData = new CollectionViewSource { Source = SupplyMonitoringListBySuppliers };
            //groupedData.GroupDescriptions.Add(new PropertyGroupDescription("SupplierName"));
            //supplyMonitoringListView.ItemsSource = groupedData.View;

        }

        private void WarehouseAndReserveButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "Склад", "Резерв" };
            supplyMonitoringListWarehouseAndReserve = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
        }

        private void OnTheWayButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "В пути" };
            supplyMonitoringListOnTheWay = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
        }
        private void SupplyMonitoringListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
