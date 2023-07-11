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
    /// Логика взаимодействия для AddEditComponentPurchase.xaml
    /// </summary>
    public partial class AddEditComponentPurchase : Window
    {
        private Frame MainFrame { get; set; } = null!;

        private List<Manufacturer>? Manufacturers { get; set; }

        private List<Seller>? Sellers { get; set; }

        private List<ComponentState>? ComponentStates { get; set; }


        private bool IsCalculation;
        private bool IsSearch;

        private Procurement Procurement;
        private List<Procurement>? Procurements { get; set; }


        private ComponentCalculation ComponentCalculation;

        SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(0x53, 0x53, 0x53));
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        public AddEditComponentPurchase(ComponentCalculation componentCalculation, Procurement procurement, List<Procurement> procurements, bool isCalculation, bool isPosition, bool isSearch)
        {
            InitializeComponent();
            Procurements = procurements;
            IsSearch = isSearch;
            Procurement = procurement;
            ComponentCalculation = componentCalculation;
            IsCalculation = isCalculation;

            Manufacturers = GET.View.Manufacturers();
            Manufacturer.ItemsSource = Manufacturers;

            Sellers = GET.View.Sellers();
            Seller.ItemsSource = Sellers;

            ComponentStates = GET.View.ComponentStates();
            ComponentState.ItemsSource = ComponentStates;

            if (componentCalculation != null)
            {
                AddOrEdit.Text = "Редактирование";
                AddPositionPurchase.Visibility = Visibility.Hidden;
                EditPositionPurchase.Visibility = Visibility.Visible;
                DeletePosition.Visibility = Visibility.Visible;

                PartNumber.Text = componentCalculation.PartNumber;
                ComponentName.Text = componentCalculation.ComponentName;
                foreach (Manufacturer manufacturer in Manufacturer.ItemsSource)
                    if (manufacturer.Id == componentCalculation.ManufacturerId)
                    {
                        Manufacturer.SelectedItem = manufacturer;
                        break;
                    }
                foreach (ComponentState componentState in ComponentState.ItemsSource)
                    if (componentState.Id == componentCalculation.ComponentStateId)
                    {
                        ComponentState.SelectedItem = componentState;
                        break;
                    }
                Date.SelectedDate = componentCalculation.Date;
                Price.Text = componentCalculation.PricePurchase.ToString();
                foreach (Seller seller in Seller.ItemsSource)
                    if (seller.Id == componentCalculation.SellerId)
                    {
                        Seller.SelectedItem = seller;
                        break;
                    }
                Count.Text = componentCalculation.Count.ToString();
                Reserve.Text = componentCalculation.Reserve;
                Note.Text = componentCalculation.NotePurchase;
            }
            else
            {
                AddOrEdit.Text = "Добавление";
                AddPositionPurchase.Visibility = Visibility.Visible;
                EditPositionPurchase.Visibility = Visibility.Hidden;
                DeletePosition.Visibility = Visibility.Hidden;
            }
            if (isPosition)
            {
                AddOrEdit.Text = "Разделитель";
                PartNumber.IsEnabled = false;
                Manufacturer.IsEnabled = false;
                ComponentState.IsEnabled = false;
                DateLabel.IsEnabled = false;
                Price.IsEnabled = false;
                Count.IsEnabled = false;
                Seller.IsEnabled = false;
                Reserve.IsEnabled = false;
                Note.IsEnabled = false;

                PartNumberLabel.Foreground = Gray;
                ManufacturerLabel.Foreground = Gray;
                ComponentStateLabel.Foreground = Gray;
                DateLabel.Foreground = Gray;
                PriceLabel.Foreground = Gray;
                CountLabel.Foreground = Gray;
                SellerLabel.Foreground = Gray;
                ReserveLabel.Foreground = Gray;
                NoteLabel.Foreground = Gray;
            }
        }

        private void Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void AddPositionPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentName.Text != "")
            {
                ComponentCalculation componentCalculation = new ComponentCalculation();
                componentCalculation.ProcurementId = Procurement.Id;
                componentCalculation.PartNumber = PartNumber.Text;
                componentCalculation.ComponentName = ComponentName.Text;
                if (Manufacturer.SelectedItem != null)
                    componentCalculation.ManufacturerId = ((Manufacturer)Manufacturer.SelectedItem).Id;
                if (ComponentState.SelectedItem != null)
                    componentCalculation.ComponentStateId = ((ComponentState)ComponentState.SelectedItem).Id;
                componentCalculation.Date = Date.SelectedDate;
                if (Price.Text != "")
                {
                    decimal PriceDecimal;
                    if (decimal.TryParse(Price.Text, out PriceDecimal))
                    {
                        componentCalculation.PricePurchase = PriceDecimal;
                    }
                    else
                    {
                        MessageBox.Show("Неправильный формат поля: Цена");
                        return;
                    }
                }

                if (Count.Text != null && Count.Text != "")
                    componentCalculation.Count = Convert.ToInt32(Count.Text);
                if (Seller.SelectedItem != null)
                    componentCalculation.SellerId = ((Seller)Seller.SelectedItem).Id;
                componentCalculation.Reserve = Reserve.Text;
                componentCalculation.NotePurchase = Note.Text;
                PUT.ComponentCalculation(componentCalculation);
                _ = MainFrame.Navigate(new Pages.ComponentCalculationsPage(Procurement, Procurements, IsCalculation, IsSearch));
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Необходимо заполнить хотя бы наименование");
            }
        }

        private void DeletePosition_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentCalculation != null)
                DELETE.ComponentCalculation(ComponentCalculation);
            _ = MainFrame.Navigate(new Pages.ComponentCalculationsPage(Procurement, Procurements, IsCalculation, IsSearch));
            DialogResult = true;
        }

        private void EditPositionPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentName.Text != "")
            {
                ComponentCalculation componentCalculation = new ComponentCalculation();
                componentCalculation.Id = ComponentCalculation.Id;
                componentCalculation.ProcurementId = Procurement.Id;
                componentCalculation.PartNumber = PartNumber.Text;
                componentCalculation.ComponentName = ComponentName.Text;
                if (Manufacturer.SelectedItem != null)
                    componentCalculation.ManufacturerId = ((Manufacturer)Manufacturer.SelectedItem).Id;
                if (ComponentState.SelectedItem != null)
                    componentCalculation.ComponentStateId = ((ComponentState)ComponentState.SelectedItem).Id;
                componentCalculation.Date = Date.SelectedDate;
                if (Price.Text != "")
                {
                    decimal PriceDecimal;
                    if (decimal.TryParse(Price.Text, out PriceDecimal))
                    {
                        componentCalculation.PricePurchase = PriceDecimal;
                    }
                    else
                    {
                        MessageBox.Show("Неправильный формат поля: Цена");
                        return;
                    }
                }
                
                if (Count.Text != null && Count.Text != "")
                    componentCalculation.Count = Convert.ToInt32(Count.Text);
                if (Seller.SelectedItem != null)
                    componentCalculation.SellerId = ((Seller)Seller.SelectedItem).Id;
                componentCalculation.Reserve = Reserve.Text;
                componentCalculation.NotePurchase = Note.Text;
                PULL.ComponentCalculation(componentCalculation);
                _ = MainFrame.Navigate(new Pages.ComponentCalculationsPage(Procurement, Procurements, IsCalculation, IsSearch));
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Необходимо заполнить хотя бы наименование");
            }
        }
    }
}
