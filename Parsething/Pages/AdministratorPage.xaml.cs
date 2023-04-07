
namespace Parsething.Pages;

public partial class AdministratorPage : Page
{
    private Frame MainFrame { get; set; } = null!;

    public AdministratorPage()
    {
        InitializeComponent();

        ParsethingContext db = new();
        Parsed.Text = Convert.ToString(GET.ProcurementsCountBy("Получен", GET.KindOf.ProcurementState));
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.Message, "лешаgей");
        }
    }
}
//DatabaseLibrary.Entities.ProcurementProperties.Procurement procurementUpdate = new DatabaseLibrary.Entities.ProcurementProperties.Procurement();
//procurementUpdate.RequestUri = "https://zakupki.gov.ru/epz/order/notice/notice223/common-info.html?noticeInfoId=15080229";
//procurementUpdate.Number = "32312229220";
//procurementUpdate.LawId = db.Laws.Where(e => e.Number == "44-ФЗ").FirstOrDefault().Id;
//procurementUpdate.Object = "Расходные материалы для лаборатории (Урискан)";
//procurementUpdate.InitialPrice = Convert.ToDecimal("407000,00");
//procurementUpdate.OrganizationId = db.Organizations.Where(e => e.Name == "АДМИНИСТРАЦИЯ МИЛЬКОВСКОГО МУНИЦИПАЛЬНОГО РАЙОНА").Where(e => e.PostalAddress == "Российская Федерация, 684300, Камчатский край, Мильковский р-н, Мильково с, УЛИЦА ПОБЕДЫ, 8").FirstOrDefault().Id;
//procurementUpdate.MethodId = db.Methods.Where(e => e.Text == "Аукцион в электронной форме").FirstOrDefault().Id;
//procurementUpdate.PlatformId = db.Platforms.Where(e => e.Name == "ЭТП ТЭК-Торг").Where(e => e.Address == "http://www.tektorg.ru/").FirstOrDefault().Id;
//procurementUpdate.Location = "689000, АВТОНОМНЫЙ ОКРУГ ЧУКОТСКИЙ, Г. АНАДЫРЬ, УЛ. БЕРИНГА, дом Д.7";
//procurementUpdate.StartDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.Deadline = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.TimeZoneId = db.TimeZones.Where(e => e.Offset == "МСК+6").FirstOrDefault().Id;
//procurementUpdate.Securing = "752,77 Российский рубль";
//procurementUpdate.Enforcement = "7 527,71 Российский рубль (10 %)";
//procurementUpdate.Warranty = "";
//procurementUpdate.RegionId = db.Regions.Where(e => e.Title == "Москва").Where(e => e.Distance == 500).FirstOrDefault().Id;
//procurementUpdate.OrganizationContractName = "Организация";
//procurementUpdate.OrganizationContractPostalAddress = "1";
//procurementUpdate.ContactPerson = "2";
//procurementUpdate.ContactPhone = "3";
//procurementUpdate.DeliveryDetails = "4";
//procurementUpdate.DeadlineAndType = "5";
//procurementUpdate.DeliveryDeadline = "6";
//procurementUpdate.AcceptanceDeadline = "7";
//procurementUpdate.ContractDeadline = "8";
//procurementUpdate.Indefinitely = true;
//procurementUpdate.AnotherDeadline = "9";
//procurementUpdate.DeadlineAndOrder = "10";
//procurementUpdate.RepresentativeTypeId = db.RepresentativeTypes.Where(e => e.Kind == "Нужен").FirstOrDefault().Id;
//procurementUpdate.CommissioningWorksId = db.CommisioningWorks.Where(e => e.Kind == "Нужны").FirstOrDefault().Id;
//procurementUpdate.PlaceCount = 11;
//procurementUpdate.FinesAndPennies = "12";
//procurementUpdate.PenniesPerDay = "13";
//procurementUpdate.TerminationConditions = "14";
//procurementUpdate.EliminationDeadline = "15";
//procurementUpdate.GuaranteePeriod = "16";
//procurementUpdate.Inn = "17";
//procurementUpdate.ContractNumber = "18";
////employeeId
//procurementUpdate.AssemblyNeed = true;
//procurementUpdate.MinopttorgId = db.Minopttorgs.Where(e => e.Name == "3logic").FirstOrDefault().Id;
//procurementUpdate.LegalEntityId = db.LegalEntities.Where(e => e.Name == "ГРИНТЕХ").FirstOrDefault().Id;
//procurementUpdate.Applications = false;
//procurementUpdate.Bet = Convert.ToDecimal("407000,00");
//procurementUpdate.MinimalPrice = Convert.ToDecimal("407000,00");
//procurementUpdate.ContractAmount = Convert.ToDecimal("407000,00");
//procurementUpdate.ReserveContractAmount = Convert.ToDecimal("407000,00");
//procurementUpdate.ProtocolDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.ShipmentPlanId = db.ShipmentPlans.Where(e => e.Kind == "Текущая").FirstOrDefault().Id;
//procurementUpdate.WaitingList = true;
//procurementUpdate.Calculating = true;
//procurementUpdate.Purchase = false;
//procurementUpdate.ExecutionStateId = db.ExecutionStates.Where(e => e.Kind == "Не требуется").FirstOrDefault().Id;
//procurementUpdate.WarrantyStateId = db.WarrantyStates.Where(e => e.Kind == "Не требуется").FirstOrDefault().Id;
//procurementUpdate.SigningDeadline = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.SigningDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.ConclusionDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.ActualDeliveryDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.DepartureDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.DeliveryDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.MaxAcceptanceDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.CorrectionDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.ActDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.MaxDueDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.ClosingDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.RealDueDate = Convert.ToDateTime("2023-03-28 02:31:00");
//procurementUpdate.Amount = Convert.ToDecimal("407000,00");
//procurementUpdate.SignedOriginalId = db.SignedOriginals.Where(e => e.Kind == "ПО (Получены)").FirstOrDefault().Id;
//procurementUpdate.Judgment = true;
//procurementUpdate.Fas = false;
//procurementUpdate.ProcurementStateId = db.ProcurementStates.Where(e => e.Kind == "Новый").FirstOrDefault().Id;
//DatabaseLibrary.Functions.PULL.ProcurementUpdate(procurementUpdate);