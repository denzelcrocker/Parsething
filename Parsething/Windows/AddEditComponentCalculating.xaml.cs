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
using System.Windows.Shapes;

namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditComponentCalculating.xaml
    /// </summary>
    public partial class AddEditComponentCalculating : Window
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Manufacturer>? Manufacturers {get; set; }

        private List<Seller>? Sellers { get; set; }

        private bool IsCalculation;

        private Procurement Procurement;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        public AddEditComponentCalculating(ComponentCalculation componentCalculation, Procurement procurement, bool isCalculation)
        {
            InitializeComponent();

            Procurement = procurement;
            IsCalculation = isCalculation;

            Manufacturers = GET.View.Manufacturers();
            Manufacturer.ItemsSource = Manufacturers;

            Sellers = GET.View.Sellers();
            Seller.ItemsSource = Sellers;

            if (componentCalculation != null)
            {
                AddOrEdit.Text = "Редактирование";
                AddPositionCalculating.Visibility = Visibility.Hidden;
            }
            else
            {
                AddOrEdit.Text = "Добавление";
                AddPositionCalculating.Visibility = Visibility.Visible;

            }
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;

        private void AddPositionCalculating_Click(object sender, RoutedEventArgs e)
        {
            if(ComponentName.Text != "")
            {
                ComponentCalculation componentCalculation = new ComponentCalculation();
                componentCalculation.ProcurementId = Procurement.Id;
                componentCalculation.PartNumber = PartNumber.Text;
                componentCalculation.ComponentName = ComponentName.Text;
                if (Manufacturer.SelectedItem != null)
                    componentCalculation.ManufacturerId = ((Manufacturer)Manufacturer.SelectedItem).Id;
                if (Price.Text != "")
                {
                    decimal PriceDecimal;
                    if (decimal.TryParse(Price.Text, out PriceDecimal))
                    {
                        componentCalculation.Price = PriceDecimal;
                    }
                    else
                    {
                        MessageBox.Show("Неправильный формат поля: Цена");
                        return;
                    }
                }
                if (Seller.SelectedItem != null)
                    componentCalculation.SellerId = ((Seller)Seller.SelectedItem).Id;
                componentCalculation.Count = Convert.ToInt32(Count.Text);
                componentCalculation.Reserve = Reserve.Text;
                componentCalculation.Note = Note.Text;
                PUT.ComponentCalculation(componentCalculation);
                _ = MainFrame.Navigate(new Pages.ComponentCalculationsPage(Procurement, IsCalculation));
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Необходимо заполнить хотя бы наименование");
            }

        }

        private void Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}