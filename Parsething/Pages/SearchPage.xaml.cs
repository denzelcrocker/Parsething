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

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Law>? Laws { get; set; }

        private List<Procurement>? FoundProcurements { get; set; }

        public SearchPage(List<Procurement>? procurements)
        {
            InitializeComponent();
            if (procurements != null)
            {
                SearchLV.ItemsSource = procurements;
            }
            Laws = GET.View.Laws();
            Law.ItemsSource = Laws;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new CardOfProcurement(procurement));
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
        public static List<Procurement>? ProcurementsBy(int searchId, string searchNumber, string searchLaw)
        {
            using ParsethingContext db = new();
            List<Procurement>? procurements = null;

            var query = db.Procurements.AsQueryable();

            if (searchId != 0)
                query = query.Where(p => p.Id == searchId);
            if(!string.IsNullOrEmpty(searchNumber))
                query = query.Where(p => p.Number == searchNumber);
            if (!string.IsNullOrEmpty(searchLaw))
                query = query.Where(p => p.Law.Number == searchLaw);

            query = query.Include(p => p.ProcurementState);
            query = query.Include(p => p.Law);
            query = query.Include(p => p.Method);
            query = query.Include(p => p.Platform);
            query = query.Include(p => p.TimeZone);
            query = query.Include(p => p.Region);
            query = query.Include(p => p.ShipmentPlan);
            query = query.Include(p => p.Organization);

            procurements = query.ToList();

            return procurements;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchLV.ItemsSource = null;
            if(FoundProcurements != null)
            FoundProcurements.Clear();
            int id = 0;
            if(SearchId.Text != "")
            id = Convert.ToInt32(SearchId.Text);
            string number = SearchNumber.Text;
            string law = Law.Text;
            
            FoundProcurements =  ProcurementsBy(id, number, law);
            SearchLV.ItemsSource = FoundProcurements;
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
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true));
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = (sender as Button)?.DataContext as Procurement;
            if (procurement != null)
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, false));
        }

    }
}
