using Microsoft.EntityFrameworkCore;
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
using Parsething.Classes;
using Parsething.Windows;
using DatabaseLibrary.Entities.ProcurementProperties;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Law>? Laws { get; set; }
        private List<ProcurementState>? ProcurementStates { get; set; }

        private List<Procurement>? FoundProcurements { get; set; }

        private List<Procurement>? Procurements { get; set; }

        public SearchPage(List<Procurement>? procurements)
        {
            InitializeComponent();

            if (SearchCriteria.Instance.Law != null && SearchCriteria.Instance.ProcurementNumber != null && SearchCriteria.Instance.Law != null && SearchCriteria.Instance.ProcurementState != null && SearchCriteria.Instance.INN != null)
            {
                procurements = GET.View.ProcurementsBy(SearchCriteria.Instance.ProcurementId, SearchCriteria.Instance.ProcurementNumber, SearchCriteria.Instance.Law, SearchCriteria.Instance.ProcurementState, SearchCriteria.Instance.INN);
                SearchLV.ItemsSource = procurements;
            }
            else if (procurements != null)
            {
                SearchLV.ItemsSource = procurements;
            }
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

            ProcurementStates = GET.View.ProcurementStates();
            ProcurementState.ItemsSource = ProcurementStates;
            Procurements = procurements;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement, Procurements,true));
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                string url = procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.Instance.ClearData();

            SearchLV.ItemsSource = null;
            if(FoundProcurements != null)
            FoundProcurements.Clear();
            int id = 0;
            if(SearchId.Text != "")
            id = Convert.ToInt32(SearchId.Text);
            string number = SearchNumber.Text;
            string law = Law.Text;
            string procurementState = ProcurementState.Text;
            string inn = SearchINN.Text;

            if (id == 0 && number == "" && law == "" && procurementState == "" && inn == "")
            { }
            else
            {
                FoundProcurements = GET.View.ProcurementsBy(id, number, law, procurementState, inn);
                SearchLV.ItemsSource = FoundProcurements;
                Procurements = FoundProcurements;
            }

            SearchCriteria.Instance.ProcurementId = id;
            SearchCriteria.Instance.ProcurementNumber = number;
            SearchCriteria.Instance.Law = law;
            SearchCriteria.Instance.ProcurementState = procurementState;
            SearchCriteria.Instance.INN = inn;
        }

        private void Search_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement,Procurements,true,true));
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, Procurements, false, true));
        }
        
        private void OverallInfo_Click(object sender, RoutedEventArgs e)
        {
            //decimal? overallAmount = 0;
            //int index = 0;
            //decimal?[] profitCalculate = new decimal?[] { };
            //decimal?[] profitReal = new decimal?[] { };
            //OverallInfoPopUp.IsOpen = !OverallInfoPopUp.IsOpen;
            //if (Procurements != null)
            //{
            //    OverallCount.Text = Procurements.Count.ToString();
            //    foreach (Procurement procurement in Procurements)
            //    {
            //        if (procurement.ContractAmount != null && procurement.ReserveContractAmount == null)
            //        {
            //            overallAmount += procurement.ContractAmount;
            //            profitReal[index] = procurement.ContractAmount - procurement.PurchaseAmount;
            //        }
            //        else if (procurement.ReserveContractAmount != null)
            //        {
            //            overallAmount += procurement.ReserveContractAmount;
            //            profitReal[index] = procurement.ReserveContractAmount - procurement.PurchaseAmount;
            //        }
            //        profitCalculate[index] = procurement.ContractAmount - procurement.CalculatingAmount;

            //        index++;
            //    }
            //    OverallAmount.Text = ((decimal)overallAmount).ToString("N2") + " р.";
            //    AvgCalculationProfit.Text = ((decimal)profitCalculate.Average()).ToString("N2") + " р.";
            //    AvgPurchaseProfit.Text = ((decimal)profitReal.Average()).ToString("N2") + " р.";
            //}
        }
        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            Procurement? procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
            {
                AssemblyMap assemblyMap = new AssemblyMap(procurement);
                assemblyMap.Show();
            }
        }
        List<Procurement> procurements;
        private void SupplyMonitoringButton_Click(object sender, RoutedEventArgs e)
        {
            List<Procurement> procurements;
            if (SearchLV.Items.Count > 0)
            {
                procurements = SearchLV.ItemsSource.Cast<Procurement>().ToList();
                _ = MainFrame.Navigate(new SupplyMonitoringPage(procurements));
            }
            else
                MessageBox.Show("Список тендеров пуст!");
        }
    }
}
