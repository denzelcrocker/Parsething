using Parsething.Classes;
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
using static DatabaseLibrary.Queries.GET;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для HeadsOfManagersPage.xaml
    /// </summary>
    public partial class HeadsOfManagersPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesManagersGroupings { get; set; }
        private List<GET.ProcurementsEmployeesGrouping>? ProcurementsMethodsGroupings { get; set; }


        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

        private DateTime StartDate = new DateTime();

        public HeadsOfManagersPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try{ MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            var globalUsingValues = GlobalUsingValues.Instance;
            StartDate = globalUsingValues.StartDate;

            int countOfManagers = 0;
            int countOfMethods = 0;


            WonPartOne.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState));// Выигран 1ч

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState));// Выигран 2ч

            WonByApplications.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)); // По заявкам

            ManagersCombobox.Text = "Менеджеры:";
            ManagersCombobox.Items.Clear();

            ProcurementsEmployeesManagersGroupings = GET.View.ProcurementsEmployeesGroupBy(
                new string[] {
                    "Специалист тендерного отдела",
                    "Руководитель тендерного отдела",
                    "Заместитель руководителя тендерного отдела"
                },
                actionType: "Appoint",
                procurementStates: new string[] {
                    "Выигран 1ч",
                    "Выигран 2ч",
                    "Приемка",
                    "Принят",
                    "Оформлен",
                    "Отправлен"
                }
            );
            foreach (var item in ProcurementsEmployeesManagersGroupings)
            {
                countOfManagers += item.CountOfProcurements;
            }
            ManagersOverAll.Text = countOfManagers.ToString(); // Расчет (общее количество)
            foreach (var item in ProcurementsEmployeesManagersGroupings)
            {
                ManagersCombobox.Items.Add(item); // Расчет (по сотрудникам)
            }// Менеджеры выпадающий список}

            ContractYes.Text = GET.Aggregate.ProcurementsCountBy("", true, GET.KindOf.ContractConclusion).ToString(); // Контракт Подписан

            ContractNo.Text = GET.Aggregate.ProcurementsCountBy("", false, GET.KindOf.ContractConclusion).ToString() ;// Контракт Не подписан

            ManagersQueue.Text = GET.Aggregate.ProcurementsManagersQueueCount().ToString(); // Не назначены

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

            Cancellation.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отмена", StartDate)); // Отменены

            Rejected.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отклонен", StartDate)); // Отклонены

            Lost.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Проигран", StartDate)); // Проиграны

            Acceptance.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.ProcurementState)); // Приемка

            // Частичная отправка

            OnTheFix.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.CorrectionDate)); // На исправлении

            NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

            NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

            UnpaidPennies.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.UnpaidPennies));// Неоплаченные пени

            NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(GET.KindOf.UnpaidPennies) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

            Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

            FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС

            ClaimWorks.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.ClaimWorks)); // Претензионные работы

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

            Shipped.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отгружен", GET.KindOf.ProcurementState));// Отгружен

            Received.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Принят", StartDate));// Принят

            ApproveCalculatingYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Calculating).ToString(); // Проверка расчета проведена

            ApproveCalculatingNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Calculating).ToString(); // Проверка расчета не проведена

            ApprovePurchaseYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Purchase).ToString(); // Проверка закупки проведена

            ApprovePurchaseNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Purchase).ToString(); // Проверка закупки не проведена

        }

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsProblem) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsInWork) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = Functions.Conversion.ConponentCalculationsConversion(ComponentCalculationsAgreed) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void WonByApplicationsButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("", GET.KindOf.Applications) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ContractYesButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("", true, GET.KindOf.ContractConclusion) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ContractNoButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("", false, GET.KindOf.ContractConclusion) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ManagersQueueButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsManagersQueue() ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NotPaidButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsNotPaid() ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NotPaidOnTimeButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(false) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void NotPaidDelayButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(true) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void UnpaidPenniesButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(GET.KindOf.UnpaidPennies) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void JudgementButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(GET.KindOf.Judgement) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void FASButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(GET.KindOf.FAS) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }
        private void ClaimWorksButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(GET.KindOf.ClaimWorks) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void PartialAcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке");
        }

        private void ShippedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отгружен", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ReceivedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Принят", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }
        private void ApproveCalculatingYesButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(true, GET.KindOf.Calculating) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApproveCalculatingNoButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(false, GET.KindOf.Calculating) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApprovePurchaseYesButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(true, GET.KindOf.Purchase) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void ApprovePurchaseNoButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy(false, GET.KindOf.Purchase) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }
        private void ManagersCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
            {
                var procurements = selectedGrouping.Procurements ?? new List<Procurement>();
                GlobalUsingValues.Instance.AddProcurements(procurements);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void SendedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отправлен", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void BargainingButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отправлен", false, GET.KindOf.ResultDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void QuotesCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
            {
                var procurements = selectedGrouping.Procurements ?? new List<Procurement>();
                GlobalUsingValues.Instance.AddProcurements(procurements);
            }
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отправлен", true, GET.KindOf.ResultDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void CancellationButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отмена", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void RejectedButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Отклонен", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void LostButton_Click(object sender, RoutedEventArgs e)
        {
            var procurements = GET.View.ProcurementsBy("Проигран", StartDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as FrameworkElement;

            if (image != null)
            {
                var parameter = image.Tag as string;
                ToolTipHelper.SetToolTip(image, parameter);
            }
        }
    }
}
