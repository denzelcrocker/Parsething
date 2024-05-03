using DatabaseLibrary.Entities.ComponentCalculationProperties;
using DatabaseLibrary.Entities.ProcurementProperties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using static Parsething.Pages.ComponentCalculationsPage;

namespace Parsething.Functions
{
    internal class ListViewInitialization
    {
        private static Procurement Procurement = new Procurement();

        private static List<ComponentCalculation>? ComponentCalculations = new List<ComponentCalculation>();

        private static List<ComponentState>? ComponentStates = new List<ComponentState>();

        private static List<Manufacturer>? Manufacturers = new List<Manufacturer>();

        private static List<Seller>? Sellers = new List<Seller>();

        private static bool IsCalculation;

        private static ListView ListView;

        private static TextBlock CalculationPriceTextBlock;

        private static TextBlock PurchasePriceTextBlock;

        static SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        static SolidColorBrush Black = new SolidColorBrush(Colors.Black);


        private static string[] columnNames = { "Заголовок", "Карта сборки" };

        public static void ComponentCalculationsListViewInitialization(bool isCalculation, List<ComponentCalculation> componentCalculations, ListView listViewToInitialization, TextBlock calculationPriceTextBlock, TextBlock purchasePriceTextBlock, Procurement procurement)
        {
            ListView = listViewToInitialization;
            Procurement = procurement;
            CalculationPriceTextBlock = calculationPriceTextBlock;
            PurchasePriceTextBlock = purchasePriceTextBlock;
            IsCalculation = isCalculation;
            Manufacturers = GET.View.Manufacturers();
            Sellers = GET.View.Sellers();
            ComponentStates = GET.View.ComponentStates();
            ComponentCalculations = componentCalculations;
            List<StackPanel> stackPanels = new();
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;

            if (IsCalculation)
            {

                foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
                {
                    StackPanel stackPanel = new StackPanel();
                    if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsAdded == false)
                    {
                        Grid grid = new Grid() { DataContext = new List<object> { componentCalculationHeader.ProcurementId, componentCalculationHeader.Id, componentCalculationHeader.IsHeader, componentCalculationHeader.IsDeleted, componentCalculationHeader. IsAdded} };
                        double[] columnWidths = { 628, 450, 90, 90 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }

                        TextBox textBoxHeader = new TextBox() { Text = componentCalculationHeader.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeader.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeader, 0, true );
                        textBoxHeader.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeader, 0);
                        LoadColumnNames(textBoxHeader, 0);
                        TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeaderAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeaderAssemblyMap,1, true);
                        textBoxHeaderAssemblyMap.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeaderAssemblyMap, 1);
                        LoadColumnNames(textBoxHeaderAssemblyMap, 1);
                        Button buttonAdd = new Button();
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.Content = "";
                        buttonAdd.Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderButton");

                        Button buttonDelete = new Button();
                        buttonDelete.Click += ButtonDeleteDivision_Click;
                        buttonDelete.Content = "";
                        buttonDelete.Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderButton");


                        Grid.SetColumn(textBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderAssemblyMap, 1);
                        Grid.SetColumn(buttonAdd, 2);
                        Grid.SetColumn(buttonDelete, 3);

                        grid.Children.Add(textBoxHeader);
                        grid.Children.Add(textBoxHeaderAssemblyMap);
                        grid.Children.Add(buttonAdd);
                        grid.Children.Add(buttonDelete);
                        stackPanel.Children.Add(grid);
                    }

                    ListView listView = new();
                    listView.Style = (Style)Application.Current.FindResource("ListView");

