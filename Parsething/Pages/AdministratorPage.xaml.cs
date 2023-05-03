using System.Net;
using System.Windows.Media;
using System.Xml.Linq;

namespace Parsething.Pages;

public partial class AdministratorPage : Page
{
    private Frame MainFrame { get; set; } = null!;
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupings { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesEPSpecialistGroupings { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesManagersGroupings { get; set; }
    private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
    private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
    private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

    public AdministratorPage() =>
        InitializeComponent();
    

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
        }
        catch { }
        int countOfCalculations = 0;
        int countOfSended = 0;
        int countOfManagers = 0;

        Parsed.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Получен", GET.KindOf.ProcurementState)); // Спаршены

        Unsorted.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Неразобранный", GET.KindOf.ProcurementState)); // Неразобранные

        Retreat.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState)); // Отбой

        //if (CalculationQueue.Text == "0")
        //{
        //    CalculationQueue.Foreground = Brushes.Red;
        //}

        // Очередь расчета

        Sended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", GET.KindOf.ProcurementState)); // Отправлены

        Bargaining.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", false, GET.KindOf.Deadline)); // Торги

        // Котировки

        OverdueSended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", true, GET.KindOf.Deadline)); // Просрочены

        Cancellation.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отмена", GET.KindOf.ProcurementState)); // Отменены

        Rejected.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отклонен", GET.KindOf.ProcurementState)); // Отклонены

        Lost.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Проигран", GET.KindOf.ProcurementState)); // Проиграны

        New.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Новый", GET.KindOf.ProcurementState)); // Новый

        Calculated.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Посчитан", GET.KindOf.ProcurementState)); // Посчитан

        ProcurementsEmployeesCalculatorsGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов");
        foreach (var item in ProcurementsEmployeesCalculatorsGroupings)
        {
            countOfCalculations += item.CountOfProcurements;
        }
        CalculationsOverAll.Text = countOfCalculations.ToString(); // Расчет (общее количество)
        foreach (var item in ProcurementsEmployeesCalculatorsGroupings)
        {
            CalculationsCombobox.Items.Add(item); // Расчет (по сотрудникам)
        }

        RetreatCalculate.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState)); // Отбой

        DrawUp.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформить", GET.KindOf.ProcurementState)); // Оформить

        // Оформление Выпадающий список

        Issued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", GET.KindOf.ProcurementState)); // Оформллены

        ForSend.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", false, GET.KindOf.StartDate));  // К отправке

        OverdueIssued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", true, GET.KindOf.StartDate));// Просрочены

        ProcurementsEmployeesEPSpecialistGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист по работе с электронными площадками");
        foreach (var item in ProcurementsEmployeesEPSpecialistGroupings)
        {
            countOfSended += item.CountOfProcurements;
        }
        SendingOverAll.Text = countOfSended.ToString(); // Подачицы (общее количество)
        foreach (var item in ProcurementsEmployeesEPSpecialistGroupings)
        {
            SendingCombobox.Items.Add(item); // Подачицы (по сотрудникам)
        } // Отправка

        WonPartOne.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState));// Выигран 1ч

        WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState));// Выигран 2ч

        WonByApplications.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)); // По заявкам

        WonByOverAll.Text = (GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState) + GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState) + GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)).ToString(); // Выиграны всего

        ProcurementsEmployeesManagersGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист тендерного отдела");
        foreach (var item in ProcurementsEmployeesManagersGroupings)
        {
            countOfManagers += item.CountOfProcurements;
        }
        ManagersOverAll.Text = countOfManagers.ToString(); // Расчет (общее количество)
        foreach (var item in ProcurementsEmployeesManagersGroupings)
        {
            ManagersCombobox.Items.Add(item); // Расчет (по сотрудникам)
        }// Менеджеры выпадающий список

        // Контракт
        ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList(); // Проблема
        if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList(); // В работе
        if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList(); // Согласовано
        if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

        ThisWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Текущая", GET.KindOf.ShipmentPlane));// Текущая неделя отгрузки

        NextWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Следующая", GET.KindOf.ShipmentPlane));// Следующая неделя отгрузки

        Received.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Принят", GET.KindOf.ProcurementState));// Принят

        WebClient client = new WebClient();
        var xml = client.DownloadString("https://www.cbr-xml-daily.ru/daily.xml");
        XDocument xdoc = XDocument.Parse(xml);
        var el = xdoc.Element("ValCurs").Elements("Valute");
        string dollar = el.Where(x => x.Attribute("ID").Value == "R01235").Select(x => x.Element("Value").Value).FirstOrDefault();
        dollar = dollar.RemoveEnd(2) + " ₽";
        RateForCentralBank.Text = dollar;

        // Рублей в обороте

        // Приемка

        // Частичная отправка

        // На исправлении

        NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

        NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

        NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

        Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

        FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС
    }

}
public static class StringExtensions
{
    public static String RemoveEnd(this String str, int len)
    {
        if (str.Length < len)
        {
            return string.Empty;
        }

        return str[..^len];
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