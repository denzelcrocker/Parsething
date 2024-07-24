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

        private List<Procurement>? Procurements = new List<Procurement>();

        public HeadsOfManagersPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try{ MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            int countOfManagers = 0;
            int countOfMethods = 0;


            WonPartOne.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 1ч", GET.KindOf.ProcurementState));// Выигран 1ч

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState));// Выигран 2ч

            WonByApplications.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("", GET.KindOf.Applications)); // По заявкам

            ManagersCombobox.Text = "Менеджеры:";
            ManagersCombobox.Items.Clear();

            ProcurementsEmployeesManagersGroupings = GET.View.ProcurementsEmployeesGroupBy("Специалист тендерного отдела", "Руководитель тендерного отдела", "Заместитель руководителя тендреного отдела", "Выигран 1ч", "Выигран 2ч", "Приемка", "Принят");
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

            Cancellation.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отмена", GET.KindOf.ProcurementState)); // Отменены

            Rejected.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Отклонен", GET.KindOf.ProcurementState)); // Отклонены

            Lost.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Проигран", GET.KindOf.ProcurementState)); // Проиграны

            Acceptance.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.ProcurementState)); // Приемка

            // Частичная отправка

            OnTheFix.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", GET.KindOf.CorrectionDate)); // На исправлении

            NotPaidOnTime.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(false)); // В срок

            NotPaidDelay.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(true));// Просрочка

            NotPaid.Text = (GET.Aggregate.ProcurementsCountBy(true) + GET.Aggregate.ProcurementsCountBy(false)).ToString(); // Не оплачены

            Judgement.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.Judgement)); // Суд

            FAS.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy(GET.KindOf.FAS)); // ФАС

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

            Received.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Принят", GET.KindOf.ProcurementState));// Принят

            ApproveCalculatingYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Calculating).ToString(); // Проверка расчета проведена

            ApproveCalculatingNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Calculating).ToString(); // Проверка расчета не проведена

            ApprovePurchaseYes.Text = GET.Aggregate.ProcurementsCountBy(true, KindOf.Purchase).ToString(); // Проверка закупки проведена

            ApprovePurchaseNo.Text = GET.Aggregate.ProcurementsCountBy(false, KindOf.Purchase).ToString(); // Проверка закупки не проведена

        }

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsProblem)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsInWork)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsAgreed)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
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

        private void ManagersQueueButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsManagersQueue();
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
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

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void PartialAcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке");
        }

        private void ReceivedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Принят", GET.KindOf.ProcurementState);
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
        private void ManagersCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
            {
                MainFrame.Navigate(new SearchPage(selectedGrouping.Procurements));
            }
        }

        private void SendedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", GET.KindOf.ProcurementState);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void BargainingButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", false, GET.KindOf.Deadline);
            if (Procurements != null)
                MainFrame.Navigate(new SearchPage(Procurements));
        }

        private void QuotesCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ProcurementsEmployeesGrouping selectedGrouping)
            {
                MainFrame.Navigate(new SearchPage(selectedGrouping.Procurements));
            }
        }

        private void OverdueSendedButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Отправлен", true, GET.KindOf.Deadline);
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
