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
    /// Логика взаимодействия для PurchaserPage.xaml
    /// </summary>
    public partial class PurchaserPage : Page
    {
        private Frame MainFrame { get; set; } = null!;
        private List<Procurement>? ProcurementsWonPartOne { get; set; }
        private List<Procurement>? ProcurementsWonPartTwo { get; set; }
        private List<Procurement>? ProcurementsAcceptance { get; set; }
        private List<Procurement>? ProcurementsPreviousWeek { get; set; }
        private List<Procurement>? ProcurementsThisWeek { get; set; }
        private List<Procurement>? ProcurementsNextWeek { get; set; }
        private List<Procurement>? ProcurementsAWeekLater { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsProblem { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsInWork { get; set; }
        private List<ComponentCalculation>? ComponentCalculationsAgreed { get; set; }

        public PurchaserPage() =>
            InitializeComponent();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }

            ProcurementsWonPartOne = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState);
            if (ProcurementsWonPartOne != null)
                WonPartOne.Text = ProcurementsWonPartOne.Count.ToString();

            ProcurementsWonPartTwo = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
            if (ProcurementsWonPartTwo != null)
                WonPartTwo.Text = ProcurementsWonPartTwo.Count.ToString();

            ComponentCalculationsProblem = GET.View.ComponentCalculationsBy("Проблема").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsProblem != null)
                Problem.Text = ComponentCalculationsProblem.Count.ToString();

            ComponentCalculationsInWork = GET.View.ComponentCalculationsBy("ТО: Обработка").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsInWork != null)
                InWork.Text = ComponentCalculationsInWork.Count.ToString();

            ComponentCalculationsAgreed = GET.View.ComponentCalculationsBy("ТО: Согласовано").Distinct(new Functions.MyClassComparer()).ToList();
            if (ComponentCalculationsAgreed != null)
                Agreed.Text = ComponentCalculationsAgreed.Count.ToString();

            ProcurementsAcceptance = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
            if (ProcurementsAcceptance != null)
                Acceptance.Text = ProcurementsAcceptance.Count.ToString();

            ProcurementsPreviousWeek = GET.View.ProcurementsBy("Предыдущая", GET.KindOf.ShipmentPlane);
            if (ProcurementsPreviousWeek != null)
                PreviousWeek.Text = ProcurementsPreviousWeek.Count.ToString();

            ProcurementsThisWeek = GET.View.ProcurementsBy("Текущая", GET.KindOf.ShipmentPlane);
            if (ProcurementsThisWeek != null)
            {
                View.ItemsSource = ProcurementsThisWeek;
                ThisWeek.Text = ProcurementsThisWeek.Count.ToString();
            }

            ProcurementsNextWeek = GET.View.ProcurementsBy("Следующая", GET.KindOf.ShipmentPlane);
            if (ProcurementsNextWeek != null)
                NextWeek.Text = ProcurementsNextWeek.Count.ToString();

            ProcurementsAWeekLater = GET.View.ProcurementsBy("Через одну", GET.KindOf.ShipmentPlane);
            if (ProcurementsAWeekLater != null)
                AWeekLater.Text = ProcurementsAWeekLater.Count.ToString();
            
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsNextWeek;
        }

        private void ThisWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsThisWeek;
        }

        private void AcceptanceButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsAcceptance;
        }

        private void WonPartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsWonPartTwo;
        }

        private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsWonPartOne;
        }
        private void ProblemButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Visible;
            View.Visibility = Visibility.Hidden;
            ViewComponents.ItemsSource = ComponentCalculationsProblem;
        }

        private void InWorkButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Visible;
            View.Visibility = Visibility.Hidden;
            ViewComponents.ItemsSource = ComponentCalculationsInWork;
        }

        private void AgreedButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Visible;
            View.Visibility = Visibility.Hidden;
            ViewComponents.ItemsSource = ComponentCalculationsAgreed;
        }
        private void AWeekLaterButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsAWeekLater;
        }
        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ViewComponents.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
            View.ItemsSource = ProcurementsPreviousWeek;
        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            ListView myListView = this.FindName("View") as ListView;
            Procurement procurement = myListView.SelectedItem as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement));
        }

        private void EditProcurementComponent_Click(object sender, RoutedEventArgs e)
        {
            ListView myListView = this.FindName("ViewComponents") as ListView;
            ComponentCalculation componentCalculation = myListView.SelectedItem as ComponentCalculation;
            if (componentCalculation != null)
            { 
                Procurement procurement = componentCalculation.Procurement;
            _ = MainFrame.Navigate(new CardOfProcurement(procurement));
            }
            
        }
    }
    
}
