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



        private Procurement? Procurement { get; set; }

        public ComponentCalculationsPage(Procurement procurement, bool isCalculation)
        {
            InitializeComponent();
            if(procurement != null)
            {
                Procurement = procurement;
                Id.Text = Procurement.Id.ToString();

                Comments = GET.View.CommentsBy(procurement.Id);
                CommentsListView.ItemsSource = Comments;


                ComponentCalculations = GET.View.ComponentCalculationsBy(procurement.Id);

                if (isCalculation)
                {
                    PurchaseOrCalculatiing.Text = "Расчет";
                    CalculatingPanel.Visibility = Visibility.Visible;
                    CalculatingListView.Visibility = Visibility.Visible;
                    ColumnsNamesCalculating.Visibility = Visibility.Visible;
                    PurchasePanel.Visibility = Visibility.Hidden;
                    CalculatingListView.Items.Clear();
                    CalculatingListView.ItemsSource = ComponentCalculations;

                    MaxPrice.Text = Procurement.InitialPrice.ToString();

                }
                else 
                {
                    PurchaseOrCalculatiing.Text = "Закупка";
                    PurchasePanel.Visibility = Visibility.Visible;
                    CalculatingPanel.Visibility = Visibility.Hidden;
                    CalculatingListView.Visibility = Visibility.Hidden;
                    ColumnsNamesCalculating.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommentsTextBox.Text != null)
            {
                Comment? comment = new Comment { EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id, Date = DateTime.Now, EntityType = "Procurement", EntryId = Procurement.Id, Text = CommentsTextBox.Text };
                CommentsTextBox.Clear();
                PUT.Comment(comment);
                CommentsListView.ItemsSource = null;
                Comments.Clear();
                Comments = GET.View.CommentsBy(Procurement.Id);
                CommentsListView.ItemsSource = Comments;
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }

        private void AddPositionCalculating_Click(object sender, RoutedEventArgs e)
        {
            ComponentCalculation componentCalculation = null;
            Windows.AddEditComponentCalculating addEditComponentCalculating = new Windows.AddEditComponentCalculating(componentCalculation, Procurement, true);
            addEditComponentCalculating.ShowDialog();
        }
    }
}
