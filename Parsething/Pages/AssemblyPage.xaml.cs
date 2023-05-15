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

        private List<Procurement>? Procurements = new List<Procurement>();

        public AssemblyPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            Procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
            if (Procurements != null)
                View.ItemsSource = Procurements;

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

            WonPartTwo.Text = Convert.ToString(GET.Aggregate.ProcurementsCountBy("Выигран 2ч", GET.KindOf.ProcurementState)); // Выигран 2ч
        }

        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsProblem)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsInWork)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            View.ItemsSource = null;
            Procurements.Clear();
            foreach (ComponentCalculation componentCalculation in ComponentCalculationsAgreed)
            {
                Procurements.Add(componentCalculation.Procurement);
            }
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            Procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
            if (Procurements != null)
                View.ItemsSource = Procurements;
        }
        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement, null, false));
        }
        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                string url = procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, null, false, false));
        }
    }
}
