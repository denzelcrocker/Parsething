using Microsoft.VisualBasic;
using System.Net;
using System.Windows.Media;
using System.Xml.Linq;
using static DatabaseLibrary.Queries.GET;

namespace Parsething.Pages;

public partial class AdministratorPage : Page
{
    private Frame MainFrame { get; set; } = null!;

    private List<Procurement>? Procurements = new List<Procurement>();
    private List<ProcurementsEmployee>? ProcurementsEmployees = new List<ProcurementsEmployee>();


    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupingsNew { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesCalculatorsGroupingsDrawUp { get; set; }

    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesEPSpecialistGroupings { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesManagersGroupings { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsMethodsGroupings { get; set; }

    private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
    private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
    private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

    public AdministratorPage() =>
        InitializeComponent();

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {

        try
        {
            MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
        }
        catch { }
       
        int countOfCalculationsNew = 0;
        int countOfCalculationsDrawUp = 0;
        int countOfMethods = 0;
        int countOfSended = 0;
        int countOfManagers = 0;
        Parsed.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Получен", GET.KindOf.ProcurementState)); // Спаршены

        Unsorted.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Неразобранный", GET.KindOf.ProcurementState)); // Неразобранные

        Retreat.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState)); // Отбой

        CalculationQueue.Text  =  Convert.ToString(GET.Aggregate.ProcurementsQueueCount());// Очередь расчета

        Sended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", GET.KindOf.ProcurementState)); // Отправлены

        Bargaining.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", false, GET.KindOf.ResultDate)); // Торги


        QuotesCombobox.Items.Clear();
        QuotesCombobox.Text = "Сп-бы опр-я:";
        ProcurementsMethodsGroupings = GET.View.ProcurementsGroupByMethod();
        foreach (var item in ProcurementsMethodsGroupings)
        {
            countOfMethods += item.CountOfProcurements;
        }
        Quotes.Text = countOfMethods.ToString(); // Котировки (общее количество)
        foreach (var item in ProcurementsMethodsGroupings)
        {
            QuotesCombobox.Items.Add(item); // Котировки (по методам)
        }// Котировки

        OverdueSended.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отправлен", true, GET.KindOf.ResultDate)); // Просрочены

        Cancellation.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отмена", GET.KindOf.ProcurementState)); // Отменены

        Rejected.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отклонен", GET.KindOf.ProcurementState)); // Отклонены

        Lost.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Проигран", GET.KindOf.ProcurementState)); // Проиграны

        New.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Новый", GET.KindOf.ProcurementState)); // Новый

        Calculated.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Посчитан", GET.KindOf.ProcurementState)); // Посчитан

        CalculationsCombobox.Items.Clear();
        CalculationsCombobox.Text = "Расчет:";
        ProcurementsEmployeesCalculatorsGroupingsNew = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов", "Новый","","", "");
        foreach (var item in ProcurementsEmployeesCalculatorsGroupingsNew)
        {
            countOfCalculationsNew += item.CountOfProcurements;
        }
        CalculationsOverAll.Text = countOfCalculationsNew.ToString(); // Расчет (общее количество)
        foreach (var item in ProcurementsEmployeesCalculatorsGroupingsNew)
        {
            CalculationsCombobox.Items.Add(item); // Расчет (по сотрудникам)
        }

        DrawUp.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформить", GET.KindOf.ProcurementState)); // Оформить

        DrawUpCombobox.Items.Clear();
        DrawUpCombobox.Text = "Оформление:";
        ProcurementsEmployeesCalculatorsGroupingsDrawUp = GET.View.ProcurementsEmployeesGroupBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов", "Оформить","","", "");
        foreach (var item in ProcurementsEmployeesCalculatorsGroupingsDrawUp)
        {
            countOfCalculationsDrawUp += item.CountOfProcurements;
        }
        DrawUpOverAll.Text = countOfCalculationsDrawUp.ToString(); // Оформление (общее количество)
        foreach (var item in ProcurementsEmployeesCalculatorsGroupingsDrawUp)
        {
            DrawUpCombobox.Items.Add(item); // Оформление (по сотрудникам)
        }// Оформление

        Issued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", GET.KindOf.ProcurementState)); // Оформллены

        ForSend.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", false, GET.KindOf.Deadline));  // К отправке

        OverdueIssued.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Оформлен", true, GET.KindOf.Deadline));// Просрочены

        SendingCombobox.Items.Clear();
        SendingCombobox.Text = "Отправка:";
        ProcurementsEmployeesEPSpecialistGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист по работе с электронными площадками","","", "Отправлен","","", "");
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

        ManagersCombobox.Items.Clear();
        ManagersCombobox.Text = "Менеджеры:";
        ProcurementsEmployeesManagersGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист тендерного отдела", "Руководитель тендерного отдела", "Заместитель руководителя тендреного отдела", "Выигран 1ч", "Выигран 2ч", "Приемка", "Принят");
        foreach (var item in ProcurementsEmployeesManagersGroupings)
        {
            countOfManagers += item.CountOfProcurements;
        }
        ManagersOverAll.Text = countOfManagers.ToString(); // Расчет (общее количество)
        foreach (var item in ProcurementsEmployeesManagersGroupings)
        {
            ManagersCombobox.Items.Add(item); // Расчет (по сотрудникам)
        }// Менеджеры выпадающий список

        ContractYes.Text = GET.Aggregate.ProcurementsCountBy("", true, GET.KindOf.ContractConclusion).ToString(); // Контракт Подписан

        ContractNo.Text = GET.Aggregate.ProcurementsCountBy("", false, GET.KindOf.ContractConclusion).ToString();// Контракт Не подписан

        ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList(); // Проблема
        if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList(); // В работе
        if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList(); // Согласовано
        if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

        PreviousWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Предыдущая", GET.KindOf.ShipmentPlane));// Предыдущая неделя отгрузки

        ThisWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Текущая", GET.KindOf.ShipmentPlane));// Текущая неделя отгрузки

        NextWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Следующая", GET.KindOf.ShipmentPlane));// Следующая неделя отгрузки

        AWeekLater.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Через одну", GET.KindOf.ShipmentPlane));// Отгрузка через неделю

        ApproveCalculatingYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Calculating).ToString(); // Проверка расчета проведена
        
        ApproveCalculatingNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Calculating).ToString(); // Проверка расчета не проведена

        ApprovePurchaseYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Purchase).ToString(); // Проверка закупки проведена
        
        ApprovePurchaseNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Purchase).ToString(); // Проверка закупки не проведена

        Received.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Принят", GET.KindOf.ProcurementState));// Принят

        await UpdateRatesFromCBRAsync();


        // Рублей в обороте

        Acceptance.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.ProcurementState)); // Приемка

        // Частичная отправка

        OnTheFix.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", KindOf.CorrectionDate)); // На исправлении

        NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

        NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

        NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

        Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

        FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС
    }
    private async Task UpdateRatesFromCBRAsync()
    {
        try
        {
            WebClient client = new WebClient();
            var xml = await client.DownloadStringTaskAsync("https://www.cbr-xml-daily.ru/daily.xml");
            XDocument xdoc = XDocument.Parse(xml);
            var el = xdoc.Element("ValCurs").Elements("Valute");
            string dollar = el.Where(x => x.Attribute("ID").Value == "R01235").Select(x => x.Element("Value").Value).FirstOrDefault();
            dollar = dollar.RemoveEnd(2) + " ₽";
            RateForCentralBank.Text = dollar;
        }
        catch { }
    }
    private void UnsortedButton_Click(object sender, RoutedEventArgs e) // неразобранные 
    {
        Procurements = GET.View.ProcurementsBy("Неразобранный", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void RetreatButton_Click(object sender, RoutedEventArgs e) // отбой
    {
        Procurements = GET.View.ProcurementsBy("Отбой", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void CalculationQueueButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsQueue();
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void SendedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Отправлен", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void BargainingButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Отправлен", false, GET.KindOf.ResultDate);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Отправлен", true, GET.KindOf.ResultDate);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void CancellationButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Отмена", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void RejectedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Отклонен", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void LostButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Проигран", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Новый", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void CalculatedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Посчитан", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void DrawUpButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Оформить", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void IssuedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Оформлен", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ForSendButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Оформлен", false, GET.KindOf.Deadline);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void OverdueIssuedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Оформлен", true, GET.KindOf.Deadline);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void WonByApplicationsButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("", GET.KindOf.Applications);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ProblemButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsProblem);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void InWorkButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsInWork);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void AgreedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsAgreed);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void NextWeekButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }
    private void ApproveCalculatingYesButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(true, GET.KindOf.Calculating);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ApproveCalculatingNoButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(false, GET.KindOf.Calculating);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ApprovePurchaseYesButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(true, GET.KindOf.Purchase);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ApprovePurchaseNoButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(false, GET.KindOf.Purchase);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ReceivedButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Принят", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }
    private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void NotPaidButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsNotPaid();
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(false);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(true);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void JudgementButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(GET.KindOf.Judgement);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void FASButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy(GET.KindOf.FAS);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ContractYesButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("", true, GET.KindOf.ContractConclusion);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void ContractNoButton_Click(object sender, RoutedEventArgs e)
    {
        Procurements = GET.View.ProcurementsBy("", false, GET.KindOf.ContractConclusion);
        if (Procurements != null)
            MainFrame.Navigate(new SearchPage(Procurements));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Charts());
    }

    private void GoToCalculationsButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedGrouping = (sender as FrameworkElement)?.DataContext as ProcurementsEmployeesGrouping;
        if (selectedGrouping != null)
        {
            MainFrame.Navigate(new SearchPage(selectedGrouping.Procurements));
        }
    }

    private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
        {
            MainFrame.Navigate(new SearchPage(selectedGrouping.Procurements));
        }
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