using DatabaseLibrary.Entities.ProcurementProperties;
using DatabaseLibrary.Queries;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Windows.Themes;
using Parsething.Classes;
using Parsething.Functions;
using Parsething.Windows;
using ParsingLibrary;
using PdfSharp.Snippets.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private List<City>? Cities { get; set; }
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

        private List<ComponentCalculation> ComponentCalculations { get; set; }
        private bool IsSearch;


        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));
        SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(0x53, 0x53, 0x53));


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        public CardOfProcurement(Procurement procurement, bool isSearch)
        {
            InitializeComponent();
            UpdateUIForUserRole();
            procurement = GET.View.ProcurementBy(procurement.Id);
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

            IsSearch = isSearch;

            ProcurementStates = GET.View.DistributionOfProcurementStates(((Employee)Application.Current.MainWindow.DataContext).Position.Kind);
            ProcurementState.ItemsSource = ProcurementStates;

            RepresentativeTypes = GET.View.RepresentativeTypes();
            RepresentativeType.ItemsSource = RepresentativeTypes;

            CommissioningWorks = GET.View.CommissioningWorks();
            CommissioningWork.ItemsSource = CommissioningWorks;

            ProcurementRegions = GET.View.Regions();
            Regions.ItemsSource = ProcurementRegions;

            Cities = GET.View.Cities().OrderBy(c => c.Name).ToList();
            City.ItemsSource = Cities;

            int idToGetPreferences = procurement.ParentProcurementId ?? procurement.Id;
                ProcurementPreferencesSelected = GET.View.ProcurementsPreferencesBy(idToGetPreferences);
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


            int idToGetDocuments = procurement.ParentProcurementId ?? procurement.Id;
                ProcurementDocumentsSelected = GET.View.ProcurementsDocumentsBy(idToGetDocuments); 
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

            var rejectionReasons = EnumHelper.GetEnumValuesAndDescriptions<RejectionReason>();
            RejectionReason.ItemsSource = rejectionReasons;

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
            ScrollToBottom();

            Procurement = procurement;
            if (Procurement != null && ProcurementState != null)
            {
                foreach (ProcurementState procurementState in ProcurementState.ItemsSource)
                    if (procurementState.Id == Procurement.ProcurementStateId)
                    {
                        ProcurementState.SelectedItem = procurementState;
                        break;
                    }
                Id.Text = Procurement.DisplayId.ToString();
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
                if (Procurement.ResultDate != null)
                    ResultDate.Text = $"{Procurement.ResultDate}";
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
                        break;
                    }
                foreach (City city in City.ItemsSource)
                    if (city.Id == Procurement.CityId)
                    {
                        City.SelectedItem = city;
                        break;
                    }
                Distance.Text = Procurement.Distance.ToString();
                OrganizationContract.Text = Procurement.OrganizationContractName;
                PostalAddressContract.Text = Procurement.OrganizationContractPostalAddress;
                ContactPerson.Text = Procurement.ContactPerson;
                ContactPhone.Text = Procurement.ContactPhone;
                DeliveryDetails.Text = Procurement.DeliveryDetails;
                DeadlineAndType.Text = Procurement.DeadlineAndType;
                DeliveryDeadline.Text = Procurement.DeliveryDeadline;
                AcceptanceDeadline.Text = Procurement.AcceptanceDeadline;
                ContractDeadline.Text = Procurement.ContractDeadline;
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
                OrganizationEmail.Text = Procurement.OrganizationEmail;
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
                ApplicationsCB.IsChecked = Procurement.Applications;
                if (ApplicationsCB.IsChecked == true && Procurement.ParentProcurementId == null)
                {
                    AddApplicationButton.Visibility = Visibility.Visible;
                    ApplicationLabel.Visibility = Visibility.Visible;
                    ApplicationUL.Visibility = Visibility.Visible;
                    Applications.Visibility = Visibility.Visible;
                    LoadApplications();
                }
                else
                {
                    AddApplicationButton.Visibility = Visibility.Hidden;
                    ApplicationLabel.Visibility = Visibility.Hidden;
                    ApplicationUL.Visibility = Visibility.Hidden;
                    Applications.Visibility = Visibility.Hidden;
                }
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
                IsUnitPriceCB.IsChecked = Procurement.IsUnitPrice;
                ProtocolDate.SelectedDate = Procurement.ProtocolDate;
                CalculatingAmount.Text = Procurement.CalculatingAmount.ToString();
                if (Procurement?.ProcurementState?.Kind == "Отклонен")
                {
                    RejectionReason.Visibility = Visibility.Visible;
                    RejectionReason.SelectedValue = Procurement.RejectionReason;
                    RejectionReasonLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    RejectionReason.Visibility = Visibility.Hidden;
                    RejectionReasonLabel.Visibility = Visibility.Hidden;
                }
                if (Procurement?.ProcurementState?.Kind == "Проигран")
                {
                    CompetitorSum.Visibility = Visibility.Visible;
                    CompetitorSum.Text = Procurement.CompetitorSum.ToString();
                    CompetitorSumLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    CompetitorSum.Visibility = Visibility.Hidden;
                    CompetitorSumLabel.Visibility = Visibility.Hidden;
                }
                HeadOfAcceptance.Text = Procurement.HeadOfAcceptance;
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
                ExecutionPrice.Text = Procurement.ExecutionPrice.ToString();
                ExecutionDate.Text = Procurement.ExecutionDate.ToString();
                if (ExecutionState.Text == null || ExecutionState.Text == "Не требуется" || ExecutionState.Text == "" || ExecutionState.Text == "Добросовестность")
                {
                    ExecutionPrice.IsEnabled = false;
                    ExecutionPriceLabel.Foreground = Gray;
                    ExecutionDateLabel.Foreground = Gray;
                }
                else if (ExecutionState.Text != "Деньги (Возвратные)")
                {
                    ExecutionDateLabel.Foreground = Gray;
                }
                foreach (WarrantyState warrantyState in WarrantyState.ItemsSource)
                    if (warrantyState.Id == Procurement.WarrantyStateId)
                    {
                        WarrantyState.SelectedItem = warrantyState;
                        break;
                    }
                WarrantyPrice.Text = Procurement.WarrantyPrice.ToString();
                WarrantyDate.Text = Procurement.WarrantyDate.ToString();
                if (WarrantyState.Text == null || WarrantyState.Text == "Не требуется" || (WarrantyState.Text == "") || (WarrantyState.Text == "Добросовестность"))
                {
                    WarrantyPrice.IsEnabled = false;
                    WarrantyPriceLabel.Foreground = Gray;
                    WarrantyDateLabel.Foreground = Gray;
                }
                else if (WarrantyState.Text != "Деньги (Возвратные)")
                {
                    WarrantyDateLabel.Foreground = Gray;
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
        private void ScrollToBottom()
        {
            if (CommentsListView.Items.Count > 0)
            {
                var lastItem = CommentsListView.Items[0];
                CommentsListView.ScrollIntoView(lastItem);
            }
        }
        private void ExecutionState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExecutionState.SelectedItem != null)
            {
                if (((ExecutionState)ExecutionState.SelectedItem).Kind == null || ((ExecutionState)ExecutionState.SelectedItem).Kind == "Не требуется" || ((ExecutionState)ExecutionState.SelectedItem).Kind == "" || ((ExecutionState)ExecutionState.SelectedItem).Kind == "Добросовестность")
                {
                    ExecutionPrice.IsEnabled = false;
                    ExecutionPriceLabel.Foreground = Gray;
                    ExecutionDateLabel.Foreground = Gray;
                }
                else if (((ExecutionState)ExecutionState.SelectedItem).Kind != "Деньги (Возвратные)")
                {
                    ExecutionPrice.IsEnabled = true;
                    ExecutionPriceLabel.Foreground = Red;
                    ExecutionDateLabel.Foreground = Gray;
                }
                else
                {
                    ExecutionPrice.IsEnabled = true;
                    ExecutionPriceLabel.Foreground = Red;
                    ExecutionDateLabel.Foreground = Red;
                }
            }
        }

        private void WarrantyState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarrantyState.SelectedItem != null)
            {
                if (((WarrantyState)WarrantyState.SelectedItem).Kind == null || ((WarrantyState)WarrantyState.SelectedItem).Kind == "Не требуется" || ((WarrantyState)WarrantyState.SelectedItem).Kind == "" || ((ExecutionState)ExecutionState.SelectedItem).Kind == "Добросовестность")
                {
                    WarrantyPrice.IsEnabled = false;
                    WarrantyPriceLabel.Foreground = Gray;
                    WarrantyDateLabel.Foreground = Gray;
                }
                else if (((WarrantyState)WarrantyState.SelectedItem).Kind != "Деньги (Возвратные)")
                {
                    WarrantyPrice.IsEnabled = true;
                    WarrantyPriceLabel.Foreground = Red;
                    WarrantyDateLabel.Foreground = Gray;
                }
                else
                {
                    WarrantyPrice.IsEnabled = true;
                    WarrantyPriceLabel.Foreground = Red;
                    WarrantyDateLabel.Foreground = Red;
                }
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
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            if (Procurement.IsProcurementBlocked == true && Procurement.ProcurementUserId == ((Employee)Application.Current.MainWindow.DataContext).Id || Procurement.ProcurementUserId == null)
            {
                string warningMessage = null;

                if (ProcurementState.SelectedItem != null)
                {
                    Procurement.ProcurementStateId = ((ProcurementState)ProcurementState.SelectedItem).Id;
                    Procurement.ProcurementState = ((ProcurementState)ProcurementState.SelectedItem);
                }

                if (ProcurementState.SelectedItem != null)
                    CreateFolderIfNeeded(((ProcurementState)ProcurementState.SelectedItem).Kind);

                DateTime deadline;

                string pattern = @"(\d{2}\.\d{2}\.\d{4} \d{1,2}:\d{2}:\d{2})";
                Match match = Regex.Match(DeadLine.Text, pattern);

                if (match.Success)
                {
                    string dateTimeString = match.Groups[1].Value;
                    if (DateTime.TryParse(dateTimeString, out deadline))
                        Procurement.Deadline = deadline;
                }
                

                DateTime resultDate;

                if (DateTime.TryParse(ResultDate.Text, out resultDate))
                    Procurement.ResultDate = resultDate;
                else
                    Procurement.ResultDate = null;

                Procurement.Securing = Securing.Text;
                Procurement.Enforcement = Enforcement.Text;
                Procurement.Warranty = Warranty.Text;
                var selectedRegion = Regions.SelectedItem as Region;
                if (selectedRegion != null)
                    Procurement.RegionId = selectedRegion.Id;
                else if (Regions.Text == "")
                    Procurement.RegionId = null;
                else
                    warningMessage += " Регион";
                var selectedCity = City.SelectedItem as City;
                if (selectedCity != null)
                    Procurement.CityId = selectedCity.Id;
                else if (City.Text == "")
                    Procurement.CityId = null;
                else
                    warningMessage += " Город";
                if (Distance.Text != "")
                {
                    int distance;
                    if (int.TryParse(Distance.Text, out distance))
                        Procurement.Distance = distance;
                    else
                        warningMessage += " Расстояние";
                }
                else
                    Procurement.Distance = null;
                Procurement.OrganizationContractName = OrganizationContract.Text;
                Procurement.OrganizationContractPostalAddress = PostalAddressContract.Text;
                Procurement.ContactPerson = ContactPerson.Text;
                Procurement.ContactPhone = ContactPhone.Text;
                Procurement.DeliveryDetails = DeliveryDetails.Text;
                Procurement.DeadlineAndType = DeadlineAndType.Text;
                Procurement.DeliveryDeadline = DeliveryDeadline.Text;
                Procurement.AcceptanceDeadline = AcceptanceDeadline.Text;
                Procurement.ContractDeadline = ContractDeadline.Text;
                Procurement.DeadlineAndOrder = DeadlineAndOrder.Text;
                if (RepresentativeType.SelectedItem != null)
                    Procurement.RepresentativeTypeId = ((RepresentativeType)RepresentativeType.SelectedItem).Id;
                else
                    Procurement.RepresentativeTypeId = null;
                if (CommissioningWork.SelectedItem != null)
                    Procurement.CommissioningWorksId = ((CommisioningWork)CommissioningWork.SelectedItem).Id;
                else
                    Procurement.CommissioningWorksId = null;
                if (PlaceCount.Text != "")
                    Procurement.PlaceCount = Convert.ToInt32(PlaceCount.Text);
                Procurement.FinesAndPennies = FinesAndPennies.Text;
                Procurement.PenniesPerDay = PenniesPerDay.Text;
                Procurement.TerminationConditions = TerminationConditions.Text;
                Procurement.EliminationDeadline = EliminationDeadline.Text;
                Procurement.GuaranteePeriod = GuaranteePeriod.Text;
                Procurement.Inn = INN.Text;
                Procurement.ContractNumber = ContractNumber.Text;
                Procurement.OrganizationEmail = OrganizationEmail.Text;
                Procurement.AssemblyNeed = AssemblyNeed.IsChecked;
                if (Minopttorg.SelectedItem != null)
                    Procurement.MinopttorgId = ((Minopttorg)Minopttorg.SelectedItem).Id;
                else
                    Procurement.MinopttorgId = null;
                if (LegalEntity.SelectedItem != null)
                    Procurement.LegalEntityId = ((LegalEntity)LegalEntity.SelectedItem).Id;
                else
                    Procurement.LegalEntityId = null;
                Procurement.Applications = ApplicationsCB.IsChecked;
                if (ApplicationsCB.IsChecked == true && Procurement.ParentProcurementId == null)
                {
                    AddApplicationButton.Visibility = Visibility.Visible;
                    ApplicationLabel.Visibility = Visibility.Visible;
                    ApplicationUL.Visibility = Visibility.Visible;
                    Applications.Visibility = Visibility.Visible;
                }
                else
                {
                    AddApplicationButton.Visibility = Visibility.Hidden;
                    ApplicationLabel.Visibility = Visibility.Hidden;
                    ApplicationUL.Visibility = Visibility.Hidden;
                    Applications.Visibility = Visibility.Hidden;
                }
                if (Bet.Text != "")
                {
                    decimal betDecimal;
                    if (decimal.TryParse(Bet.Text, out betDecimal))
                        Procurement.Bet = betDecimal;
                    else
                        warningMessage += " Ставка";
                }
                else
                    Procurement.Bet = null;
                if (MinimalPrice.Text != "")
                {
                    decimal minimalPriceDecimal;
                    if (decimal.TryParse(MinimalPrice.Text, out minimalPriceDecimal))
                        Procurement.MinimalPrice = minimalPriceDecimal;
                    else
                        warningMessage += " Минимальная цена";
                }
                else
                    Procurement.MinimalPrice = null;
                if (ContractAmount.Text != "")
                {
                    decimal contractAmountDecimal;
                    if (decimal.TryParse(ContractAmount.Text, out contractAmountDecimal))
                        Procurement.ContractAmount = contractAmountDecimal;
                    else
                        warningMessage += " Сумма контракта";
                }
                else
                    Procurement.ContractAmount = null;
                if (ReserveContractAmount.Text != "")
                {
                    decimal reserveContractAmountDecimal;
                    if (decimal.TryParse(ReserveContractAmount.Text, out reserveContractAmountDecimal))
                        Procurement.ReserveContractAmount = reserveContractAmountDecimal;
                    else
                        warningMessage += " Измененная сумма контракта";
                }
                else
                    Procurement.ReserveContractAmount = null;
                Procurement.IsUnitPrice = IsUnitPriceCB.IsChecked;
                Procurement.ProtocolDate = ProtocolDate.SelectedDate;
                if (RejectionReason.SelectedValue is RejectionReason selectedReason)
                    Procurement.RejectionReason = selectedReason;
                if (ProcurementState.Text == "Отклонен")
                {
                    RejectionReason.Visibility = Visibility.Visible;
                    RejectionReasonLabel.Visibility = Visibility.Visible;
                    RejectionReason.SelectedValue = Procurement.RejectionReason;
                }
                else
                {
                    RejectionReason.Visibility = Visibility.Hidden;
                    RejectionReasonLabel.Visibility = Visibility.Hidden;
                }
                if (CompetitorSum.Text != "")
                {
                    decimal competitorSum;
                    if (decimal.TryParse(CompetitorSum.Text.Replace(".",","), out competitorSum))
                        Procurement.CompetitorSum = competitorSum;
                    else
                        warningMessage += " Сумма конкурентов";
                }
                if (ProcurementState.Text == "Проигран")
                {
                    CompetitorSum.Visibility = Visibility.Visible;
                    CompetitorSumLabel.Visibility = Visibility.Visible;
                    CompetitorSum.Text = Procurement.CompetitorSum.ToString();
                }
                else
                {
                    CompetitorSum.Visibility = Visibility.Hidden;
                    CompetitorSumLabel.Visibility = Visibility.Hidden;
                }
                Procurement.HeadOfAcceptance = HeadOfAcceptance.Text;
                if (ShipmentPlan.SelectedItem != null)
                    Procurement.ShipmentPlanId = ((ShipmentPlan)ShipmentPlan.SelectedItem).Id;
                else
                    Procurement.ShipmentPlanId = null;
                Procurement.WaitingList = WaitingList.IsChecked;
                Procurement.Calculating = CalculatingCB.IsChecked;
                Procurement.Purchase = PurchasingCB.IsChecked;
                if (ExecutionState.SelectedItem != null)
                    Procurement.ExecutionStateId = ((ExecutionState)ExecutionState.SelectedItem).Id;
                else
                    Procurement.ExecutionStateId = null;
                if (ExecutionPrice.Text != "")
                {
                    decimal executionPriceDecimal;
                    if (decimal.TryParse(ExecutionPrice.Text, out executionPriceDecimal))
                        Procurement.ExecutionPrice = executionPriceDecimal;
                    else
                        warningMessage += " Стоимость БГ";
                }
                else
                    Procurement.ExecutionPrice = null;
                Procurement.ExecutionDate = ExecutionDate.SelectedDate;
                if (WarrantyState.SelectedItem != null)
                    Procurement.WarrantyStateId = ((WarrantyState)WarrantyState.SelectedItem).Id;
                else
                    Procurement.WarrantyStateId = null;
                if (WarrantyPrice.Text != "")
                {
                    decimal warrantyPriceDecimal;
                    if (decimal.TryParse(WarrantyPrice.Text, out warrantyPriceDecimal))
                        Procurement.WarrantyPrice = warrantyPriceDecimal;
                    else
                        warningMessage += " Стоимость БГ";
                }
                else
                    Procurement.WarrantyPrice = null;
                Procurement.WarrantyDate = WarrantyDate.SelectedDate;
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
                    decimal amountDecimal;
                    if (decimal.TryParse(Amount.Text, out amountDecimal))
                        Procurement.Amount = amountDecimal;
                    else
                        warningMessage += " Оплаченная сумма";
                }
                else
                    Procurement.Amount = null;
                if (SignedOriginal.SelectedItem != null)
                    Procurement.SignedOriginalId = ((SignedOriginal)SignedOriginal.SelectedItem).Id;
                else
                    Procurement.SignedOriginalId = null;
                    Procurement.Judgment = Judgment.IsChecked;
                Procurement.Fas = FAS.IsChecked;

                if (warningMessage != null)
                {
                    MessageBox.Show($"Неверный формат полей: {warningMessage}");
                    return;
                }
                PULL.Procurement(Procurement);
                AutoClosingMessageBox.ShowAutoClosingMessageBox($"Успешно сохранено.", "Информация", 1000);
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
        public void CreateFolderIfNeeded(string procurementState)
        {
            if (procurementState == "Оформить")
            {
                string networkPath = @"\\192.168.1.128\Parsething\Tender_files";
                string newFolderPath = System.IO.Path.Combine(networkPath, Procurement.DisplayId.ToString());

                try
                {
                    if (!Directory.Exists(newFolderPath))
                    {
                        Directory.CreateDirectory(newFolderPath);
                        AutoClosingMessageBox.ShowAutoClosingMessageBox($"Папка {newFolderPath} успешно создана.", "Информация", 1500); 
                    }
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.ShowAutoClosingMessageBox($"Ошибка при создании папки: {ex.Message}", "Ошибка", 1500); 
                }
            }
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
                int idToGetPreferences = Procurement.ParentProcurementId ?? Procurement.Id;
                Procurement procurement = GET.Entry.ProcurementBy(idToGetPreferences);

                ProcurementsPreference procurementsPreferenceToRemove = new ProcurementsPreference
                {
                    Procurement = procurement,
                    Preference = preferenceItem,
                    PreferenceId = preferenceItem.Id,
                    ProcurementId = idToGetPreferences
                };
                DELETE.ProcurementPreference(procurementsPreferenceToRemove);

                ProcurementPreferencesSelectedLV.ItemsSource = null;
                ProcurementPreferencesNotSelectedLV.ItemsSource = null;
                PreferencesSelected.Clear();
                ProcurementPreferencesSelected = GET.View.ProcurementsPreferencesBy(idToGetPreferences);
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
                int idToGetPreferences = Procurement.ParentProcurementId ?? Procurement.Id;
                Procurement procurement = GET.Entry.ProcurementBy(idToGetPreferences);

                ProcurementsPreference procurementsPreferenceToAdd = new ProcurementsPreference
                {
                    Procurement = procurement,
                    Preference = preferenceItem,
                    PreferenceId = preferenceItem.Id,
                    ProcurementId = idToGetPreferences
                };
                PUT.ProcurementsPreferences(procurementsPreferenceToAdd);

                ProcurementPreferencesSelectedLV.ItemsSource = null;
                ProcurementPreferencesNotSelectedLV.ItemsSource = null;
                PreferencesSelected.Clear();
                ProcurementPreferencesSelected = GET.View.ProcurementsPreferencesBy(idToGetPreferences);
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
                int idToGetDocuments = Procurement.ParentProcurementId ?? Procurement.Id;
                Procurement procurement = GET.Entry.ProcurementBy(idToGetDocuments);

                ProcurementsDocument procurementsDocumentsToRemove = new ProcurementsDocument 
                {
                    Procurement = procurement,
                    Document = documentItem,
                    DocumentId = documentItem.Id,
                    ProcurementId = idToGetDocuments
                };
                DELETE.ProcurementDocument(procurementsDocumentsToRemove);
                ProcurementDocumentsSelectedLV.ItemsSource = null;
                ProcurementDocumentsNotSelectedLV.ItemsSource = null;
                DocumentsSelected.Clear();
                ProcurementDocumentsSelected = GET.View.ProcurementsDocumentsBy(idToGetDocuments);
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
                int idToGetDocuments = Procurement.ParentProcurementId ?? Procurement.Id;
                Procurement procurement = GET.Entry.ProcurementBy(idToGetDocuments);

                ProcurementsDocument procurementsDocumentsToAdd = new ProcurementsDocument 
                {
                    Procurement = procurement, 
                    Document = documentItem,
                    DocumentId = documentItem.Id,
                    ProcurementId = idToGetDocuments
                };
                PUT.ProcurementsDocuments(procurementsDocumentsToAdd);
                ProcurementDocumentsSelectedLV.ItemsSource = null;
                ProcurementDocumentsNotSelectedLV.ItemsSource = null;
                DocumentsSelected.Clear();
                ProcurementDocumentsSelected = GET.View.ProcurementsDocumentsBy(idToGetDocuments);
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
                ScrollToBottom();
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
            if (Procurement.IsProcurementBlocked == true && Procurement.ProcurementUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
            {
                Procurement.IsProcurementBlocked = false;
                Procurement.ProcurementUserId = null;
                PULL.Procurement(Procurement);
            }
            Procurement existingProcurement = GlobalUsingValues.Instance.Procurements.FirstOrDefault(p => p.Id == Procurement.Id);
            if (existingProcurement != null)
            {
                existingProcurement.ResultDate = Procurement.ResultDate;
                existingProcurement.CalculatingAmount = Procurement.CalculatingAmount;
                existingProcurement.PurchaseAmount = Procurement.PurchaseAmount;
                existingProcurement.ContractAmount = Procurement.ContractAmount;
                existingProcurement.ReserveContractAmount = Procurement.ReserveContractAmount;
                existingProcurement.ProcurementStateId = Procurement.ProcurementStateId;
                existingProcurement.ProcurementState = Procurement.ProcurementState;
                existingProcurement.Applications = Procurement.Applications;
                existingProcurement.SigningDeadline = Procurement.SigningDeadline;
                existingProcurement.SigningDate = Procurement.SigningDate;
                existingProcurement.ConclusionDate = Procurement.ConclusionDate;
                existingProcurement.Calculating = Procurement.Calculating;
                existingProcurement.Purchase = Procurement.Purchase;
                existingProcurement.ActualDeliveryDate = Procurement.ActualDeliveryDate;
                existingProcurement.MaxAcceptanceDate = Procurement.MaxAcceptanceDate;
                existingProcurement.Region = Procurement.Region;
                existingProcurement.RegionId = Procurement.RegionId;

            }
            MainFrame.GoBack();


            //var employee = (Employee)Application.Current.MainWindow.DataContext;
            //var positionKind = employee.Position.Kind;

            //var navigationMap = new Dictionary<string, Page>
            //    {
            //        { "Администратор", new Pages.AdministratorPage() },
            //        { "Руководитель отдела расчетов", new Pages.HeadsOfCalculatorsPage() },
            //        { "Заместитель руководителя отдела расчетов", new Pages.HeadsOfCalculatorsPage() },
            //        { "Специалист отдела расчетов", new Pages.CalculatorPage() },
            //        { "Руководитель тендерного отдела", new Pages.HeadsOfManagersPage() },
            //        { "Заместитель руководителя тендерного отдела", new Pages.HeadsOfManagersPage() },
            //        { "Специалист по работе с электронными площадками", new Pages.EPlatformSpecialistPage() },
            //        { "Специалист тендерного отдела", new Pages.ManagerPage() },
            //        { "Руководитель отдела закупки", new Pages.PurchaserPage() },
            //        { "Заместитель руководителя отдела закупок", new Pages.PurchaserPage() },
            //        { "Специалист закупки", new Pages.PurchaserPage() },
            //        { "Руководитель отдела производства", new Pages.AssemblyPage() },
            //        { "Заместитель руководителя отдела производства", new Pages.AssemblyPage() },
            //        { "Специалист по производству", new Pages.AssemblyPage() },
            //        { "Юрист", new Pages.LawyerPage() }
            //    };

            //if (navigationMap.TryGetValue(positionKind, out var page))
            //{
            //    _ = MainFrame.Navigate(page);
            //}
            //else
            //{
            //}
        }
        private void ResetAll()
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

            PaymentLabel.Foreground = Gray;
            PaymentUL.Fill = Gray;
            PaymentLV.Visibility = Visibility.Hidden;

            ApplicationLabel.Foreground = Gray;
            ApplicationUL.Fill = Gray;
            ApplicationLV.Visibility = Visibility.Hidden;
        }

        private void SetActiveElement(Label label, Rectangle ul, ListView lv)
        {
            label.Foreground = Red;
            ul.Fill = Red;
            lv.Visibility = Visibility.Visible;
        }

        private void ProcurementInfo_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(ProcurementInfoLabel, ProcurementInfoUL, ProcurementInfoLV);
        }

        private void ContractInfo_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(ContractInfoLabel, ContractInfoUL, ContractInfoLV);
        }

        private void ContractNuances_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(ContractNuancesLabel, ContractNuancesUL, ContractNuancesLV);
        }

        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(CalculatingLabel, CalculatingUL, CalculatingLV);
        }

        private void Sending_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(SendingLabel, SendingUL, SendingLV);
        }

        private void Bargaining_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(BargainingLabel, BargainingUL, BargainingLV);
        }

        private void Supply_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(SupplyLabel, SupplyUL, SupplyLV);
        }

        private void Payment_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(PaymentLabel, PaymentUL, PaymentLV);
        }
        private void Applications_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            SetActiveElement(ApplicationLabel, ApplicationUL, ApplicationLV);
        }
        private void LoadApplications()
        {
            var applications = GET.View.ApplicationsBy(Procurement.DisplayId);

            decimal? applicationAmount = 0;

            foreach (var application in applications)
            { 
                applicationAmount += application.ContractAmount;
            }

            ApplicationAmount.Text = applicationAmount.ToString();
            int displayId = Procurement.DisplayId.GetValueOrDefault(0);
            ApplicationCount.Text = GET.Aggregate.CountOfApplications(displayId).ToString();
            RemainingApplicationAmount.Text = (Procurement.ContractAmount - applicationAmount).ToString();
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(615) });

            int row = 0;

            foreach (var application in applications)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Label label = new Label
                {
                    Content = $"Заявка {application.ApplicationNumber}",
                    Style = (Style)FindResource("Label.CardOfProcurement")
                };
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);
                grid.Children.Add(label);

                TextBox textBox = new TextBox
                {
                    Text = application.ContractAmount.ToString(),
                    Style = (Style)FindResource("SingleLineInput.CardOfProcurement"),
                    MinHeight = 50,
                    MaxHeight = 150,
                    TextWrapping = TextWrapping.Wrap,
                    IsReadOnly = true
                };
                Grid.SetColumn(textBox, 1);
                Grid.SetRow(textBox, row);
                grid.Children.Add(textBox);

                row++;
            }

            ApplicationLV.Items.Add(grid);

            ComponentCalculations = GET.View.ComponentCalculationsBy(Procurement.Id);

            ListViewInitialization.CardRemainingComponentCalculationsListViewInitialization(ComponentCalculations, RemainingComponentCalculationLV, Procurement);
        }

        private void AddApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null)
            {
                AddApplicationWindow addApplicationWindow = new AddApplicationWindow(Procurement);
                addApplicationWindow.Show();
            }
        }

        private void GoToProcurementFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string networkPath = $@"\\192.168.1.128\Parsething\Tender_files\{Procurement.DisplayId}";

            try
            {
                Process.Start("explorer.exe", networkPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при попытке открыть папку: {ex.Message}");
            }
        }
        
        private void UpdateUIForUserRole()
        {
            switch (((Employee)Application.Current.MainWindow.DataContext).Position.Kind)
            {
                case "Администратор":
                    RefreshProcurementButton.Visibility = Visibility.Visible;
                    break;
                case "Руководитель отдела расчетов":
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    Distance.IsReadOnly = true;
                    OrganizationContract.IsReadOnly = true;
                    PostalAddressContract.IsReadOnly = true;
                    ContactPerson.IsReadOnly = true;
                    ContactPhone.IsReadOnly = true;
                    OrganizationEmail.IsReadOnly = true;
                    HeadOfAcceptance.IsReadOnly = true;
                    DeliveryDetails.IsReadOnly = true;
                    break;
                case "Заместитель руководителя отдела расчетов":
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    Distance.IsReadOnly = true;
                    OrganizationContract.IsReadOnly = true;
                    PostalAddressContract.IsReadOnly = true;
                    ContactPerson.IsReadOnly = true;
                    ContactPhone.IsReadOnly = true;
                    OrganizationEmail.IsReadOnly = true;
                    HeadOfAcceptance.IsReadOnly = true;
                    DeliveryDetails.IsReadOnly = true;
                    break;
                case "Специалист отдела расчетов":
                    DeadLine.IsReadOnly = true;
                    ResultDate.IsReadOnly = true;
                    Securing.IsReadOnly = true;
                    Enforcement.IsReadOnly = true;
                    Warranty.IsReadOnly = true;
                    ProcurementInfo.IsEnabled = false;
                    ContractInfo.IsEnabled = false;
                    ContractNuances.IsEnabled = false;
                    Calculating.IsEnabled = false;
                    Sending.IsEnabled = false;
                    Bargaining.IsEnabled = false;
                    Supply.IsEnabled = false;
                    Payment.IsEnabled = false;
                    Applications.IsEnabled = false;
                    AddApplicationButton.IsEnabled = false;
                    break;
                case "Руководитель тендерного отдела":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    break;
                case "Заместитель руководителя тендреного отдела":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    break;
                case "Специалист по работе с электронными площадками":
                    RefreshProcurementButton.Visibility = Visibility.Visible;
                    Calculator.IsEnabled = false;
                    Manager.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    break;
                case "Специалист тендерного отдела":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
                case "Руководитель отдела закупки":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
                case "Заместитель руководителя отдела закупок":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
                case "Специалист закупки":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
                case "Руководитель отдела производства":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
                case "Заместитель руководителя отдела производства":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
                case "Специалист по производству":
                    DeadLine.IsReadOnly = true;
                    ResultDate.IsReadOnly = true;
                    Securing.IsReadOnly = true;
                    Enforcement.IsReadOnly = true;
                    Warranty.IsReadOnly = true;
                    ProcurementInfo.IsEnabled = false;
                    ContractInfo.IsEnabled = false;
                    ContractNuances.IsEnabled = false;
                    Calculating.IsEnabled = false;
                    Sending.IsEnabled = false;
                    Bargaining.IsEnabled = false;
                    Supply.IsEnabled = false;
                    Payment.IsEnabled = false;
                    Applications.IsEnabled = false;
                    AddApplicationButton.IsEnabled = false;
                    break;
                case "Юрист":
                    Calculator.IsEnabled = false;
                    Sender.IsEnabled = false;
                    Purchaser.IsEnabled = false;
                    Manager.IsEnabled = false;
                    break;
            }
        }
        private void RefreshProcurementInfo()
        {
            if (Procurement != null && ProcurementState != null)
            {
                Procurement = GET.Entry.ProcurementBy(Procurement.Id);
                Id.Text = Procurement.Id.ToString();
                if (Procurement.Number != null)
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
                if (Procurement.ResultDate != null)
                    ResultDate.Text = $"{Procurement.ResultDate}";
                InitialPrice.Text = Procurement.InitialPrice.ToString();
                if (Procurement.Organization != null)
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
                if (Procurement.Location != null)
                    Location.Text = Procurement.Location.ToString();
            }
        }
        private void RefreshProcurementButton_Click(object sender, RoutedEventArgs e)
        {
            string requestUri = Procurement.RequestUri;
            Thread thread = new Thread(() =>
            {
                Source source = new(requestUri);

                try
                {
                    foreach (Process process in Process.GetProcessesByName("msedgedriver"))
                    {
                        process.Kill();
                        Thread.Sleep(1000);
                    }
                }
                catch { }

                // Выполнить RefreshProcurementInfo на основном потоке
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RefreshProcurementInfo();
                });
            });

            // Запуск потока
            thread.Start();

        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null && Procurement.RequestUri != null)
            {
                string url = Procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
    }
}
