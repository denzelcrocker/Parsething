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

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для CardOfProcurement.xaml
    /// </summary>
    public partial class CardOfProcurement : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<ProcurementState>? ProcurementStates { get; set; }
        private List<RepresentativeType>? RepresentativeTypes { get; set; }
        private List<CommisioningWork>? CommissioningWorks { get; set; }

        private List<Region>? ProcurementRegions { get; set; }

        private List<Employee>? Calculators { get; set; }
        private ProcurementsEmployee? ProcurementsEmployeeCalculators = new ProcurementsEmployee();

        private List<Minopttorg>? Minopttorgs { get; set; }

        private List<ProcurementsPreference>? ProcurementPreferencesSelected { get; set; }
        private List<Preference> PreferencesSelected = new List<Preference>();
        private List<Preference>? PreferencesNotSelected { get; set; }

        private List<LegalEntity>? LegalEntities { get; set; }

        private List<Employee>? Senders { get; set; }
        private ProcurementsEmployee? ProcurementsEmployeeSenders = new ProcurementsEmployee();

        private List<Employee>? Managers { get; set; }
        private ProcurementsEmployee? ProcurementsEmployeeManagers = new ProcurementsEmployee();

        private List<Employee>? Purchasers { get; set; }
        private ProcurementsEmployee? ProcurementsEmployeePurchasers = new ProcurementsEmployee();

        private List<ShipmentPlan>? ShipmentPlans { get; set; }

        private List<ExecutionState>? ExecutionStates { get; set; }

        private List<WarrantyState>? WarrantyStates { get; set; }

        private List<ProcurementsDocument>? ProcurementDocumentsSelected { get; set; }
        private List<Document> DocumentsSelected = new List<Document>();
        private List<Document>? DocumentsNotSelected { get; set; }

        private List<SignedOriginal>? SignedOriginals { get; set; }

        private List<Employee>? Lawyers { get; set; }
        private ProcurementsEmployee? ProcurementsEmployeeLawyers = new ProcurementsEmployee();

        private List<Comment>? Comments = new List<Comment>();

        private List<History>? Histories = new List<History>();
        private string Historylog;

        private Procurement? Procurement { get; set; }

        private List<Procurement> Procurements { get; set; }
        private bool IsSearch;


        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));
        SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(0x53, 0x53, 0x53));


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        public CardOfProcurement(Procurement procurement, List<Procurement> procurements, bool isSearch)
        {
            InitializeComponent();

            procurement = GET.Entry.ProcurementBy(procurement.Id);
            if (procurement.IsProcurementBlocked == true)
            {
                MessageBox.Show($"Данный тендер сейчас редактируется пользователем: \n {GET.View.Employees().Where(e => e.Id == procurement.ProcurementUserId).First().FullName}");
            }
            else
            {
                procurement.IsProcurementBlocked = true;
                procurement.ProcurementUserId = ((Employee)Application.Current.MainWindow.DataContext).Id;
                PULL.Procurement(procurement);
            }

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

            Procurements = procurements;
            IsSearch = isSearch;

            ProcurementStates = GET.View.DistributionOfProcurementStates(((Employee)Application.Current.MainWindow.DataContext).Position.Kind);
            ProcurementState.ItemsSource = ProcurementStates;

            RepresentativeTypes = GET.View.RepresentativeTypes();
            RepresentativeType.ItemsSource = RepresentativeTypes;

            CommissioningWorks = GET.View.CommissioningWorks();
            CommissioningWork.ItemsSource = CommissioningWorks;

            ProcurementRegions = GET.View.Regions();
            Regions.ItemsSource = ProcurementRegions;

            ProcurementPreferencesSelected = GET.View.ProcurementsPreferencesBy(procurement.Id);
            PreferencesNotSelected = GET.View.Preferences();

            foreach (ProcurementsPreference procurementsPreference in ProcurementPreferencesSelected)
                PreferencesSelected.Add(procurementsPreference.Preference);

            foreach (Preference preference in PreferencesSelected)
            {
                Preference preferenceToRemove = PreferencesNotSelected.FirstOrDefault(p => p.Id == preference.Id);
                if (preferenceToRemove != null)
                {
                    PreferencesNotSelected.Remove(preferenceToRemove);
                }
            }

            ProcurementPreferencesSelectedLV.ItemsSource = PreferencesSelected;
            ProcurementPreferencesNotSelectedLV.ItemsSource = PreferencesNotSelected;

            ProcurementDocumentsSelected = GET.View.ProcurementsDocumentsBy(procurement.Id);
            DocumentsNotSelected = GET.View.Documents();

            foreach (ProcurementsDocument procurementsDocument in ProcurementDocumentsSelected)
                DocumentsSelected.Add(procurementsDocument.Document);

            foreach (Document document in DocumentsSelected)
            {
                Document documentToRemove = DocumentsNotSelected.FirstOrDefault(p => p.Id == document.Id);
                if (documentToRemove != null)
                {
                    DocumentsNotSelected.Remove(documentToRemove);
                }
            }

            ProcurementDocumentsSelectedLV.ItemsSource = DocumentsSelected;
            ProcurementDocumentsNotSelectedLV.ItemsSource = DocumentsNotSelected;

            Calculators = GET.View.EmployeesBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов");
            Calculator.ItemsSource = Calculators;
            ProcurementsEmployeeCalculators = GET.View.ProcurementsEmployeesBy(procurement, "Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов");

            Minopttorgs = GET.View.Minopttorgs();
            Minopttorg.ItemsSource = Minopttorgs;

            LegalEntities = GET.View.LegalEntities();
            LegalEntity.ItemsSource = LegalEntities;

            Senders = GET.View.EmployeesBy("Специалист по работе с электронными площадками", "", "");
            Sender.ItemsSource = Senders;
            ProcurementsEmployeeSenders = GET.View.ProcurementsEmployeesBy(procurement, "Специалист по работе с электронными площадками", "", "");

            Managers = GET.View.EmployeesBy("Специалист тендерного отдела", "Руководитель тендерного отдела", "Заместитель руководителя тендреного отдела");
            Manager.ItemsSource = Managers;
            ProcurementsEmployeeManagers = GET.View.ProcurementsEmployeesBy(procurement, "Специалист тендерного отдела", "Руководитель тендерного отдела", "Заместитель руководителя тендреного отдела");

            Purchasers = GET.View.EmployeesBy("Руководитель отдела закупки", "Заместитель руководителя отдела закупок", "Специалист закупки");
            Purchaser.ItemsSource = Purchasers;
            ProcurementsEmployeePurchasers = GET.View.ProcurementsEmployeesBy(procurement, "Руководитель отдела закупки", "Заместитель руководителя отдела закупок", "Специалист закупки");

            ShipmentPlans = GET.View.ShipmentPlans();
            ShipmentPlan.ItemsSource = ShipmentPlans;

            ExecutionStates = GET.View.ExecutionStates();
            ExecutionState.ItemsSource = ExecutionStates;

            WarrantyStates = GET.View.WarrantyStates();
            WarrantyState.ItemsSource = WarrantyStates;

            SignedOriginals = GET.View.SignedOriginals();
            SignedOriginal.ItemsSource = SignedOriginals;

            Lawyers = GET.View.EmployeesBy("Юрист", "", "");
            Lawyer.ItemsSource = Lawyers;
            ProcurementsEmployeeLawyers = GET.View.ProcurementsEmployeesBy(procurement, "Юрист", "", "");

            Comments = GET.View.CommentsBy(procurement.Id);
            CommentsListView.ItemsSource = Comments;

            

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
                if(Procurement.Organization != null)
                {
                    if (Procurement.Organization.Name != null)
                        Organization.Text = Procurement.Organization.Name.ToString();
                    if (Procurement.Organization.PostalAddress != null)
                        PostalAddress.Text = Procurement.Organization.PostalAddress.ToString();
                }                
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
                DeadlineAndType.Text = Procurement.DeadlineAndType;
                DeliveryDeadline.Text = Procurement.DeliveryDeadline;
                AcceptanceDeadline.Text = Procurement.AcceptanceDeadline;
                ContractDeadline.Text = Procurement.ContractDeadline;
                Indefinitely.IsChecked = Procurement.Indefinitely;
                AnotherDeadline.Text = Procurement.AnotherDeadline;
                DeadlineAndOrder.Text = Procurement.DeadlineAndOrder;
                foreach (RepresentativeType representativeType in RepresentativeType.ItemsSource)
                    if (representativeType.Id == Procurement.RepresentativeTypeId)
                    {
                        RepresentativeType.SelectedItem = representativeType;
                        break;
                    }
                foreach (CommisioningWork commisioningWork in CommissioningWork.ItemsSource)
                    if (commisioningWork.Id == Procurement.CommissioningWorksId)
                    {
                        CommissioningWork.SelectedItem = commisioningWork;
                        break;
                    }
                PlaceCount.Text = Procurement.PlaceCount.ToString();
                FinesAndPennies.Text = Procurement.FinesAndPennies;
                PenniesPerDay.Text = Procurement.PenniesPerDay;
                TerminationConditions.Text = Procurement.TerminationConditions;
                EliminationDeadline.Text = Procurement.EliminationDeadline;
                GuaranteePeriod.Text = Procurement.GuaranteePeriod;
                INN.Text = Procurement.Inn;
                ContractNumber.Text = Procurement.ContractNumber;
                foreach (Employee employee in Calculator.ItemsSource)
                    if(ProcurementsEmployeeCalculators != null)
                    if (employee.Id == ProcurementsEmployeeCalculators.EmployeeId)
                    {
                        Calculator.SelectedItem = employee;
                        break;
                    }
                AssemblyNeed.IsChecked = Procurement.AssemblyNeed;
                foreach (Minopttorg minopttorg in Minopttorg.ItemsSource)
                    if (minopttorg.Id == Procurement.MinopttorgId)
                    {
                        Minopttorg.SelectedItem = minopttorg;
                        break;
                    }
                foreach (LegalEntity legalEntity in LegalEntity.ItemsSource)
                    if (legalEntity.Id == Procurement.LegalEntityId)
                    {
                        LegalEntity.SelectedItem = legalEntity;
                        break;
                    }
                Applications.IsChecked = Procurement.Applications;
                foreach (Employee employee in Sender.ItemsSource)
                    if (ProcurementsEmployeeSenders != null)
                        if (employee.Id == ProcurementsEmployeeSenders.EmployeeId)
                        {
                            Sender.SelectedItem = employee;
                            break;
                        }
                Bet.Text = Procurement.Bet.ToString();
                MinimalPrice.Text = Procurement.MinimalPrice.ToString();
                ContractAmount.Text = Procurement.ContractAmount.ToString();
                ReserveContractAmount.Text = Procurement.ReserveContractAmount.ToString();
                ProtocolDate.SelectedDate = Procurement.ProtocolDate;
                foreach (Employee employee in Manager.ItemsSource)
                    if (ProcurementsEmployeeManagers != null)
                        if (employee.Id == ProcurementsEmployeeManagers.EmployeeId)
                        {
                            Manager.SelectedItem = employee;
                            break;
                        }
                foreach (Employee employee in Purchaser.ItemsSource)
                    if (ProcurementsEmployeePurchasers != null)
                        if (employee.Id == ProcurementsEmployeePurchasers.EmployeeId)
                        {
                            Purchaser.SelectedItem = employee;
                            break;
                        }
                foreach (ShipmentPlan shipmentPlan in ShipmentPlan.ItemsSource)
                    if (shipmentPlan.Id == Procurement.ShipmentPlanId)
                    {
                        ShipmentPlan.SelectedItem = shipmentPlan;
                        break;
                    }
                WaitingList.IsChecked = Procurement.WaitingList;
                CalculatingCB.IsChecked = Procurement.Calculating;
                PurchasingCB.IsChecked = Procurement.Purchase;
                foreach (ExecutionState executionState in ExecutionState.ItemsSource)
                    if (executionState.Id == Procurement.ExecutionStateId)
                    {
                        ExecutionState.SelectedItem = executionState;
                        break;
                    }
                foreach (WarrantyState warrantyState in WarrantyState.ItemsSource)
                    if (warrantyState.Id == Procurement.WarrantyStateId)
                    {
                        WarrantyState.SelectedItem = warrantyState;
                        break;
                    }
                SigningDeadline.SelectedDate = Procurement.SigningDeadline;
                SigningDate.SelectedDate = Procurement.SigningDate;
                ConclusionDate.SelectedDate = Procurement.ConclusionDate;
                ActualDeliveryDate.SelectedDate = Procurement.ActualDeliveryDate;
                DepartureDate.SelectedDate = Procurement.DepartureDate;
                DeliveryDate.SelectedDate = Procurement.DeliveryDate;
                MaxAcceptanceDate.SelectedDate = Procurement.MaxAcceptanceDate;
                CorrectionDate.SelectedDate = Procurement.CorrectionDate;
                ActDate.SelectedDate = Procurement.ActDate;
                MaxDueDate.SelectedDate = Procurement.MaxDueDate;
                ClosingDate.SelectedDate = Procurement.ClosingDate;
                RealDueDate.SelectedDate = Procurement.RealDueDate;
                Amount.Text = Procurement.Amount.ToString();
                foreach (SignedOriginal signedOriginal in SignedOriginal.ItemsSource)
                    if (signedOriginal.Id == Procurement.SignedOriginalId)
                    {
                        SignedOriginal.SelectedItem = signedOriginal;
                        break;
                    }
                foreach (Employee employee in Lawyer.ItemsSource)
                    if (ProcurementsEmployeeLawyers != null)
                        if (employee.Id == ProcurementsEmployeeLawyers.EmployeeId)
                        {
                            Lawyer.SelectedItem = employee;
                            break;
                        }
                Judgment.IsChecked = Procurement.Judgment;
                FAS.IsChecked = Procurement.Fas;
            }

            Historylog = ProcurementState.Text;
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
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            if (Procurement.IsProcurementBlocked == true && Procurement.ProcurementUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
            {
                string warningMessage = null;

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
                Procurement.DeadlineAndType = DeadlineAndType.Text;
                Procurement.DeliveryDeadline = DeliveryDeadline.Text;
                Procurement.AcceptanceDeadline = AcceptanceDeadline.Text;
                Procurement.ContractDeadline = ContractDeadline.Text;
                Procurement.Indefinitely = Indefinitely.IsChecked;
                Procurement.AnotherDeadline = AnotherDeadline.Text;
                Procurement.DeadlineAndOrder = DeadlineAndOrder.Text;
                if (RepresentativeType.SelectedItem != null)
                    Procurement.RepresentativeTypeId = ((RepresentativeType)RepresentativeType.SelectedItem).Id;
                if (CommissioningWork.SelectedItem != null)
                    Procurement.CommissioningWorksId = ((CommisioningWork)CommissioningWork.SelectedItem).Id;
                if (PlaceCount.Text != "")
                    Procurement.PlaceCount = Convert.ToInt32(PlaceCount.Text);
                Procurement.FinesAndPennies = FinesAndPennies.Text;
                Procurement.PenniesPerDay = PenniesPerDay.Text;
                Procurement.TerminationConditions = TerminationConditions.Text;
                Procurement.EliminationDeadline = EliminationDeadline.Text;
                Procurement.GuaranteePeriod = GuaranteePeriod.Text;
                Procurement.Inn = INN.Text;
                Procurement.ContractNumber = ContractNumber.Text;
                Procurement.AssemblyNeed = AssemblyNeed.IsChecked;
                if (Minopttorg.SelectedItem != null)
                    Procurement.MinopttorgId = ((Minopttorg)Minopttorg.SelectedItem).Id;
                if (LegalEntity.SelectedItem != null)
                    Procurement.LegalEntityId = ((LegalEntity)LegalEntity.SelectedItem).Id;
                Procurement.Applications = Applications.IsChecked;
                if (Bet.Text != "")
                {
                    decimal BetDecimal;
                    if (decimal.TryParse(Bet.Text, out BetDecimal))
                    {
                        Procurement.Bet = BetDecimal;
                    }
                    else
                    {
                        warningMessage += " Ставка";
                    }
                }
                else
                    Procurement.Bet = null;
                if (MinimalPrice.Text != "")
                {
                    decimal MinimalPriceDecimal;
                    if (decimal.TryParse(MinimalPrice.Text, out MinimalPriceDecimal))
                    {
                        Procurement.MinimalPrice = MinimalPriceDecimal;
                    }
                    else
                    {
                        warningMessage += " Минимальная цена";
                    }
                }
                else
                    Procurement.MinimalPrice = null;
                if (ContractAmount.Text != "")
                {
                    decimal ContractAmountDecimal;
                    if (decimal.TryParse(ContractAmount.Text, out ContractAmountDecimal))
                    {
                        Procurement.ContractAmount = ContractAmountDecimal;
                    }
                    else
                    {
                        warningMessage += " Сумма контракта";
                    }
                }
                else
                    Procurement.ContractAmount = null;
                if (ReserveContractAmount.Text != "")
                {
                    decimal ReserveContractAmountDecimal;
                    if (decimal.TryParse(ReserveContractAmount.Text, out ReserveContractAmountDecimal))
                    {
                        Procurement.ReserveContractAmount = ReserveContractAmountDecimal;
                    }
                    else
                    {
                        warningMessage += " Измененная сумма контракта";
                    }
                }
                else
                    Procurement.ReserveContractAmount = null;
                Procurement.ProtocolDate = ProtocolDate.SelectedDate;
                if (ShipmentPlan.SelectedItem != null)
                    Procurement.ShipmentPlanId = ((ShipmentPlan)ShipmentPlan.SelectedItem).Id;
                Procurement.WaitingList = WaitingList.IsChecked;
                Procurement.Calculating = CalculatingCB.IsChecked;
                Procurement.Purchase = PurchasingCB.IsChecked;
                if (ExecutionState.SelectedItem != null)
                    Procurement.ExecutionStateId = ((ExecutionState)ExecutionState.SelectedItem).Id;
                if (WarrantyState.SelectedItem != null)
                    Procurement.WarrantyStateId = ((WarrantyState)WarrantyState.SelectedItem).Id;
                Procurement.SigningDeadline = SigningDeadline.SelectedDate;
                Procurement.SigningDate = SigningDate.SelectedDate;
                Procurement.ConclusionDate = ConclusionDate.SelectedDate;
                Procurement.ActualDeliveryDate = ActualDeliveryDate.SelectedDate;
                Procurement.DepartureDate = DepartureDate.SelectedDate;
                Procurement.DeliveryDate = DeliveryDate.SelectedDate;
                Procurement.MaxAcceptanceDate = MaxAcceptanceDate.SelectedDate;
                Procurement.CorrectionDate = CorrectionDate.SelectedDate;
                Procurement.ActDate = ActDate.SelectedDate;
                Procurement.MaxDueDate = MaxDueDate.SelectedDate;
                Procurement.ClosingDate = ClosingDate.SelectedDate;
                Procurement.RealDueDate = RealDueDate.SelectedDate;
                if (Amount.Text != "")
                {
                    decimal AmountDecimal;
                    if (decimal.TryParse(Amount.Text, out AmountDecimal))
                    {
                        Procurement.Amount = AmountDecimal;
                    }
                    else
                    {
                        warningMessage += " Оплаченная сумма";
                    }
                }
                else
                    Procurement.Amount = null;
                if (SignedOriginal.SelectedItem != null)
                    Procurement.SignedOriginalId = ((SignedOriginal)SignedOriginal.SelectedItem).Id;
                Procurement.Judgment = Judgment.IsChecked;
                Procurement.Fas = FAS.IsChecked;

                if (warningMessage != null)
                {
                    MessageBox.Show($"Неверный формат полей: {warningMessage}");
                    return;
                }
                PULL.Procurement(Procurement);
                if (ProcurementState.Text != Historylog)
                {
                    History? history = new History { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = ProcurementState.Text };
                    PUT.History(history);
                    HistoryListView.ItemsSource = null;
                    Histories.Clear();
                    Histories = GET.View.HistoriesBy(Procurement.Id);
                    HistoryListView.ItemsSource = Histories;
                    Historylog = ProcurementState.Text;
                }
            }
            else if (Procurement.ProcurementUserId != ((Employee)Application.Current.MainWindow.DataContext).Id)
                MessageBox.Show($"Данный тендер сейчас редактируется пользователем: \n{GET.View.Employees().Where(e => e.Id == Procurement.ProcurementUserId).First().FullName}\nВы не можете сохранить изменения");
        }

        private void Sender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcurementsEmployee procurementsEmployee = new ProcurementsEmployee();
            procurementsEmployee.ProcurementId = Procurement.Id;
            procurementsEmployee.EmployeeId = ((Employee)Sender.SelectedItem).Id;
            PUT.ProcurementsEmployeesBy(procurementsEmployee, "Специалист по работе с электронными площадками", "", "");
        }

        private void Calculator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcurementsEmployee procurementsEmployee = new ProcurementsEmployee();
            procurementsEmployee.ProcurementId = Procurement.Id;
            procurementsEmployee.EmployeeId = ((Employee)Calculator.SelectedItem).Id;
            PUT.ProcurementsEmployeesBy(procurementsEmployee, "Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов");
        }
        private void Manager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcurementsEmployee procurementsEmployee = new ProcurementsEmployee();
            procurementsEmployee.ProcurementId = Procurement.Id;
            procurementsEmployee.EmployeeId = ((Employee)Manager.SelectedItem).Id;
            PUT.ProcurementsEmployeesBy(procurementsEmployee, "Специалист тендерного отдела", "Руководитель тендерного отдела", "Заместитель руководителя тендреного отдела");
        }
        private void Purchaser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcurementsEmployee procurementsEmployee = new ProcurementsEmployee();
            procurementsEmployee.ProcurementId = Procurement.Id;
            procurementsEmployee.EmployeeId = ((Employee)Purchaser.SelectedItem).Id;
            PUT.ProcurementsEmployeesBy(procurementsEmployee, "Руководитель отдела закупки", "Заместитель руководителя отдела закупок", "Специалист закупки");
        }

        private void Lawyer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcurementsEmployee procurementsEmployee = new ProcurementsEmployee();
            procurementsEmployee.ProcurementId = Procurement.Id;
            procurementsEmployee.EmployeeId = ((Employee)Lawyer.SelectedItem).Id;
            PUT.ProcurementsEmployeesBy(procurementsEmployee, "Юрист", "", "");
        }

        private void ProcurementPreferencesSelectedLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Preference preferenceItem = ((ListView)sender).SelectedItem as Preference;
            if (preferenceItem != null)
            {
                ProcurementsPreference procurementsPreferenceToRemove = new ProcurementsPreference { Procurement = Procurement, Preference = preferenceItem, PreferenceId = preferenceItem.Id, ProcurementId = Procurement.Id};
                DELETE.ProcurementPreference(procurementsPreferenceToRemove);
                ProcurementPreferencesSelectedLV.ItemsSource = null;
                ProcurementPreferencesNotSelectedLV.ItemsSource = null;
                PreferencesSelected.Clear();
                ProcurementPreferencesSelected = GET.View.ProcurementsPreferencesBy(Procurement.Id);
                PreferencesNotSelected = GET.View.Preferences();

                foreach (ProcurementsPreference procurementsPreference in ProcurementPreferencesSelected)
                    PreferencesSelected.Add(procurementsPreference.Preference);

                foreach (Preference preference in PreferencesSelected)
                {
                    Preference preferenceToRemove = PreferencesNotSelected.FirstOrDefault(p => p.Id == preference.Id);
                    if (preferenceToRemove != null)
                    {
                        PreferencesNotSelected.Remove(preferenceToRemove);
                    }
                }
                ProcurementPreferencesSelectedLV.ItemsSource = PreferencesSelected;
                ProcurementPreferencesNotSelectedLV.ItemsSource = PreferencesNotSelected;
            }

        }
        
        private void ProcurementPreferencesNotSelectedLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Preference preferenceItem = ((ListView)sender).SelectedItem as Preference;
            if (preferenceItem != null)
            {
                ProcurementsPreference procurementsPreferenceToAdd = new ProcurementsPreference { Procurement = Procurement, Preference = preferenceItem };
                PUT.ProcurementsPreferences(procurementsPreferenceToAdd);
                ProcurementPreferencesSelectedLV.ItemsSource = null;
                ProcurementPreferencesNotSelectedLV.ItemsSource = null;
                PreferencesSelected.Clear();
                ProcurementPreferencesSelected = GET.View.ProcurementsPreferencesBy(Procurement.Id);
                PreferencesNotSelected = GET.View.Preferences();

                foreach (ProcurementsPreference procurementsPreference in ProcurementPreferencesSelected)
                    PreferencesSelected.Add(procurementsPreference.Preference);

                foreach (Preference preference in PreferencesSelected)
                {
                    Preference preferenceToRemove = PreferencesNotSelected.FirstOrDefault(p => p.Id == preference.Id);
                    if (preferenceToRemove != null)
                    {
                        PreferencesNotSelected.Remove(preferenceToRemove);
                    }
                }
                ProcurementPreferencesSelectedLV.ItemsSource = PreferencesSelected;
                ProcurementPreferencesNotSelectedLV.ItemsSource = PreferencesNotSelected;
            }
        }

        private void ProcurementDocumentsSelectedLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Document documentItem = ((ListView)sender).SelectedItem as Document;
            if (documentItem != null)
            {
                ProcurementsDocument procurementsDocumentsToRemove = new ProcurementsDocument { Procurement = Procurement, Document = documentItem, DocumentId = documentItem.Id, ProcurementId = Procurement.Id };
                DELETE.ProcurementDocument(procurementsDocumentsToRemove);
                ProcurementDocumentsSelectedLV.ItemsSource = null;
                ProcurementDocumentsNotSelectedLV.ItemsSource = null;
                DocumentsSelected.Clear();
                ProcurementDocumentsSelected = GET.View.ProcurementsDocumentsBy(Procurement.Id);
                DocumentsNotSelected = GET.View.Documents();

                foreach (ProcurementsDocument procurementsDocument in ProcurementDocumentsSelected)
                    DocumentsSelected.Add(procurementsDocument.Document);

                foreach (Document document in DocumentsSelected)
                {
                    Document documentToRemove = DocumentsNotSelected.FirstOrDefault(p => p.Id == document.Id);
                    if (documentToRemove != null)
                    {
                        DocumentsNotSelected.Remove(documentToRemove);
                    }
                }
                ProcurementDocumentsSelectedLV.ItemsSource = DocumentsSelected;
                ProcurementDocumentsNotSelectedLV.ItemsSource = DocumentsNotSelected;
            }
        }

        private void ProcurementDocumentsNotSelectedLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Document documentItem = ((ListView)sender).SelectedItem as Document;
            if (documentItem != null)
            {
                ProcurementsDocument procurementsDocumentsToAdd = new ProcurementsDocument { Procurement = Procurement, Document = documentItem, DocumentId = documentItem.Id, ProcurementId = Procurement.Id };
                PUT.ProcurementsDocuments(procurementsDocumentsToAdd);
                ProcurementDocumentsSelectedLV.ItemsSource = null;
                ProcurementDocumentsNotSelectedLV.ItemsSource = null;
                DocumentsSelected.Clear();
                ProcurementDocumentsSelected = GET.View.ProcurementsDocumentsBy(Procurement.Id);
                DocumentsNotSelected = GET.View.Documents();

                foreach (ProcurementsDocument procurementsDocument in ProcurementDocumentsSelected)
                    DocumentsSelected.Add(procurementsDocument.Document);

                foreach (Document document in DocumentsSelected)
                {
                    Document documentToRemove = DocumentsNotSelected.FirstOrDefault(p => p.Id == document.Id);
                    if (documentToRemove != null)
                    {
                        DocumentsNotSelected.Remove(documentToRemove);
                    }
                }
                ProcurementDocumentsSelectedLV.ItemsSource = DocumentsSelected;
                ProcurementDocumentsNotSelectedLV.ItemsSource = DocumentsNotSelected;
            }
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

        private void Distance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            Histories = GET.View.HistoriesBy(Procurement.Id);
            HistoryListView.ItemsSource = Histories;
            HistoryPopUp.IsOpen = !HistoryPopUp.IsOpen;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Точно выйти? Не забудьте СОХРАНИТЬ результат", "Выход без сохранения", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (Procurement.IsProcurementBlocked == true && Procurement.ProcurementUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
                { 
                    Procurement.IsProcurementBlocked = false;
                    Procurement.ProcurementUserId = null;
                    PULL.Procurement(Procurement);
                }
                if (IsSearch)
                {
                    _ = MainFrame.Navigate(new SearchPage(Procurements));
                }
                else
                {
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
            }
            else if (messageBoxResult == MessageBoxResult.No) { }
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
