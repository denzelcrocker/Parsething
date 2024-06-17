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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Parsething.Functions;
using LiveCharts.Configurations;
using Parsething.Classes;


namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddApplicationWindow.xaml
    /// </summary>
    public partial class AddApplicationWindow : Window
    {
        private Frame MainFrame { get; set; } = null!;
        private List<ComponentCalculation>? ComponentCalculations { get; set; }
        private Procurement? Procurement { get; set; }
        private List<Procurement?> Applications { get; set; }


        public AddApplicationWindow(Procurement procurement)
        {
            InitializeComponent();

            Procurement = procurement;
            decimal? remainingAmount = null;

            if (procurement.IsUnitPrice == true)
                remainingAmount = procurement.InitialPrice;
            else if (procurement.IsUnitPrice == false || procurement.IsUnitPrice == null)
            {
                if (procurement.ReserveContractAmount != null)
                    remainingAmount = procurement.ReserveContractAmount.Value;
                else if (procurement.ContractAmount != null)
                    remainingAmount = procurement.ContractAmount.Value;
            }
            if (remainingAmount == null)
                remainingAmount = 0;
            else
            {
                Applications = GET.View.ApplicationsBy(procurement.Id);
                foreach (Procurement application in Applications)
                {
                    if (procurement.IsUnitPrice == true)
                        remainingAmount -= application.ApplicationAmount ?? 0;
                    else
                        remainingAmount -= application.ApplicationAmount ?? 0;
                }
            }
            ApplicationRemaining.Text = remainingAmount.ToString();



            ComponentCalculations = GET.View.ComponentCalculationsBy(Procurement.Id);
            NumberOfApplicationTextBlock.Text = GET.Aggregate.NumberOfApplication(Procurement.Id).ToString();

            if (procurement.IsUnitPrice == true)
            {
                
                ListViewInitialization.RemainingComponentCalculationsListViewInitialization(ComponentCalculations, ComponentCalculationsListView, Procurement);
            }
            else
            {
                ListViewInitialization.RemainingComponentCalculationsListViewInitialization(ComponentCalculations, ComponentCalculationsListView, Procurement);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void AddApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(ApplicationRemaining.Text, out decimal applicationRemainingValue) && decimal.TryParse(ApplicationAmount.Text, out decimal applicationAmountValue))
            {
                if (applicationRemainingValue - applicationAmountValue < 0)
                    MessageBox.Show("Лимит суммы заявки превышен");
                else
                {
                    Procurement newProcurement = new Procurement()
                    {
                        ParentProcurementId = Procurement.Id,
                        RequestUri = Procurement.RequestUri,
                        Number = Procurement.Number,
                        LawId = Procurement.LawId,
                        Object = Procurement.Object,
                        InitialPrice = Procurement.InitialPrice,
                        OrganizationId = Procurement.OrganizationId,
                        MethodId = Procurement.MethodId,
                        PlatformId = Procurement.PlatformId,
                        Location = Procurement.Location,
                        PostingDate = Procurement.PostingDate,
                        StartDate = Procurement.StartDate,
                        Deadline = Procurement.Deadline,
                        ResultDate = Procurement.ResultDate,
                        TimeZoneId = Procurement.TimeZoneId,
                        Securing = Procurement.Securing,
                        Enforcement = Procurement.Enforcement,
                        Warranty = Procurement.Warranty,
                        RegionId = Procurement.RegionId,
                        OrganizationContractName = Procurement.OrganizationContractName,
                        OrganizationContractPostalAddress = Procurement.OrganizationContractPostalAddress,
                        ContactPerson = Procurement.ContactPerson,
                        ContactPhone = Procurement.ContactPhone,
                        DeliveryDetails = Procurement.DeliveryDetails,
                        DeadlineAndType = Procurement.DeadlineAndType,
                        DeliveryDeadline = Procurement.DeliveryDeadline,
                        AcceptanceDeadline = Procurement.AcceptanceDeadline,
                        ContractDeadline = Procurement.ContractDeadline,
                        Indefinitely = Procurement.Indefinitely,
                        AnotherDeadline = Procurement.AnotherDeadline,
                        DeadlineAndOrder = Procurement.DeadlineAndOrder,
                        RepresentativeTypeId = Procurement.RepresentativeTypeId,
                        CommissioningWorksId = Procurement.CommissioningWorksId,
                        PlaceCount = Procurement.PlaceCount,
                        FinesAndPennies = Procurement.FinesAndPennies,
                        PenniesPerDay = Procurement.PenniesPerDay,
                        TerminationConditions = Procurement.TerminationConditions,
                        EliminationDeadline = Procurement.EliminationDeadline,
                        GuaranteePeriod = Procurement.GuaranteePeriod,
                        Inn = Procurement.Inn,
                        ContractNumber = Procurement.ContractNumber,
                        OrganizationEmail = Procurement.OrganizationEmail,
                        EmployeeId = Procurement.EmployeeId,
                        AssemblyNeed = Procurement.AssemblyNeed,
                        MinopttorgId = Procurement.MinopttorgId,
                        LegalEntityId = Procurement.LegalEntityId,
                        //Applications = Procurement.Applications,
                        ApplicationAmount = decimal.TryParse(ApplicationAmount.Text, out var applicationAmount) ? applicationAmount : 0,
                        Bet = Procurement.Bet,
                        MinimalPrice = Procurement.MinimalPrice,
                        ContractAmount = Procurement.ContractAmount,
                        ReserveContractAmount = Procurement.ReserveContractAmount,
                        IsUnitPrice = Procurement.IsUnitPrice,
                        ProtocolDate = Procurement.ProtocolDate,
                        HeadOfAcceptance = Procurement.HeadOfAcceptance,
                        ExecutionStateId = Procurement.ExecutionStateId,
                        ExecutionPrice = Procurement.ExecutionPrice,
                        ExecutionDate = Procurement.ExecutionDate,
                        WarrantyStateId = Procurement.WarrantyStateId,
                        WarrantyPrice = Procurement.WarrantyPrice,
                        WarrantyDate = Procurement.WarrantyDate,
                        SigningDeadline = Procurement.SigningDeadline,
                        SigningDate = Procurement.SigningDate,
                        ConclusionDate = Procurement.ConclusionDate,
                        SignedOriginalId = Procurement.SignedOriginalId,
                        Judgment = Procurement.Judgment,
                        Fas = Procurement.Fas,
                        ProcurementStateId = 9,
                        CalculatingAmount = Procurement.CalculatingAmount,
                        PurchaseAmount = Procurement.PurchaseAmount,
                        PassportOfMonitor = Procurement.PassportOfMonitor,
                        PassportOfPC = Procurement.PassportOfPC,
                        PassportOfMonoblock = Procurement.PassportOfMonoblock,
                        PassportOfNotebook = Procurement.PassportOfNotebook,
                    };
                    PUT.Procurement(newProcurement);

                    ListViewInitialization.CopyComponentCalculationsToNewProcurement(newProcurement, ComponentCalculationsListView);


                    MessageBox.Show("Заявка успешно создана");
                }
            }
            else
            {
                MessageBox.Show("Ошибка в веденных данных");
            }
        }
    }
}