                    foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                    {
                        if (componentCalculation.ParentName == componentCalculationHeader.Id && componentCalculation.ParentName != null && componentCalculation.IsAdded == false)
                        {
                            Grid grid = new Grid() { DataContext = new List<object> { componentCalculation.Id, componentCalculation.ProcurementId ,componentCalculation.IsHeader, componentCalculation.ParentName, componentCalculation.IsDeleted, componentCalculation.IsAdded } };
                            double[] columnWidths = { 60, 450, 115, 70, 50, 90, 90, 150, 150 };

                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            TextBox textBoxPartNumber = new TextBox() { Text = componentCalculation.PartNumber, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxPartNumber.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxComponentName.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            //comboBoxManufacturer.SelectionChanged += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerId)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.Price.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            if (componentCalculation.Price != null && componentCalculation.Count != null)
                                calculatingAmount += (componentCalculation.Price * componentCalculation.Count);
                            textBoxPrice.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.Count.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            //comboBoxSeller.SelectionChanged += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerId)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.Reserve, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxReserve.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.Note, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxNote.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            Button buttonDelete = new Button();
                            buttonDelete.Click += ButtonDelete_Click;
                            buttonDelete.Content = "";
                            buttonDelete.Style = (Style)Application.Current.FindResource("ComponentCalculationItemButton");

                            Grid.SetColumn(textBoxPartNumber, 0);
                            Grid.SetColumn(textBoxComponentName, 1);
                            Grid.SetColumn(comboBoxManufacturer, 2);
                            Grid.SetColumn(textBoxPrice, 3);
                            Grid.SetColumn(textBoxCount, 4);
                            Grid.SetColumn(comboBoxSeller, 5);
                            Grid.SetColumn(textBoxReserve, 6);
                            Grid.SetColumn(textBoxNote, 7);
                            Grid.SetColumn(textBoxAssemblyMap, 8);
                            Grid.SetColumn(buttonDelete, 9);

                            grid.Children.Add(textBoxPartNumber);
                            grid.Children.Add(textBoxComponentName);
                            grid.Children.Add(comboBoxManufacturer);
                            grid.Children.Add(textBoxPrice);
                            grid.Children.Add(textBoxCount);
                            grid.Children.Add(comboBoxSeller);
                            grid.Children.Add(textBoxReserve);
                            grid.Children.Add(textBoxNote);
                            grid.Children.Add(textBoxAssemblyMap);
                            grid.Children.Add(buttonDelete);

                            listView.Items.Add(grid);
                        }

                    }
                    stackPanel.Children.Add(listView);
                    if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsAdded == false)
                    {
                        stackPanels.Add(stackPanel);
                    }
                }
                if (calculatingAmount > Procurement.InitialPrice)
                    CalculationPriceTextBlock.Foreground = Red;
                else 
                    CalculationPriceTextBlock.Foreground = Black;
                CalculationPriceTextBlock.Text = calculatingAmount.ToString();
                Procurement.CalculatingAmount =calculatingAmount;
                PULL.Procurement(Procurement);
            }
            else
            {
                foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
                {
                    StackPanel stackPanel = new StackPanel();
                    if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsDeleted == false)
                    {
                        Grid grid = new Grid() { DataContext = new List<object> { componentCalculationHeader.ProcurementId, componentCalculationHeader.Id, componentCalculationHeader.IsHeader, componentCalculationHeader.IsDeleted, componentCalculationHeader.IsAdded } };
                        double[] columnWidths = { 628, 450, 90, 90 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }

                        TextBox textBoxHeader = new TextBox() { Text = componentCalculationHeader.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeader.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeader, 0, true);
                        textBoxHeader.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeader, 0);
                        LoadColumnNames(textBoxHeader, 0);
                        TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeaderAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeaderAssemblyMap, 1, true);
                        textBoxHeaderAssemblyMap.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeaderAssemblyMap, 1);
                        LoadColumnNames(textBoxHeaderAssemblyMap, 1);
                        Button buttonAdd = new Button();
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.Content = "";
                        buttonAdd.Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderButton");

                        Button buttonDelete = new Button();
                        buttonDelete.Click += ButtonDeleteDivision_Click;
                        buttonDelete.Content = "";
                        buttonDelete.Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderButton");


                        Grid.SetColumn(textBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderAssemblyMap, 1);
                        Grid.SetColumn(buttonAdd, 2);
                        Grid.SetColumn(buttonDelete, 3);

                        grid.Children.Add(textBoxHeader);
                        grid.Children.Add(textBoxHeaderAssemblyMap);
                        grid.Children.Add(buttonAdd);
                        grid.Children.Add(buttonDelete);
                        stackPanel.Children.Add(grid);
                    }

                    ListView listView = new();
                    listView.Style = (Style)Application.Current.FindResource("ListView");

                    foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                    {
                        if (componentCalculation.ParentName == componentCalculationHeader.Id && componentCalculation.ParentName != null && componentCalculation.IsDeleted == false)
                        {
                            Grid grid = new Grid() { DataContext = new List<object> { componentCalculation.Id, componentCalculation.ProcurementId, componentCalculation.IsHeader, componentCalculation.ParentName, componentCalculationHeader.IsDeleted, componentCalculationHeader.IsAdded } };
                            double[] columnWidths = { 55, 300, 100, 125, 100, 75, 40, 105, 125, 125, 100 };

                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            TextBox textBoxPartNumber = new TextBox() { Text = componentCalculation.PartNumber, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxPartNumber.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentNamePurchase, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxComponentName.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            //comboBoxManufacturer.SelectionChanged += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerIdPurchase)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            ComboBox comboBoxComponentState = new ComboBox() { ItemsSource = ComponentStates, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            //comboBoxComponentState.SelectionChanged += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            foreach (ComponentState componentState in ComponentStates)
                                if (componentState.Id == componentCalculation.ComponentStateId)
                                {
                                    comboBoxComponentState.SelectedItem = componentState;
                                    break;
                                }
                            DatePicker datePicker = new DatePicker() { SelectedDate = componentCalculation.Date, Style = (Style)Application.Current.FindResource("ComponentCalculations.DatePickerStyle") };
                            datePicker.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.PricePurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            if (componentCalculation.PricePurchase != null && componentCalculation.CountPurchase != null)
                                purchaseAmount += (componentCalculation.PricePurchase * componentCalculation.CountPurchase);
                            textBoxPrice.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.CountPurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            //comboBoxSeller.SelectionChanged += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerIdPurchase)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.ReservePurchase, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxReserve.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.NotePurchase, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxNote.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            Button buttonDelete = new Button();
                            buttonDelete.Click += ButtonDelete_Click;
                            buttonDelete.Content = "";
                            buttonDelete.Style = (Style)Application.Current.FindResource("ComponentCalculationItemButton");

                            Grid.SetColumn(textBoxPartNumber, 0);
                            Grid.SetColumn(textBoxComponentName, 1);
                            Grid.SetColumn(comboBoxManufacturer, 2);
                            Grid.SetColumn(comboBoxComponentState, 3);
                            Grid.SetColumn(datePicker, 4);
                            Grid.SetColumn(textBoxPrice, 5);
                            Grid.SetColumn(textBoxCount, 6);
                            Grid.SetColumn(comboBoxSeller, 7);
                            Grid.SetColumn(textBoxReserve, 8);
                            Grid.SetColumn(textBoxNote, 9);
                            Grid.SetColumn(textBoxAssemblyMap, 10);
                            Grid.SetColumn(buttonDelete, 11);

                            grid.Children.Add(textBoxPartNumber);
                            grid.Children.Add(textBoxComponentName);
                            grid.Children.Add(comboBoxManufacturer);
                            grid.Children.Add(comboBoxComponentState);
                            grid.Children.Add(datePicker);
                            grid.Children.Add(textBoxPrice);
                            grid.Children.Add(textBoxCount);
                            grid.Children.Add(comboBoxSeller);
                            grid.Children.Add(textBoxReserve);
                            grid.Children.Add(textBoxNote);
                            grid.Children.Add(textBoxAssemblyMap);
                            grid.Children.Add(buttonDelete);

                            listView.Items.Add(grid);
                        }

                    }
                    stackPanel.Children.Add(listView);
                    if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsDeleted == false)
                    {
                        stackPanels.Add(stackPanel);
                    }
                }

                if (Procurement.ReserveContractAmount != null)
                {
                    if (purchaseAmount > Procurement.ReserveContractAmount)
                        PurchasePriceTextBlock.Foreground = Red;
                }
                else
                {
                    if (purchaseAmount > Procurement.ContractAmount)
                        PurchasePriceTextBlock.Foreground = Red;
                }
                PurchasePriceTextBlock.Text = purchaseAmount.ToString();
                Procurement.PurchaseAmount = purchaseAmount;
                PULL.Procurement(Procurement);
            }
            ListView.ItemsSource = stackPanels;
        }

        private static void ButtonDeleteDivision_Click(object sender, RoutedEventArgs e)
        {
            if (IsCalculation)
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[1]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if ((ComponentCalculations[i].Id == idToDelete && ComponentCalculations[i].IsHeader == true) || (ComponentCalculations[i].ParentName == idToDelete && ComponentCalculations[i].IsHeader == false))
                    {
                        ComponentCalculations.RemoveAt(i);
                    }
                }
                DELETE.ComponentCalculation(idToDelete);
            }
            else
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[1]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if ((ComponentCalculations[i].Id == idToDelete && ComponentCalculations[i].IsHeader == true) || (ComponentCalculations[i].ParentName == idToDelete && ComponentCalculations[i].IsHeader == false))
                    {
                        ComponentCalculations[i].IsDeleted = true;
                        PULL.ComponentCalculation(ComponentCalculations[i]);
                    }
                }
            }
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
        }

        private static void ButtonAddPosition_Click(object sender, RoutedEventArgs e)
        {
            StackPanel parentStackPanel = (StackPanel)((Grid)((Button)sender).Parent).Parent;
            Grid grid = (Grid)parentStackPanel.Children[0];
            ComponentCalculation newComponentCalculation = new ComponentCalculation
            {
                ProcurementId = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[0]),
                ParentName = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[1]),

                IsAdded = IsCalculation ? false : true,
                IsDeleted = false,
                IsHeader = false,
            };
            ComponentCalculations.Add(newComponentCalculation);
            PUT.ComponentCalculation(newComponentCalculation);
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
        }
        private static void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IsCalculation)
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[0]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if (ComponentCalculations[i].Id == idToDelete)
                    {
                        ComponentCalculations.RemoveAt(i);
                        break;
                    }
                }
                DELETE.ComponentCalculation(idToDelete);
            }
            else
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[0]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if (ComponentCalculations[i].Id == idToDelete)
                    {
                        ComponentCalculations[i].IsDeleted = true;
                        PULL.ComponentCalculation(ComponentCalculations[i]);
                        break;
                    }
                }
            }
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
        }
        public static void ButtonAddDivision_Click(object sender, RoutedEventArgs e, Procurement procurement)
        {
                ComponentCalculation newComponentCalculation = new ComponentCalculation
                {
                    ProcurementId = procurement.Id,
                    IsHeader = true,
                    IsAdded = IsCalculation ? false : true,
                    IsDeleted = false,
                };
                ComponentCalculations.Add(newComponentCalculation);
                PUT.ComponentCalculation(newComponentCalculation);
                ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
        }
        private static void TextBox_LostFocus(object sender, RoutedEventArgs e, TextBox textBox, int headerId, bool isHeader)
        {
            UpdateListView();
            if (isHeader)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = columnNames[headerId];
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }
        private static void TextBox_GotFocus (object sender, RoutedEventArgs e, TextBox textBox, int headerId)
        {
            if (textBox.Text == columnNames[headerId])
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }
        private static void LoadColumnNames(TextBox textBox, int headerId)
        {
            if (textBox.Text == string.Empty || textBox.Text == columnNames[headerId])
            {
                textBox.Text = columnNames[headerId];
                textBox.Foreground = Brushes.Gray;
            }
        }
        private static void UpdateListView()
        {
            foreach (StackPanel stackPanel in ListView.Items)
            {
                ComponentCalculation componentCalculationHeader = new ComponentCalculation();
                try
                {
                    TextBox textBoxHeader = (TextBox)((Grid)stackPanel.Children[0]).Children[0];
                    TextBox textBoxAssemblyMapHeader = (TextBox)((Grid)stackPanel.Children[0]).Children[1];

                    componentCalculationHeader.Id = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[1];
                    componentCalculationHeader.ProcurementId = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[0];
                    componentCalculationHeader.ComponentName = textBoxHeader.Text;
                    componentCalculationHeader.AssemblyMap = textBoxAssemblyMapHeader.Text;
                    componentCalculationHeader.IsDeleted = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[3];
                    componentCalculationHeader.IsAdded = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[4];
                    componentCalculationHeader.IsHeader = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[2]; ;
                    PULL.ComponentCalculation(componentCalculationHeader);
                    ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculationHeader.ProcurementId);
                    ListView innerListView = (ListView)stackPanel.Children[1];
                    foreach (Grid grid in innerListView.Items)
                    {
                        ComponentCalculation componentCalculationItem = new ComponentCalculation();
                        TextBox textBoxPartNumber = (TextBox)grid.Children[0];
                        TextBox textBoxComponentName = (TextBox)grid.Children[1];
                        TextBox textBoxPrice = (TextBox)grid.Children[3];
                        TextBox textBoxCount = (TextBox)grid.Children[4];
                        TextBox textBoxReserve = (TextBox)grid.Children[6];
                        TextBox textBoxNote = (TextBox)grid.Children[7];
                        TextBox textBoxAssemblyMap = (TextBox)grid.Children[8];

                        componentCalculationItem.Id = (int)((List<object>)grid.DataContext)[0];
                        componentCalculationItem.ProcurementId = (int)((List<object>)grid.DataContext)[1];
                        componentCalculationItem.PartNumber = textBoxPartNumber.Text;
                        componentCalculationItem.ComponentName = textBoxComponentName.Text;
                        if ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem != null)
                        {
                            componentCalculationItem.ManufacturerId = ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem).Id;
                            //componentCalculationItem.ManufacturerIdPurchase = ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem).Id;
                        }
                        if (textBoxPrice.Text != "")
                        {
                            componentCalculationItem.Price = Convert.ToDecimal(textBoxPrice.Text);
                            //componentCalculationItem.PricePurchase = Convert.ToDecimal(textBoxPrice.Text);
                        }
                        if (textBoxCount.Text != "")
                        {
                            componentCalculationItem.Count = Convert.ToInt32(textBoxCount.Text);
                            //componentCalculationItem.CountPurchase = Convert.ToInt32(textBoxCount.Text);
                        }
                        if ((Seller)((ComboBox)grid.Children[5]).SelectedItem != null)
                        {
                            componentCalculationItem.SellerId = ((Seller)((ComboBox)grid.Children[5]).SelectedItem).Id;
                            //componentCalculationItem.SellerIdPurchase = ((Seller)((ComboBox)grid.Children[5]).SelectedItem).Id;
                        }
                        componentCalculationItem.Reserve = textBoxReserve.Text;
                        componentCalculationItem.Note = textBoxNote.Text;
                        componentCalculationItem.AssemblyMap = textBoxAssemblyMap.Text;
                        componentCalculationItem.ParentName = Convert.ToInt32(componentCalculationHeader.Id);
                        componentCalculationItem.IsDeleted = (bool)((List<object>)grid.DataContext)[4];
                        componentCalculationItem.IsAdded = (bool)((List<object>)grid.DataContext)[5];
                        componentCalculationItem.IsHeader = (bool)((List<object>)grid.DataContext)[2];
                        PULL.ComponentCalculation(componentCalculationItem);
                        ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculationItem.ProcurementId);
                    }
                    ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
                }
                catch 
                {

                }
            }
        }
    }
}
