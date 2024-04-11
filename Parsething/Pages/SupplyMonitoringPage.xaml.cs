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
        public List<GET.SupplyMonitoringList> supplyMonitoringListCommon { get; set; }
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
            supplyMonitoringListCommon = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
            listViewSupplyMonitoring.ItemsSource = supplyMonitoringListCommon;
            //var componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
            //supplyMonitoringListCommon = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
            //supplyMonitoringListCommon.Add(new GET.SupplyMonitoringList {SupplierName = "Тестовый заголовок"});
            //SupplyMonitoringListView.ItemsSource = supplyMonitoringListCommon;
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
            var componentStatuses = new List<string> { "Склад", "Резерв"};
            supplyMonitoringListWarehouseAndReserve = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
        }

        private void OnTheWayButton_Click(object sender, RoutedEventArgs e)
        {
            var componentStatuses = new List<string> { "В пути"};
            supplyMonitoringListOnTheWay = GET.View.GetSupplyMonitoringLists(procurementsList, componentStatuses);
        }
        private void SupplyMonitoringListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
