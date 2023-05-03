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
        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));
        SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(0x53, 0x53, 0x53));


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        public CardOfProcurement(Procurement procurement)
        {
            InitializeComponent();
            ProcurementStates = GET.View.DistributionOfProcurementStates(((Employee)Application.Current.MainWindow.DataContext).Position.Kind);
            ProcurementState.ItemsSource = ProcurementStates;
            Procurement = procurement;
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
            MainFrame.GoBack();
        }

        private void ProcurementInfo_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Red;
            ProcurementInfoUL.Fill = Red;
            ProcurementInfoLV.Visibility = Visibility.Visible;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void ContractInfo_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Red;
            ContractInfoUL.Fill = Red;
            ContractInfoLV.Visibility = Visibility.Visible;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void ContractNuances_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Red;
            ContractNuancesUL.Fill = Red;
            ContractNuancesLV.Visibility = Visibility.Visible;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Red;
            CalculatingUL.Fill = Red;
            CalculatingLV.Visibility = Visibility.Visible;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void Sending_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Red;
            SendingUL.Fill = Red;
            SendingLV.Visibility = Visibility.Visible;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void Bargaining_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Red;
            BargainingUL.Fill = Red;
            BargainingLV.Visibility = Visibility.Visible;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void Supply_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Red;
            SupplyUL.Fill = Red;
            SupplyLV.Visibility = Visibility.Visible;
            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;
        }

        private void Payment_Click(object sender, RoutedEventArgs e)
        {
            ProcurementInfoLabel.Foreground = Gray;
            ProcurementInfoUL.Fill = Gray;
            ProcurementInfoLV.Visibility = Visibility.Hidden;
            ContractInfoLabel.Foreground = Gray;
            ContractInfoUL.Fill = Gray;
            ContractInfoLV.Visibility = Visibility.Hidden;
            ContractNuancesLabel.Foreground = Gray;
            ContractNuancesUL.Fill = Gray;
            ContractNuancesLV.Visibility = Visibility.Hidden;
            CalculatingLabel.Foreground = Gray;
            CalculatingUL.Fill = Gray;
            CalculatingLV.Visibility = Visibility.Hidden;
            SendingLabel.Foreground = Gray;
            SendingUL.Fill = Gray;
            SendingLV.Visibility = Visibility.Hidden;
            BargainingLabel.Foreground = Gray;
            BargainingUL.Fill = Gray;
            BargainingLV.Visibility = Visibility.Hidden;
            SupplyLabel.Foreground = Gray;
            SupplyUL.Fill = Gray;
            SupplyLV.Visibility = Visibility.Hidden;
            PaymentLabel.Foreground = Red;
            PaymentUL.Fill = Red;
            PaymentLV.Visibility = Visibility.Visible;
        }
    }
}
