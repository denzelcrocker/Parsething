using DatabaseLibrary.Entities.ProcurementProperties;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using Parsething.Functions;
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
    /// Логика взаимодействия для ComponentCalculationsPage.xaml
    /// </summary>
    public partial class ComponentCalculationsPage : Page
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Comment>? Comments = new List<Comment>();

        private List<ComponentCalculation>? ComponentCalculations = new List<ComponentCalculation>();

        private List<Procurement>? Procurements = new List<Procurement>();
       
        private Procurement? Procurement { get; set; }

        private bool IsSearch;

        public ComponentCalculationsPage(Procurement procurement,List<Procurement> procurements ,bool isCalculation, bool isSearch)
        {
            InitializeComponent();
            IsSearch = isSearch;
            Procurements = procurements;
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;
            if(procurement != null)
            {
                Procurement = procurement;
                Id.Text = Procurement.Id.ToString();

                Comments = GET.View.CommentsBy(procurement.Id);
                CommentsListView.ItemsSource = Comments;
                ComponentCalculations = GET.View.ComponentCalculationsBy(procurement.Id);
                ListViewInitialization.ComponentCalculationsListViewInitialization(isCalculation, ComponentCalculations, ComponentCalculationsListView);
            }
        }

        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommentsTextBox.Text != "")
            {
                Comment? comment = new Comment { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = CommentsTextBox.Text, IsTechnical = IsTechnical.IsChecked };
                CommentsTextBox.Clear();
                IsTechnical.IsChecked = false;
                PUT.Comment(comment);
                CommentsListView.ItemsSource = null;
                Comments.Clear();
                Comments = GET.View.CommentsBy(Procurement.Id);
                CommentsListView.ItemsSource = Comments;
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSearch)
            {
                _ = MainFrame.Navigate(new SearchPage(Procurements));
            }
            else
            {
                if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Администратор")
                {
                    _ = MainFrame.Navigate(new Pages.AdministratorPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела расчетов" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела расчетов")
                {
                    _ = MainFrame.Navigate(new Pages.HeadsOfCalculatorsPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист отдела расчетов")
                {
                    _ = MainFrame.Navigate(new Pages.CalculatorPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель тендерного отдела" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя тендреного отдела")
                {
                    _ = MainFrame.Navigate(new Pages.HeadsOfManagersPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист по работе с электронными площадками")
                {
                    _ = MainFrame.Navigate(new Pages.EPlatformSpecialistPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист тендерного отдела")
                {
                    _ = MainFrame.Navigate(new Pages.ManagerPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела закупки" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела закупок" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист закупки")
                {
                    _ = MainFrame.Navigate(new Pages.PurchaserPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела производства" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела производства" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист по производству")
                {
                    _ = MainFrame.Navigate(new Pages.AssemblyPage());
                }
                else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Юрист")
                {
                    _ = MainFrame.Navigate(new Pages.LawyerPage());
                }
                else
                {

                }
            }
        }

        private void AddPositionCalculating_Click(object sender, RoutedEventArgs e)
        {
            ComponentCalculation componentCalculation = null;
            Windows.AddEditComponentCalculating addEditComponentCalculating = new Windows.AddEditComponentCalculating(componentCalculation, Procurement,Procurements , true, false, IsSearch);
            addEditComponentCalculating.ShowDialog();
        }

        private void CalculatingListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ComponentCalculation componentCalculation = ((ListView)sender).SelectedItem as ComponentCalculation;
            if (componentCalculation != null)
            {
                Windows.AddEditComponentCalculating addEditComponentCalculating = new Windows.AddEditComponentCalculating(componentCalculation, Procurement, Procurements, true, false, IsSearch);
                addEditComponentCalculating.ShowDialog();
            }
        }

        private void AddDivisionCalculating_Click(object sender, RoutedEventArgs e)
        {
            ComponentCalculation componentCalculation = null;
            Windows.AddEditComponentCalculating addEditComponentCalculating = new Windows.AddEditComponentCalculating(componentCalculation, Procurement, Procurements, true, true, IsSearch);
            addEditComponentCalculating.ShowDialog();
        }

        private void AddPositionPurchase_Click(object sender, RoutedEventArgs e)
        {
            ComponentCalculation componentCalculation = null;
            Windows.AddEditComponentPurchase addEditComponentCalculating = new Windows.AddEditComponentPurchase(componentCalculation, Procurement, Procurements, false, false, IsSearch);
            addEditComponentCalculating.ShowDialog();
        }

        private void AddDivisionPurchase_Click(object sender, RoutedEventArgs e)
        {
            ComponentCalculation componentCalculation = null;
            Windows.AddEditComponentPurchase addEditComponentCalculating = new Windows.AddEditComponentPurchase(componentCalculation, Procurement, Procurements, false, true, IsSearch);
            addEditComponentCalculating.ShowDialog();
        }

        private void PurchaseListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ComponentCalculation componentCalculation = ((ListView)sender).SelectedItem as ComponentCalculation;
            if (componentCalculation != null)
            {
                Windows.AddEditComponentPurchase addEditComponentCalculating = new Windows.AddEditComponentPurchase(componentCalculation, Procurement, Procurements, false, false, IsSearch);
                addEditComponentCalculating.ShowDialog();
            }
        }

        private void Assembly_Click(object sender, RoutedEventArgs e)
        {
            AssemblyPopUp.IsOpen = !AssemblyPopUp.IsOpen;
        }
    }
}
