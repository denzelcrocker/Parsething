using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using DatabaseLibrary.Entities.ProcurementProperties;
using DatabaseLibrary.Queries;
using Parsething.Classes;
using Parsething.Windows;
using static DatabaseLibrary.Queries.GET;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для AssemblyPage.xaml
    /// </summary>
    public partial class AssemblyPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }


        public AssemblyPage()
        {
            InitializeComponent();

            ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList(); // Проблема
            if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList(); // В работе
            if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList(); // Согласовано
            if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

            OnTheFix.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Приемка", KindOf.CorrectionDate)); // На исправлении

            PreviousWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Предыдущая", GET.KindOf.ShipmentPlane));// Предыдущая неделя отгрузки

            ThisWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Текущая", GET.KindOf.ShipmentPlane));// Текущая неделя отгрузки

            NextWeek.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Следующая", GET.KindOf.ShipmentPlane));// Следующая неделя отгрузки

            AWeekLater.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Через одну", GET.KindOf.ShipmentPlane));// Отгрузка через неделю

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState)); // Выигран 2ч
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            NavigationState.AddLastSelectedProcurement(View);
        }

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsProblem)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsInWork)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            GlobalUsingValues.Instance.Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsAgreed)
            {
                GlobalUsingValues.Instance.AddProcurement(componentCalculation.Procurement);
            }
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void OnTheFixButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if(GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            var procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements != null)
            {
                GET.View.PopulateComponentStates(GlobalUsingValues.Instance.Procurements);
                View.ItemsSource = GlobalUsingValues.Instance.Procurements;
            }
        }
        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            {
                NavigationState.LastSelectedProcurement = procurement;
                _ = MainFrame.Navigate(new CardOfProcurement(procurement, false));
            }
        }
        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            {
                string url = procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            {
                NavigationState.LastSelectedProcurement = procurement;

                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, false, false));
            }
        }

        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
            if (procurement != null)
            { 
                AssemblyMap assemblyMap = new AssemblyMap(procurement);
                assemblyMap.Show();
            }
        }

    }
}
