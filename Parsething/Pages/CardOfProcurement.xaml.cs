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

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для CardOfProcurement.xaml
    /// </summary>
    public partial class CardOfProcurement : Page
    {
        private Frame MainFrame { get; set; } = null!;
        private List<ProcurementState> ProcurementStates { get; set; }
        private Procurement? Procurement { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        public CardOfProcurement(ProcurementsEmployee procurementsEmployee)
        {
            InitializeComponent();
            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;
            Procurement = procurementsEmployee.Procurement;

            if (Procurement != null && ProcurementState != null)
            {
                Id.Text = Procurement.Id.ToString();
                foreach (ProcurementState procurementState in ProcurementState.ItemsSource)
                    if (procurementState.Id == Procurement.ProcurementStateId)
                    {
                        ProcurementState.SelectedItem = procurementState;
                        break;
                    }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement.Id = Convert.ToInt32(Id.Text);
            Procurement.ProcurementStateId = ((ProcurementState)ProcurementState.SelectedItem).Id;
            PULL.Procurement(Procurement);
            _ = MainFrame.Navigate(new CalculatorPage());
        }
    }
}
