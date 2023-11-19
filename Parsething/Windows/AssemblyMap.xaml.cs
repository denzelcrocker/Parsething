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
using System.Windows.Shapes;

namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для AssemblyMap.xaml
    /// </summary>
    public partial class AssemblyMap : Window
    {
        private List<ComponentCalculation>? ComponentCalculations { get; set; }

        public AssemblyMap(Procurement procurement)
        {
            InitializeComponent();

            ProcurementId.Text = procurement.Id.ToString();

            ComponentCalculations = GET.View.ComponentCalculationsBy(procurement.Id);
            ComponentCalculationsDataGrid.ItemsSource = ComponentCalculations;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(PrintGrid, "Лист комплектаций");
            }
        }
    }
}
