using DatabaseLibrary.Entities.ComponentCalculationProperties;
using DatabaseLibrary.Entities.ProcurementProperties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private static List<ComponentHeaderType>? ComponentHeaderTypes = new List<ComponentHeaderType>();

        private static List<Seller>? Sellers = new List<Seller>();

        private static bool IsCalculation;

        private static ListView ListView;

        private static TextBlock CalculationPriceTextBlock;

        private static TextBlock PurchasePriceTextBlock;

        static SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        static SolidColorBrush Black = new SolidColorBrush(Colors.Black);

        static List<string> ProcurementStates = new List<string>() { "Новый", "Посчитан", "Оформить", "Оформлен", "Отправлен", "Отмена", "Отклонен"};

        private static string[] columnNames = { "Заголовок", "Карта сборки" };

        public static void ComponentCalculationsListViewInitialization(bool isCalculation, List<ComponentCalculation> componentCalculations, ListView listViewToInitialization, TextBlock calculationPriceTextBlock, TextBlock purchasePriceTextBlock, Procurement procurement)
        {
            ListView = listViewToInitialization;
            Procurement = procurement;
            CalculationPriceTextBlock = calculationPriceTextBlock;
            ComponentHeaderTypes = GET.View.ComponentHeaderTypes();
            PurchasePriceTextBlock = purchasePriceTextBlock;
            IsCalculation = isCalculation;
            Manufacturers = GET.View.Manufacturers();
            Sellers = GET.View.Sellers();
            ComponentStates = GET.View.ComponentStates();
            ComponentCalculations = componentCalculations;
            List<StackPanel> stackPanels = new();
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);
            if (Procurement.IsPurchaseBlocked == true && Procurement.PurchaseUserId != ((Employee)Application.Current.MainWindow.DataContext).Id)
            {
                MessageBox.Show($"Закупка сейчас редактируется пользователем: \n{GET.View.Employees().Where(e => e.Id == Procurement.PurchaseUserId).First().FullName}");
            }
            else if (Procurement.IsCalculationBlocked == true && Procurement.CalculatingUserId != ((Employee)Application.Current.MainWindow.DataContext).Id)
            {
                MessageBox.Show($"Расчет сейчас редактируется пользователем: \n{GET.View.Employees().Where(e => e.Id == Procurement.CalculatingUserId).First().FullName}");
            }
            else if (IsCalculation == true)
            {
                Procurement.IsCalculationBlocked = true;
                Procurement.CalculatingUserId = ((Employee)Application.Current.MainWindow.DataContext).Id;
            }
            else if (IsCalculation == false)
            {
                Procurement.IsPurchaseBlocked = true;
                Procurement.PurchaseUserId = ((Employee)Application.Current.MainWindow.DataContext).Id;
            }
            if (IsCalculation)
            {

                foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
                {
                    StackPanel stackPanel = new StackPanel();
                    if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsAdded == false)
                    {
                        Grid grid = new Grid() { DataContext = new List<object> { componentCalculationHeader.ProcurementId, componentCalculationHeader.Id, componentCalculationHeader.IsHeader, componentCalculationHeader.IsDeleted, componentCalculationHeader.IsAdded } };
                        double[] columnWidths = { 628, 500, 90, 90 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }

                        ComboBox comboBoxHeader = new ComboBox() { ItemsSource = ComponentHeaderTypes, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationHeader") };
                        foreach (ComponentHeaderType componentHeaderType in ComponentHeaderTypes)
                            if (componentHeaderType.Id == componentCalculationHeader.HeaderTypeId)
                            {
                                comboBoxHeader.SelectedItem = componentHeaderType;
                                break;
                            }
                        comboBoxHeader.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        comboBoxHeader.LostFocus += (sender, e) => ComboBox_LostFocus(sender, e, comboBoxHeader, 0, true);
                        comboBoxHeader.GotFocus += (sender, e) => ComboBox_GotFocus(sender, e, comboBoxHeader, 0);
                        LoadColumnNames(comboBoxHeader, 0);
                        TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeaderAssemblyMap.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        textBoxHeaderAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeaderAssemblyMap, 1, true);
                        textBoxHeaderAssemblyMap.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeaderAssemblyMap, 1);
                        LoadColumnNames(textBoxHeaderAssemblyMap, 1);
                        Button buttonAdd = new Button();
                        buttonAdd.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.Content = "";
                        buttonAdd.Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderButton");
                        Button buttonDelete = new Button();
                        buttonDelete.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        buttonDelete.Click += ButtonDeleteDivision_Click;
                        buttonDelete.Content = "";
                        buttonDelete.Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderButton");


                        Grid.SetColumn(comboBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderAssemblyMap, 1);
                        Grid.SetColumn(buttonAdd, 2);
                        Grid.SetColumn(buttonDelete, 3);

                        grid.Children.Add(comboBoxHeader);
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
                            Grid grid = new Grid() { DataContext = new List<object> { componentCalculation.Id, componentCalculation.ProcurementId, componentCalculation.IsHeader, componentCalculation.ParentName, componentCalculation.IsDeleted, componentCalculation.IsAdded, componentCalculation.ComponentNamePurchase, componentCalculation.ManufacturerIdPurchase, componentCalculation.PricePurchase, componentCalculation.CountPurchase, componentCalculation.SellerIdPurchase, componentCalculation.ReservePurchase, componentCalculation.NotePurchase, componentCalculation.ComponentStateId } };
                            double[] columnWidths = { 60, 450, 115, 70, 50, 115, 90, 175, 150 };

                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            TextBox textBoxPartNumber = new TextBox() { Text = componentCalculation.PartNumber, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxPartNumber.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxPartNumber.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxComponentName.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxComponentName.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            comboBoxManufacturer.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerId)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.Price.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxPrice.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            if (componentCalculation.Price != null && componentCalculation.Count != null)
                                calculatingAmount += (componentCalculation.Price * componentCalculation.Count);
                            textBoxPrice.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.Count.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxCount.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            comboBoxSeller.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerId)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.Reserve, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxReserve.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxReserve.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.Note, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxNote.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxNote.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxAssemblyMap.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            Button buttonDelete = new Button();
                            buttonDelete.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
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
                Procurement.CalculatingAmount = calculatingAmount;
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
                        double[] columnWidths = { 628, 500, 90, 90 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }
                        ComboBox comboBoxHeader = new ComboBox() { ItemsSource = ComponentHeaderTypes, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationHeader") };
                        foreach (ComponentHeaderType componentHeaderType in ComponentHeaderTypes)
                            if (componentHeaderType.Id == componentCalculationHeader.HeaderTypeId)
                            {
                                comboBoxHeader.SelectedItem = componentHeaderType;
                                break;
                            }
                        comboBoxHeader.LostFocus += (sender, e) => ComboBox_LostFocus(sender, e, comboBoxHeader, 0, true);
                        comboBoxHeader.GotFocus += (sender, e) => ComboBox_GotFocus(sender, e, comboBoxHeader, 0);
                        LoadColumnNames(comboBoxHeader, 0);
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

                        Grid.SetColumn(comboBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderAssemblyMap, 1);
                        Grid.SetColumn(buttonAdd, 2);
                        Grid.SetColumn(buttonDelete, 3);

                        grid.Children.Add(comboBoxHeader);
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
                            Grid grid = new Grid() { DataContext = new List<object> { componentCalculation.Id, componentCalculation.ProcurementId, componentCalculation.IsHeader, componentCalculation.ParentName, componentCalculation.IsDeleted, componentCalculation.IsAdded, componentCalculation.ComponentName, componentCalculation.ManufacturerId, componentCalculation.Price, componentCalculation.Count, componentCalculation.SellerId, componentCalculation.Reserve, componentCalculation.Note } };
                            double[] columnWidths = { 55, 300, 100, 125, 100, 75, 40, 105, 100, 125, 150 };
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
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerIdPurchase)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            ComboBox comboBoxComponentState = new ComboBox() { ItemsSource = ComponentStates, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            foreach (ComponentState componentState in ComponentStates)
                                if (componentState.Id == componentCalculation.ComponentStateId)
                                {
                                    comboBoxComponentState.SelectedItem = componentState;
                                    break;
                                }
                            DatePicker datePicker = new DatePicker() { SelectedDate = componentCalculation.Date, Style = (Style)Application.Current.FindResource("ComponentCalculations.DatePickerStyle") };
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.PricePurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            if (componentCalculation.PricePurchase != null && componentCalculation.CountPurchase != null)
                                purchaseAmount += (componentCalculation.PricePurchase * componentCalculation.CountPurchase);
                            textBoxPrice.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.CountPurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            textBoxCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
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
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);

            if (IsCalculation && Procurement.CalculatingUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
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
            else if (!IsCalculation && Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[1]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if ((ComponentCalculations[i].Id == idToDelete && ComponentCalculations[i].IsHeader == true) || (ComponentCalculations[i].ParentName == idToDelete && ComponentCalculations[i].IsHeader == false))
                    {
                        ComponentCalculations[i].IsDeleted = true;
                        PULL.ComponentCalculation(ComponentCalculations[i]);
                        ComponentCalculations.RemoveAt(i);
                        ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
                    }
                }
            }
            UpdateListView();
        }

        private static void ButtonAddPosition_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);

            if ((IsCalculation && Procurement.CalculatingUserId == ((Employee)Application.Current.MainWindow.DataContext).Id) || (!IsCalculation && Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id))
            {
                StackPanel parentStackPanel = (StackPanel)((Grid)((Button)sender).Parent).Parent;
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
            }
            UpdateListView();

        }
        private static void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);

            if (IsCalculation && Procurement.CalculatingUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
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
            else if (!IsCalculation && Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[0]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if (ComponentCalculations[i].Id == idToDelete)
                    {
                        ComponentCalculations[i].IsDeleted = true;
                        PULL.ComponentCalculation(ComponentCalculations[i]);
                        ComponentCalculations.RemoveAt(i);
                        ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
                        break;
                    }
                }
            }
            UpdateListView();
        }
        public static void ButtonAddDivision_Click(object sender, RoutedEventArgs e, Procurement procurement)
        {
                Procurement = GET.Entry.ProcurementBy(Procurement.Id);
                if (IsCalculation)
                {
                    if (ProcurementStates.Contains(Procurement.ProcurementState.Kind))
                    {
                        if (Procurement.CalculatingUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
                        {
                            ComponentCalculation newComponentCalculation = new ComponentCalculation
                            {
                                ProcurementId = Procurement.Id,
                                IsHeader = true,
                                IsAdded = false,
                                IsDeleted = false,
                            };
                            ComponentCalculations.Add(newComponentCalculation);
                            PUT.ComponentCalculation(newComponentCalculation);
                        }
                    }
                }
                else
                {
                    if (Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id)
                    {
                        ComponentCalculation newComponentCalculation = new ComponentCalculation
                        {
                            ProcurementId = Procurement.Id,
                            IsHeader = true,
                            IsAdded = true,
                            IsDeleted = false,
                        };
                        ComponentCalculations.Add(newComponentCalculation);
                        PUT.ComponentCalculation(newComponentCalculation);
                    }
                }
                UpdateListView();
            }
        
        private static void TextBox_LostFocus(object sender, RoutedEventArgs e, TextBox textBox, int headerId, bool isHeader)
        {
            if (isHeader)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = columnNames[headerId];
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }
        private static void TextBox_GotFocus(object sender, RoutedEventArgs e, TextBox textBox, int headerId)
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
        private static void ComboBox_LostFocus(object sender, RoutedEventArgs e, ComboBox comboBox, int headerId, bool isHeader)
        {
            if (isHeader)
            {
                if (string.IsNullOrWhiteSpace(comboBox.Text))
                {
                    comboBox.Text = columnNames[headerId];
                    comboBox.Foreground = Brushes.Gray;
                }
            }
        }
        private static void ComboBox_GotFocus(object sender, RoutedEventArgs e, ComboBox comboBox, int headerId)
        {
            if (comboBox.Text == columnNames[headerId])
            {
                comboBox.Text = string.Empty;
                comboBox.Foreground = Brushes.Black;
            }
        }
        private static void LoadColumnNames(ComboBox comboBox, int headerId)
        {
            if (comboBox.Text == string.Empty || comboBox.Text == columnNames[headerId])
            {
                comboBox.Text = columnNames[headerId];
                comboBox.Foreground = Brushes.Gray;
            }
        }
        public static void UpdateListView()
        {
            foreach (StackPanel stackPanel in ListView.Items)
            {
                ComponentCalculation componentCalculationHeader = new ComponentCalculation();
                try
                {
                    if ((ComponentHeaderType)((ComboBox)((Grid)stackPanel.Children[0]).Children[0]).SelectedItem != null)
                    {
                        componentCalculationHeader.HeaderTypeId = ((ComponentHeaderType)((ComboBox)((Grid)stackPanel.Children[0]).Children[0]).SelectedItem).Id;
                    }
                    ComboBox comboBoxHeader = (ComboBox)((Grid)stackPanel.Children[0]).Children[0];
                    TextBox textBoxAssemblyMapHeader = (TextBox)((Grid)stackPanel.Children[0]).Children[1];

                    componentCalculationHeader.Id = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[1];
                    componentCalculationHeader.ProcurementId = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[0];
                    componentCalculationHeader.AssemblyMap = textBoxAssemblyMapHeader.Text;
                    componentCalculationHeader.IsDeleted = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[3];
                    componentCalculationHeader.IsAdded = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[4];
                    componentCalculationHeader.IsHeader = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[2]; ;
                    PULL.ComponentCalculation(componentCalculationHeader);
                    ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculationHeader.ProcurementId);
                    ListView innerListView = (ListView)stackPanel.Children[1];
                    foreach (Grid grid in innerListView.Items)
                    {
                        if (IsCalculation && ProcurementStates.Contains(Procurement.ProcurementState.Kind))
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
                            componentCalculationItem.ComponentNamePurchase = textBoxComponentName.Text;
                            if ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem != null)
                            {
                                componentCalculationItem.ManufacturerId = ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem).Id;
                                componentCalculationItem.ManufacturerIdPurchase = ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem).Id;
                            }
                            componentCalculationItem.ComponentStateId = (int?)((List<object>)grid.DataContext)[13];

                            if (textBoxPrice.Text != "")
                            {
                                componentCalculationItem.Price = Convert.ToDecimal(textBoxPrice.Text);
                                componentCalculationItem.PricePurchase = Convert.ToDecimal(textBoxPrice.Text);
                            }
                            if (textBoxCount.Text != "")
                            {
                                componentCalculationItem.Count = Convert.ToInt32(textBoxCount.Text);
                                componentCalculationItem.CountPurchase = Convert.ToInt32(textBoxCount.Text);
                            }
                            if ((Seller)((ComboBox)grid.Children[5]).SelectedItem != null)
                            {
                                componentCalculationItem.SellerId = ((Seller)((ComboBox)grid.Children[5]).SelectedItem).Id;
                                componentCalculationItem.SellerIdPurchase = ((Seller)((ComboBox)grid.Children[5]).SelectedItem).Id;
                            }
                            componentCalculationItem.Reserve = textBoxReserve.Text;
                            componentCalculationItem.ReservePurchase = textBoxReserve.Text;
                            componentCalculationItem.Note = textBoxNote.Text;
                            componentCalculationItem.NotePurchase = textBoxNote.Text;
                            componentCalculationItem.AssemblyMap = textBoxAssemblyMap.Text;
                            componentCalculationItem.ParentName = Convert.ToInt32(componentCalculationHeader.Id);
                            componentCalculationItem.IsDeleted = (bool)((List<object>)grid.DataContext)[4];
                            componentCalculationItem.IsAdded = (bool)((List<object>)grid.DataContext)[5];
                            componentCalculationItem.IsHeader = (bool)((List<object>)grid.DataContext)[2];
                            PULL.ComponentCalculation(componentCalculationItem);
                            ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculationItem.ProcurementId);
                        }
                        else if(!IsCalculation)
                        {
                            ComponentCalculation componentCalculationItem = new ComponentCalculation();
                            TextBox textBoxPartNumber = (TextBox)grid.Children[0];
                            TextBox textBoxComponentName = (TextBox)grid.Children[1];
                            DatePicker datePicker = (DatePicker)grid.Children[4];
                            TextBox textBoxPrice = (TextBox)grid.Children[5];
                            TextBox textBoxCount = (TextBox)grid.Children[6];
                            TextBox textBoxReserve = (TextBox)grid.Children[8];
                            TextBox textBoxNote = (TextBox)grid.Children[9];
                            TextBox textBoxAssemblyMap = (TextBox)grid.Children[10];

                            componentCalculationItem.Id = (int)((List<object>)grid.DataContext)[0];
                            componentCalculationItem.ProcurementId = (int)((List<object>)grid.DataContext)[1];
                            componentCalculationItem.PartNumber = textBoxPartNumber.Text;
                            componentCalculationItem.ComponentName = (string)((List<object>)grid.DataContext)[6];
                            componentCalculationItem.ComponentNamePurchase = textBoxComponentName.Text;
                            if ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem != null)
                            {
                                componentCalculationItem.ManufacturerIdPurchase = ((Manufacturer)((ComboBox)grid.Children[2]).SelectedItem).Id;
                            }
                            componentCalculationItem.ManufacturerId = (int?)((List<object>)grid.DataContext)[7];
                            if ((ComponentState)((ComboBox)grid.Children[3]).SelectedItem != null)
                            {
                                componentCalculationItem.ComponentStateId = ((ComponentState)((ComboBox)grid.Children[3]).SelectedItem).Id;
                            }
                            componentCalculationItem.Date = datePicker.SelectedDate;
                            if (textBoxPrice.Text != "")
                            {
                                componentCalculationItem.PricePurchase = Convert.ToDecimal(textBoxPrice.Text);
                            }
                            componentCalculationItem.Price = (decimal?)((List<object>)grid.DataContext)[8];
                            if (textBoxCount.Text != "")
                            {
                                componentCalculationItem.CountPurchase = Convert.ToInt32(textBoxCount.Text);
                            }
                            componentCalculationItem.Count = (int?)((List<object>)grid.DataContext)[9];
                            if ((Seller)((ComboBox)grid.Children[7]).SelectedItem != null)
                            {
                                componentCalculationItem.SellerIdPurchase = ((Seller)((ComboBox)grid.Children[7]).SelectedItem).Id;
                            }
                            componentCalculationItem.SellerId = (int?)((List<object>)grid.DataContext)[10];
                            componentCalculationItem.ReservePurchase = textBoxReserve.Text;
                            componentCalculationItem.Reserve = (string)((List<object>)grid.DataContext)[11];
                            componentCalculationItem.NotePurchase = textBoxNote.Text;
                            componentCalculationItem.AssemblyMap = textBoxAssemblyMap.Text;
                            componentCalculationItem.ParentName = Convert.ToInt32(componentCalculationHeader.Id);
                            componentCalculationItem.IsDeleted = (bool)((List<object>)grid.DataContext)[4];
                            componentCalculationItem.IsAdded = (bool)((List<object>)grid.DataContext)[5];
                            componentCalculationItem.IsHeader = (bool)((List<object>)grid.DataContext)[2];
                            PULL.ComponentCalculation(componentCalculationItem);
                            ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculationItem.ProcurementId);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Дружище, ты сделал что-то неправильно...");
                }
            }
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
        }
        public static void AssemblyMapListViewInitialization(Procurement procurement, List<ComponentCalculation> componentCalculations, ListView listViewToInitialization)
        {
            Manufacturers = GET.View.Manufacturers();
            Sellers = GET.View.Sellers();
            ComponentStates = GET.View.ComponentStates();
            ComponentCalculations = componentCalculations;
            List<StackPanel> stackPanels = new();
            int idOfPosition = 1;
            foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
            {
                StackPanel stackPanel = new StackPanel();
                if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsDeleted == false)
                {
                    Grid grid = new Grid();
                    double[] columnWidths = { 400, 390 };
                    for (int i = 0; i < columnWidths.Length; i++)
                    {
                        ColumnDefinition columnDefinition = new ColumnDefinition();
                        columnDefinition.Width = new GridLength(columnWidths[i]);
                        grid.ColumnDefinitions.Add(columnDefinition);
                    }

                    TextBox textBoxHeader = new TextBox() { Style = (Style)Application.Current.FindResource("AssemblyMap.Header") };
                    if(componentCalculationHeader.ComponentHeaderType != null)
                        textBoxHeader.Text = componentCalculationHeader.ComponentHeaderType.Kind.ToString();
                    LoadColumnNames(textBoxHeader, 0);
                    TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("AssemblyMap.Header") };
                    LoadColumnNames(textBoxHeaderAssemblyMap, 1);
                    
                    Grid.SetColumn(textBoxHeader, 0);
                    Grid.SetColumn(textBoxHeaderAssemblyMap, 1);

                    grid.Children.Add(textBoxHeader);
                    grid.Children.Add(textBoxHeaderAssemblyMap);
                    stackPanel.Children.Add(grid);
                }

                ListView listView = new();
                listView.Style = (Style)Application.Current.FindResource("ListView");

                foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                {
                    if (componentCalculation.ParentName == componentCalculationHeader.Id && componentCalculation.ParentName != null && componentCalculation.IsDeleted == false)
                    {
                        Grid grid = new Grid();
                        double[] columnWidths = { 30, 365, 30, 100, 120, 140};

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }
                        TextBox textBoxIdOfPosition = new TextBox() { Text = idOfPosition.ToString(), Style = (Style)Application.Current.FindResource("AssemblyMap.Item") };
                        idOfPosition++;
                        TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentNamePurchase, Style = (Style)Application.Current.FindResource("AssemblyMap.Item") };
                        TextBox textBoxCount = new TextBox() { Text = componentCalculation.CountPurchase.ToString(), Style = (Style)Application.Current.FindResource("AssemblyMap.Item") };
                        TextBox textBoxSeller = new TextBox() { Style = (Style)Application.Current.FindResource("AssemblyMap.Item") };
                        foreach (Seller seller in Sellers)
                            if (seller.Id == componentCalculation.SellerIdPurchase)
                            {
                                textBoxSeller.Text = seller.Name;
                                break;
                            }
                        TextBox textBoxNote = new TextBox() { Text = componentCalculation.NotePurchase, Style = (Style)Application.Current.FindResource("AssemblyMap.Item") };
                        TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("AssemblyMap.Item") };

                        Grid.SetColumn(textBoxIdOfPosition, 0);
                        Grid.SetColumn(textBoxComponentName, 1);
                        Grid.SetColumn(textBoxCount, 2);
                        Grid.SetColumn(textBoxSeller, 3);
                        Grid.SetColumn(textBoxNote, 4);
                        Grid.SetColumn(textBoxAssemblyMap, 5);

                        grid.Children.Add(textBoxIdOfPosition);
                        grid.Children.Add(textBoxComponentName);
                        grid.Children.Add(textBoxCount);
                        grid.Children.Add(textBoxSeller);
                        grid.Children.Add(textBoxNote);
                        grid.Children.Add(textBoxAssemblyMap);

                        listView.Items.Add(grid);
                    }

                }
                stackPanel.Children.Add(listView);
                if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsAdded == false)
                {
                    stackPanels.Add(stackPanel);
                }
            }
            listViewToInitialization.ItemsSource = stackPanels;
        }
    }
}
