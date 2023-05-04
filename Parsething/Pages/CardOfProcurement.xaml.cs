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
        private List<Region> ProcurementRegions { get; set; }


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
            ProcurementStates = GET.View.DistributionOfProcurementStates(((Employee)Application.Current.MainWindow.DataContext).Position.Kind);
            ProcurementState.ItemsSource = ProcurementStates;
            ProcurementRegions = GET.View.Regions();
            Regions.ItemsSource = ProcurementRegions;
            Procurement = procurement;
            if (Procurement != null && ProcurementState != null)
            {
                foreach (ProcurementState procurementState in ProcurementState.ItemsSource)
                    if (procurementState.Id == Procurement.ProcurementStateId)
                    {
                        ProcurementState.SelectedItem = procurementState;
                        break;
                    }
                Id.Text = Procurement.Id.ToString();
                if(Procurement.Number != null)
                Number.Text = Procurement.Number.ToString();
                if (Procurement.Method != null)
                    Method.Text = Procurement.Method.Text.ToString();
                if (Procurement.Law != null)
                    Law.Text = Procurement.Law.Number.ToString();
                if (Procurement.RequestUri != null)
                    URL.Text = Procurement.RequestUri.ToString();
                if (Procurement.Platform != null)
                {
                    Platform.Text = Procurement.Platform.Name.ToString();
                    PlatformURL.Text = Procurement.Platform.Address.ToString();
                }
                if (Procurement.StartDate != null && Procurement.TimeZone != null)
                    StartDate.Text = $"{Procurement.StartDate} ({Procurement.TimeZone.Offset})";
                if (Procurement.Deadline != null && Procurement.TimeZone != null)
                    DeadLine.Text = $"{Procurement.Deadline} ({Procurement.TimeZone.Offset})";
                InitialPrice.Text = Procurement.InitialPrice.ToString();
                if (Procurement.Organization.Name != null)
                    Organization.Text = Procurement.Organization.Name.ToString();
                if (Procurement.Organization.PostalAddress != null)
                    PostalAddress.Text = Procurement.Organization.PostalAddress.ToString();
                if (Procurement.Object != null)
                    Object.Text = Procurement.Object.ToString();
                if (Procurement.Securing != null)
                    Securing.Text = Procurement.Securing.ToString();
                if (Procurement.Enforcement != null)
                    Enforcement.Text = Procurement.Enforcement.ToString();
                if (Procurement.Warranty != null)
                    Warranty.Text = Procurement.Warranty.ToString();
                if(Procurement.Location != null)
                    Location.Text = Procurement.Location.ToString();
                foreach (Region region in Regions.ItemsSource)
                    if (region.Id == Procurement.RegionId)
                    {
                        Regions.SelectedItem = region;
                        Distance.Text = region.Distance.ToString();
                        break;
                    }
                OrganizationContract.Text = Procurement.OrganizationContractName;
                PostalAddressContract.Text = Procurement.OrganizationContractPostalAddress;
                ContactPerson.Text = Procurement.ContactPerson;
                ContactPhone.Text = Procurement.ContactPhone;
                DeliveryDetails.Text = Procurement.DeliveryDetails;
            }
        }

        private void Method_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            MethodToolTip.Text = Method.Text;
        }
        private void URL_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            URLToolTip.Text = URL.Text;
        }
        private void Organization_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            OrganizationToolTip.Text = Organization.Text;
        }
        private void PostalAddress_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            PostalAddressToolTip.Text = PostalAddress.Text;
        }
        private void Object_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            ObjectToolTip.Text = Object.Text;
        }
        private void Location_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            LocationToolTip.Text = Location.Text;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isRegionExists = false;
            List<Region> ProcurementRegion = new List<Region> { new Region() };
            Procurement.Id = Convert.ToInt32(Id.Text);
            if (ProcurementState.SelectedItem != null)
                Procurement.ProcurementStateId = ((ProcurementState)ProcurementState.SelectedItem).Id;
            if (Distance.Text != "" && Regions.Text != "")
            {
                ProcurementRegion[0].Title = Regions.Text;
                ProcurementRegion[0].Distance = Convert.ToInt32(Distance.Text);
                foreach (Region region in ProcurementRegions)
                {
                    if (ProcurementRegion[0].Title == region.Title && ProcurementRegion[0].Distance == region.Distance)
                    {
                        Procurement.RegionId = GET.Entry.Region(region.Title, region.Distance).Id;
                        isRegionExists = true;
                    }
                }
                if (isRegionExists == false)
                {
                    PUT.Region(ProcurementRegion[0]);
                    Procurement.RegionId = GET.Entry.Region(Regions.Text, Convert.ToInt32(Distance.Text)).Id; ;
                }
            }
            Procurement.OrganizationContractName = OrganizationContract.Text;
            Procurement.OrganizationContractPostalAddress = PostalAddressContract.Text;
            Procurement.ContactPerson = ContactPerson.Text;
            Procurement.ContactPhone = ContactPhone.Text;
            Procurement.DeliveryDetails = DeliveryDetails.Text;
            PULL.Procurement(Procurement);

            if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Администратор")
            {
                _ = MainFrame.Navigate(new Pages.AdministratorPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела расчетов" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела расчетов")
            {
                _ = MainFrame.Navigate(new Pages.HeadsOfCalculatorsPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист отдела расчетов")
            {
                _ = MainFrame.Navigate(new Pages.CalculatorPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель тендерного отдела" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя тендреного отдела")
            {
                _ = MainFrame.Navigate(new Pages.HeadsOfManagersPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист по работе с электронными площадками")
            {
                _ = MainFrame.Navigate(new Pages.EPlatformSpecialistPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист тендерного отдела")
            {
                _ = MainFrame.Navigate(new Pages.ManagerPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела закупки" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела закупок" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист закупки")
            {
                _ = MainFrame.Navigate(new Pages.PurchaserPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела производства" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела производства" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист по производству")
            {
                _ = MainFrame.Navigate(new Pages.AssemblyPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Юрист")
            {
                _ = MainFrame.Navigate(new Pages.LawyerPage());
            }
            else
            {

            }
        }
        private void Regions_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (Region region in Regions.ItemsSource)
                if (region.Title == Regions.Text)
                {
                    Distance.Text = region.Distance.ToString();
                    break;
                }
        }
        private void Regions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void Regions_DropDownClosed(object sender, EventArgs e)
        {
            foreach (Region region in Regions.ItemsSource)
                if (region.Title == Regions.Text)
                {
                    Distance.Text = region.Distance.ToString();
                    break;
                }
        }
        private void History_Click(object sender, RoutedEventArgs e)
        {

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
