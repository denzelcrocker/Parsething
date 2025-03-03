using DatabaseLibrary.Entities.ComponentCalculationProperties;
using DatabaseLibrary.Entities.ProcurementProperties;
using Parsething.Classes;
using PdfSharp.Drawing.BarCodes;
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
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
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

        static List<string> ProcurementStates = new List<string>() { "Новый", "Посчитан", "Оформить", "Оформлен", "Отправлен", "Отмена", "Отклонен", "Проверка" };

        private static string[] columnNames = { "Заголовок", "Количество" };

        public static void ComponentCalculationsListViewInitialization(bool isCalculation, List<ComponentCalculation> componentCalculations, ListView listViewToInitialization, TextBlock calculationPriceTextBlock, TextBlock purchasePriceTextBlock, Procurement procurement)
        {
            int indexOfComponent = 1;
            ListView = listViewToInitialization;
            Procurement = procurement;
            CalculationPriceTextBlock = calculationPriceTextBlock;
            ComponentHeaderTypes = GET.View.ComponentHeaderTypes();
            PurchasePriceTextBlock = purchasePriceTextBlock;
            IsCalculation = isCalculation;
            Manufacturers = GET.View.Manufacturers().OrderBy(m => m.ManufacturerName).ToList();
            Sellers = GET.View.Sellers();
            ComponentStates = GET.View.ComponentStates();
            ComponentCalculations = componentCalculations.OrderBy(cc => cc.IndexOfComponent).ToList();

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
                        double[] columnWidths = { 628, 523, 105, 105 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }

                        ComboBox comboBoxHeader = new ComboBox() { ItemsSource = ComponentHeaderTypes, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("EditableComponentCalculationsHeaderComboBox") };
                        foreach (ComponentHeaderType componentHeaderType in ComponentHeaderTypes)
                            if (componentHeaderType.Id == componentCalculationHeader.HeaderTypeId)
                            {
                                comboBoxHeader.SelectedItem = componentHeaderType;
                                break;
                            }
                        comboBoxHeader.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        comboBoxHeader.LostFocus += (sender, e) => ComboBox_LostFocus(sender, e, comboBoxHeader, 0, true);
                        comboBoxHeader.GotFocus += (sender, e) => ComboBox_GotFocus(sender, e, comboBoxHeader, 0);
                        LoadColumnNames(comboBoxHeader, 0);
                        TextBox textBoxHeaderCount = new TextBox() { Text = componentCalculationHeader.Count.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox") };
                        textBoxHeaderCount.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        textBoxHeaderCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeaderCount, 1, true);
                        textBoxHeaderCount.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeaderCount, 1);
                        LoadColumnNames(textBoxHeaderCount, 1);
                        Button buttonAdd = new Button();
                        buttonAdd.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.MouseRightButtonUp += ButtonAdd_MouseRightButtonUp;
                        buttonAdd.Content = "";
                        buttonAdd.Style = (Style)Application.Current.FindResource("BaseButton");
                        buttonAdd.FontFamily = new FontFamily("Segoe Fluent Icons");
                        buttonAdd.Width = 100;
                        buttonAdd.Height = 50;
                        buttonAdd.FontSize = 20;
                        buttonAdd.FontWeight = FontWeights.Medium;
                        Button buttonDelete = new Button();
                        buttonDelete.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                        buttonDelete.Click += ButtonDeleteDivision_Click;
                        buttonDelete.Content = "";
                        buttonDelete.Style = (Style)Application.Current.FindResource("BaseButton");
                        buttonDelete.FontFamily = new FontFamily("Segoe Fluent Icons");
                        buttonDelete.Width = 100;
                        buttonDelete.Height = 50;
                        buttonDelete.FontSize = 20;
                        buttonDelete.FontWeight = FontWeights.Medium;


                        Grid.SetColumn(comboBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderCount, 1);
                        Grid.SetColumn(buttonAdd, 2);
                        Grid.SetColumn(buttonDelete, 3);

                        grid.Children.Add(comboBoxHeader);
                        grid.Children.Add(textBoxHeaderCount);
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
                            double[] columnWidths = { 60, 450, 115, 75, 40, 115, 115, 177, 150 };

                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            Button buttonMoveUp = new Button();
                            buttonMoveUp.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            buttonMoveUp.Click += ButtonMoveUp_Click;
                            buttonMoveUp.Content = "";
                            buttonMoveUp.Margin = new Thickness (0, 0, 35, 11);
                            buttonMoveUp.Style = (Style)Application.Current.FindResource("TitleBarButton");
                            Button buttonMoveDown = new Button();
                            buttonMoveDown.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            buttonMoveDown.Click += ButtonMoveDown_Click;
                            buttonMoveDown.Content = "";
                            buttonMoveDown.Margin = new Thickness(0, 11, 35, 0);
                            buttonMoveDown.Style = (Style)Application.Current.FindResource("TitleBarButton");
                            TextBox textBoxIndex = new TextBox() { Name = "textBoxIndex", Text = indexOfComponent.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true, HorizontalContentAlignment = HorizontalAlignment.Center };
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxComponentName.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxComponentName.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);

                            ComboBox comboBoxManufacturer = new ComboBox()
                            {
                                Style = (Style)Application.Current.FindResource("EditableMainComboBox"),
                                ItemsSource = Manufacturers,
                                DisplayMemberPath = "ManufacturerName"
                            };
                            comboBoxManufacturer.SelectedValuePath = "Id";
                            comboBoxManufacturer.SelectedValue = componentCalculation.ManufacturerId;
                            comboBoxManufacturer.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            VirtualizingStackPanel.SetIsVirtualizing(comboBoxManufacturer, true);
                            VirtualizingStackPanel.SetVirtualizationMode(comboBoxManufacturer, VirtualizationMode.Recycling);
                            ScrollViewer.SetIsDeferredScrollingEnabled(comboBoxManufacturer, true);

                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.Price.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxPrice.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            if (componentCalculation.Price != null && componentCalculation.Count != null)
                                calculatingAmount += (componentCalculation.Price * componentCalculation.Count);
                            textBoxPrice.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.Count.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxCount.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("EditableMainComboBox") };
                            comboBoxSeller.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerId)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.Reserve, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxReserve.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxReserve.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.Note, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxNote.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxNote.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxAssemblyMap.IsReadOnly = !ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            textBoxAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            Button buttonDelete = new Button();
                            buttonDelete.IsEnabled = ProcurementStates.Contains(Procurement.ProcurementState.Kind);
                            buttonDelete.Click += ButtonDelete_Click;
                            buttonDelete.Content = "";
                            buttonDelete.Style = (Style)Application.Current.FindResource("BaseButton");
                            buttonDelete.FontFamily = new FontFamily("Segoe Fluent Icons");
                            buttonDelete.Width = 25;
                            buttonDelete.Height = 25;
                            buttonDelete.FontSize = 12;
                            buttonDelete.Padding = new Thickness(0);
                            buttonDelete.FontWeight = FontWeights.Medium;
                            Button buttonCopy = new Button();
                            buttonCopy.Click += ButtonCopy_Click;
                            buttonCopy.Content = "";
                            buttonCopy.Style = (Style)Application.Current.FindResource("BaseButton");
                            buttonCopy.FontFamily = new FontFamily("Segoe Fluent Icons");
                            buttonCopy.Width = 25;
                            buttonCopy.Height = 25;
                            buttonCopy.FontSize = 12;
                            buttonCopy.Padding = new Thickness(0);
                            buttonCopy.FontWeight = FontWeights.Medium;


                            Grid.SetColumn(textBoxIndex, 0);
                            Grid.SetColumn(buttonMoveUp, 0);
                            Grid.SetColumn(buttonMoveDown, 0);
                            Grid.SetColumn(textBoxComponentName, 1);
                            Grid.SetColumn(comboBoxManufacturer, 2);
                            Grid.SetColumn(textBoxPrice, 3);
                            Grid.SetColumn(textBoxCount, 4);
                            Grid.SetColumn(comboBoxSeller, 5);
                            Grid.SetColumn(textBoxReserve, 6);
                            Grid.SetColumn(textBoxNote, 7);
                            Grid.SetColumn(textBoxAssemblyMap, 8);
                            Grid.SetColumn(buttonDelete, 9);
                            Grid.SetColumn(buttonCopy, 10);

                            grid.Children.Add(textBoxIndex);
                            grid.Children.Add(textBoxComponentName);
                            grid.Children.Add(comboBoxManufacturer);
                            grid.Children.Add(textBoxPrice);
                            grid.Children.Add(textBoxCount);
                            grid.Children.Add(comboBoxSeller);
                            grid.Children.Add(textBoxReserve);
                            grid.Children.Add(textBoxNote);
                            grid.Children.Add(textBoxAssemblyMap);
                            grid.Children.Add(buttonDelete);
                            grid.Children.Add(buttonMoveUp);
                            grid.Children.Add(buttonMoveDown);
                            grid.Children.Add(buttonCopy);

                            listView.Items.Add(grid);

                            indexOfComponent++;
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
                        double[] columnWidths = { 628, 523, 105, 105 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }
                        ComboBox comboBoxHeader = new ComboBox() { ItemsSource = ComponentHeaderTypes, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("EditableComponentCalculationsHeaderComboBox") };
                        foreach (ComponentHeaderType componentHeaderType in ComponentHeaderTypes)
                            if (componentHeaderType.Id == componentCalculationHeader.HeaderTypeId)
                            {
                                comboBoxHeader.SelectedItem = componentHeaderType;
                                break;
                            }
                        comboBoxHeader.LostFocus += (sender, e) => ComboBox_LostFocus(sender, e, comboBoxHeader, 0, true);
                        comboBoxHeader.GotFocus += (sender, e) => ComboBox_GotFocus(sender, e, comboBoxHeader, 0);
                        LoadColumnNames(comboBoxHeader, 0);
                        TextBox textBoxHeaderCount = new TextBox() { Text = componentCalculationHeader.CountPurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox") };
                        textBoxHeaderCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeaderCount, 1, true);
                        textBoxHeaderCount.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeaderCount, 1);
                        LoadColumnNames(textBoxHeaderCount, 1);
                        Button buttonAdd = new Button();
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.MouseRightButtonUp += ButtonAdd_MouseRightButtonUp;
                        buttonAdd.Content = "";
                        buttonAdd.FontFamily = new FontFamily("Segoe Fluent Icons");
                        buttonAdd.Style = (Style)Application.Current.FindResource("BaseButton");
                        buttonAdd.Width = 100;
                        buttonAdd.Height = 50;
                        buttonAdd.FontSize = 20;
                        buttonAdd.FontWeight = FontWeights.Medium;

                        Button buttonDelete = new Button();
                        buttonDelete.Click += ButtonDeleteDivision_Click;
                        buttonDelete.Content = "";
                        buttonDelete.FontFamily = new FontFamily("Segoe Fluent Icons");
                        buttonDelete.Style = (Style)Application.Current.FindResource("BaseButton");
                        buttonDelete.Width = 100;
                        buttonDelete.Height = 50;
                        buttonDelete.FontSize = 20;
                        buttonDelete.FontWeight = FontWeights.Medium;

                        Grid.SetColumn(comboBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderCount, 1);
                        Grid.SetColumn(buttonAdd, 2);
                        Grid.SetColumn(buttonDelete, 3);

                        grid.Children.Add(comboBoxHeader);
                        grid.Children.Add(textBoxHeaderCount);
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
                            double[] columnWidths = { 55, 300, 100, 125, 100, 75, 40, 105, 122, 125, 178 };
                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            Button buttonMoveUp = new Button();
                            buttonMoveUp.Click += ButtonMoveUp_Click;
                            buttonMoveUp.Content = "";
                            buttonMoveUp.Margin = new Thickness(0, 0, 35, 11);
                            buttonMoveUp.Style = (Style)Application.Current.FindResource("TitleBarButton");
                            Button buttonMoveDown = new Button();
                            buttonMoveDown.Click += ButtonMoveDown_Click;
                            buttonMoveDown.Content = "";
                            buttonMoveDown.Margin = new Thickness(0, 11, 35, 0);
                            buttonMoveDown.Style = (Style)Application.Current.FindResource("TitleBarButton");
                            TextBox textBoxIndex = new TextBox() { Name = "textBoxIndex", Text = indexOfComponent.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true, HorizontalContentAlignment = HorizontalAlignment.Center };
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentNamePurchase, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            Label replacementLabel = new Label() { Content = "i", Style = (Style)Application.Current.FindResource("ComponentCalculation.Label") };
                            ToolTip tooltip = new ToolTip();
                            tooltip.Style = (Style)Application.Current.FindResource("ComponentCalculation.ToolTip");
                            tooltip.Content = string.IsNullOrEmpty(componentCalculation.ComponentName) ? "Добавлено в закупке" : $"{componentCalculation.ComponentName} - {componentCalculation.Price} р.";
                            replacementLabel.ToolTip = tooltip;
                            if (componentCalculation.ComponentName != componentCalculation.ComponentNamePurchase)
                                tooltip.Content += " (Позиция изменена)";
                            textBoxComponentName.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName", Style = (Style)Application.Current.FindResource("EditableMainComboBox"), };
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerIdPurchase)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            ComboBox comboBoxComponentState = new ComboBox() { ItemsSource = ComponentStates, DisplayMemberPath = "Kind", Style = (Style)Application.Current.FindResource("EditableMainComboBox") };
                            foreach (ComponentState componentState in ComponentStates)
                                if (componentState.Id == componentCalculation.ComponentStateId)
                                {
                                    comboBoxComponentState.SelectedItem = componentState;
                                    break;
                                }
                            ComponentStatusToColorConverter statusToColorConverter = new ComponentStatusToColorConverter();
                            var histories = GET.View.HistoriesBy(componentCalculation.Id, "ComponentCalculation");

                            // Формируем текст для ToolTip
                            string toolTipText = string.Join(Environment.NewLine, histories.Select(h => $"{h.Date} - {h.Text}"));

                            Ellipse ellipseComponentState = new Ellipse
                            {
                                Width = 8,
                                Height = 8,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Margin = new Thickness(4, 0, 0, 0),
                                ToolTip = new ToolTip
                                {
                                    Content = toolTipText,
                                    Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
                                }
                            };
                            Binding binding = new Binding
                            {
                                Path = new PropertyPath("SelectedItem.Kind"),
                                Source = comboBoxComponentState,
                                Converter = statusToColorConverter
                            };
                            ellipseComponentState.SetBinding(Ellipse.FillProperty, binding);
                            DatePicker datePicker = new DatePicker() { SelectedDate = componentCalculation.Date, Style = (Style)Application.Current.FindResource("ComponentCalculations.DatePickerStyle") };
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.PricePurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            if (componentCalculation.PricePurchase != null && componentCalculation.CountPurchase != null)
                                purchaseAmount += (componentCalculation.PricePurchase * componentCalculation.CountPurchase);
                            textBoxPrice.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.CountPurchase.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxCount.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("EditableMainComboBox") };
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerIdPurchase)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.ReservePurchase, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxReserve.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.NotePurchase, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxNote.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };
                            textBoxAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, null, 0, false);
                            Button buttonDelete = new Button();
                            buttonDelete.Click += ButtonDelete_Click;
                            buttonDelete.Content = "";
                            buttonDelete.Style = (Style)Application.Current.FindResource("BaseButton");
                            buttonDelete.FontFamily = new FontFamily("Segoe Fluent Icons");
                            buttonDelete.Width = 25;
                            buttonDelete.Height = 25;
                            buttonDelete.FontSize = 12;
                            buttonDelete.Padding = new Thickness(0);
                            buttonDelete.FontWeight = FontWeights.Medium;

                            Grid.SetColumn(textBoxIndex, 0);
                            Grid.SetColumn(buttonMoveUp, 0);
                            Grid.SetColumn(buttonMoveDown, 0);
                            Grid.SetColumn(textBoxComponentName, 1);
                            Grid.SetColumn(replacementLabel, 1);
                            Grid.SetColumn(comboBoxManufacturer, 2);
                            Grid.SetColumn(comboBoxComponentState, 3);
                            Grid.SetColumn(ellipseComponentState, 3);
                            Grid.SetColumn(datePicker, 4);
                            Grid.SetColumn(textBoxPrice, 5);
                            Grid.SetColumn(textBoxCount, 6);
                            Grid.SetColumn(comboBoxSeller, 7);
                            Grid.SetColumn(textBoxReserve, 8);
                            Grid.SetColumn(textBoxNote, 9);
                            Grid.SetColumn(textBoxAssemblyMap, 10);
                            Grid.SetColumn(buttonDelete, 11);

                            grid.Children.Add(textBoxIndex);
                            grid.Children.Add(textBoxComponentName);
                            grid.Children.Add(replacementLabel);
                            grid.Children.Add(comboBoxManufacturer);
                            grid.Children.Add(comboBoxComponentState);
                            grid.Children.Add(ellipseComponentState);
                            grid.Children.Add(datePicker);
                            grid.Children.Add(textBoxPrice);
                            grid.Children.Add(textBoxCount);
                            grid.Children.Add(comboBoxSeller);
                            grid.Children.Add(textBoxReserve);
                            grid.Children.Add(textBoxNote);
                            grid.Children.Add(textBoxAssemblyMap);
                            grid.Children.Add(buttonDelete);
                            grid.Children.Add(buttonMoveUp);
                            grid.Children.Add(buttonMoveDown);

                            listView.Items.Add(grid);

                            indexOfComponent++;
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
                    else
                        PurchasePriceTextBlock.Foreground = Black;
                }
                else
                {
                    if (purchaseAmount > Procurement.ContractAmount)
                        PurchasePriceTextBlock.Foreground = Red;
                    else
                        PurchasePriceTextBlock.Foreground = Black;
                }

                PurchasePriceTextBlock.Text = purchaseAmount.ToString();
                Procurement.PurchaseAmount = purchaseAmount;
                PULL.Procurement(Procurement);
            }
            ListView.ItemsSource = stackPanels;
        }
        private static void ButtonAdd_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                ContextMenu contextMenu = CreateContextMenu();
                contextMenu.PlacementTarget = button;
                contextMenu.StaysOpen = true;
                contextMenu.IsOpen = true;

                contextMenu.DataContext = button.DataContext;
            }
        }
        private static ContextMenu CreateContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu
            {
                Style = (Style)Application.Current.FindResource("RoundedContextMenuStyle")
            };

            var componentTypes = GET.View.ComponentTypes();

            foreach (var componentType in componentTypes)
            {
                MenuItem typeMenuItem = new MenuItem
                {
                    Header = componentType.Kind,
                };

                foreach (var predefinedComponent in componentType.PredefinedComponent)
                {
                    MenuItem componentMenuItem = new MenuItem
                    {
                        Header = $"{predefinedComponent.ComponentName} - {predefinedComponent.Price} р.",
                        DataContext = predefinedComponent,
                        Style = (Style)Application.Current.FindResource("RoundedMenuItemStyle")
                         
                    };
                    componentMenuItem.Click += PredefinedComponentMenuItem_Click;
                    typeMenuItem.Items.Add(componentMenuItem);
                }

                contextMenu.Items.Add(typeMenuItem);
            }

            return contextMenu;
        }

        private static void PredefinedComponentMenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (sender is MenuItem menuItem)
            {
                PredefinedComponent predefinedComponent = menuItem.DataContext as PredefinedComponent;
                var parentMenuItem = menuItem.Parent as MenuItem;
                if (parentMenuItem != null)
                {
                    var contextMenu = parentMenuItem.Parent as ContextMenu;
                    var lastComponent = ComponentCalculations
                    .OrderByDescending(cc => cc.IndexOfComponent)
                    .FirstOrDefault();

                    int newIndexOfComponent = lastComponent != null
                        ? (lastComponent.IndexOfComponent ?? 0) + 1
                        : 1;
                    if (contextMenu != null)
                    {
                        var dataContext = (List<object>)(contextMenu).DataContext;
                        var procurementId = Convert.ToInt32(dataContext[0]);
                        var parentName = Convert.ToInt32(dataContext[1]);

                        ComponentCalculation newComponentCalculationHeader = new ComponentCalculation
                        {
                            ProcurementId = procurementId,
                            IndexOfComponent = newIndexOfComponent,
                            ComponentName = predefinedComponent.ComponentName,
                            ComponentNamePurchase = predefinedComponent.ComponentName,
                            ManufacturerId = predefinedComponent.ManufacturerId,
                            ManufacturerIdPurchase = predefinedComponent.ManufacturerId,
                            Price = predefinedComponent.Price,
                            PricePurchase = predefinedComponent.Price,
                            ParentName = parentName,
                            IsAdded = IsCalculation ? false : true,
                            IsDeleted = false,
                            IsHeader = false
                        };

                        PUT.ComponentCalculation(newComponentCalculationHeader);

                        UpdateComponentCalculationListView(null, null);
                    }
                }
            }
        }
        
        private static void ButtonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var grid = button?.Parent as Grid;

            var textBlock = grid?.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "textBoxIndex");

            if (textBlock != null && int.TryParse(textBlock.Text, out int index))
            {
                if (index > 0)
                {
                    var listView = grid?.Parent as ListView;

                    var previousGrid = listView?.Items.OfType<Grid>()
                        .FirstOrDefault(g => g.Children.OfType<TextBox>().Any(tb => tb.Name == "textBoxIndex" && int.Parse(tb.Text) == index - 1));

                    if (previousGrid != null)
                    {

                        textBlock.Text = (index - 1).ToString();
                        previousGrid.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "textBoxIndex").Text = index.ToString();

                        UpdateComponentCalculationListView(null, null);
                    }
                }
            }

        }

        private static void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var grid = button?.Parent as Grid;

            var textBlock = grid?.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "textBoxIndex");

            if (textBlock != null && int.TryParse(textBlock.Text, out int index))
            {
                var listView = grid?.Parent as ListView;

                if (index < ComponentCalculations.Count - 1)
                {
                    var nextGrid = listView?.Items.OfType<Grid>()
                        .FirstOrDefault(g => g.Children.OfType<TextBox>().Any(tb => tb.Name == "textBoxIndex" && int.Parse(tb.Text) == index + 1));

                    if (nextGrid != null)
                    {
                        textBlock.Text = (index + 1).ToString();
                        nextGrid.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "textBoxIndex").Text = index.ToString();

                        UpdateComponentCalculationListView(null, null);
                    }
                }
            }
        }
        private static void ButtonDeleteDivision_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Точно удалить?", "Удаление", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
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
                UpdateComponentCalculationListView(null, null);
            }
            else if (messageBoxResult == MessageBoxResult.No) { }
        }

        private static void ButtonAddPosition_Click(object sender, RoutedEventArgs e)
        {
            Procurement = GET.Entry.ProcurementBy(Procurement.Id);

            if ((IsCalculation && Procurement.CalculatingUserId == ((Employee)Application.Current.MainWindow.DataContext).Id) ||
                (!IsCalculation && Procurement.PurchaseUserId == ((Employee)Application.Current.MainWindow.DataContext).Id))
            {
                var dataContext = (List<object>)((Grid)((Button)sender).Parent).DataContext;
                var procurementId = Convert.ToInt32(dataContext[0]);
                var parentName = Convert.ToInt32(dataContext[1]);

                var lastComponent = ComponentCalculations
                    .OrderByDescending(cc => cc.IndexOfComponent)
                    .FirstOrDefault();

                int newIndexOfComponent = lastComponent != null
                    ? (lastComponent.IndexOfComponent ?? 0) + 1  
                    : 1;

                var countHeaderTextBox = ((Grid)((Button)sender).Parent).Children[1] as TextBox;


                int? countOfComponentCalculation = null;

                if (int.TryParse(countHeaderTextBox.Text, out int result))
                {
                    countOfComponentCalculation = result;
                }
                ComponentCalculation newComponentCalculation = new ComponentCalculation
                {
                    ProcurementId = procurementId,
                    Count = countOfComponentCalculation,
                    CountPurchase = countOfComponentCalculation,
                    IndexOfComponent = newIndexOfComponent,
                    ParentName = parentName,
                    IsAdded = IsCalculation ? false : true,
                    IsDeleted = false,
                    IsHeader = false,
                };

                ComponentCalculations.Add(newComponentCalculation);
                PUT.ComponentCalculation(newComponentCalculation);
            }
            UpdateComponentCalculationListView(null, null);
        }
        private static void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Точно удалить?", "Удаление", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
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
                            //ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
                            break;
                        }
                    }
                }
                ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement); // на всякий оставлю коммент

                //UpdateComponentCalculationListView(null, null);
            }
            else { }
        }
        private static void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            string componentCalculationName = (((Grid)((Button)sender).Parent)?.DataContext as List<object>)?[6]?.ToString() ?? "Пустое значение"; int manufacturerId = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[7]);
            if (componentCalculationName != "" && manufacturerId != null)
            {
                string clipboardText = "";
                Manufacturer? manufacturer = GET.Entry.Manufacturer(manufacturerId);
                clipboardText += $"{componentCalculationName} (производитель «{manufacturer?.FullManufacturerName}», {manufacturer?.ManufacturerCountry?.Name})";
                Clipboard.SetText(clipboardText);
                AutoClosingMessageBox.ShowAutoClosingMessageBox($"Данные скопированы в буфер обмена.", "Оповещение", 900);
            }

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
            UpdateComponentCalculationListView(null, null);
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
        public static bool UpdateComponentCalculationListView(DatePicker? sameDatePicker, ComboBox? sameComboBox)
        {
            bool successSave = true;
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
                    TextBox textBoxHeaderCount = (TextBox)((Grid)stackPanel.Children[0]).Children[1];

                    componentCalculationHeader.Id = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[1];
                    componentCalculationHeader.ProcurementId = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[0];
                    if (IsCalculation)
                    {
                        if (int.TryParse(textBoxHeaderCount.Text, out int count))
                        {
                            componentCalculationHeader.Count = count;
                            componentCalculationHeader.CountPurchase = count;
                        }
                    }
                    else
                    {
                        if (int.TryParse(textBoxHeaderCount.Text, out int countPurchase))
                        {
                            componentCalculationHeader.CountPurchase = countPurchase;
                        }
                    }
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
                            TextBox textBoxIndex = (TextBox)grid.Children[0];
                            TextBox textBoxComponentName = (TextBox)grid.Children[1];
                            TextBox textBoxPrice = (TextBox)grid.Children[3];
                            TextBox textBoxCount = (TextBox)grid.Children[4];
                            TextBox textBoxReserve = (TextBox)grid.Children[6];
                            TextBox textBoxNote = (TextBox)grid.Children[7];
                            TextBox textBoxAssemblyMap = (TextBox)grid.Children[8];

                            componentCalculationItem.Id = (int)((List<object>)grid.DataContext)[0];
                            componentCalculationItem.ProcurementId = (int)((List<object>)grid.DataContext)[1];
                            componentCalculationItem.IndexOfComponent = Convert.ToInt32(textBoxIndex.Text);
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
                        else if (!IsCalculation)
                        {
                            ComponentCalculation componentCalculationItem = new ComponentCalculation();
                            TextBox textBoxIndex = (TextBox)grid.Children[0];
                            TextBox textBoxComponentName = (TextBox)grid.Children[1];
                            DatePicker datePicker = (DatePicker)grid.Children[6];
                            TextBox textBoxPrice = (TextBox)grid.Children[7];
                            TextBox textBoxCount = (TextBox)grid.Children[8];
                            TextBox textBoxReserve = (TextBox)grid.Children[10];
                            TextBox textBoxNote = (TextBox)grid.Children[11];
                            TextBox textBoxAssemblyMap = (TextBox)grid.Children[12];

                            componentCalculationItem.Id = (int)((List<object>)grid.DataContext)[0];
                            componentCalculationItem.ProcurementId = (int)((List<object>)grid.DataContext)[1];
                            componentCalculationItem.IndexOfComponent = Convert.ToInt32(textBoxIndex.Text);
                            componentCalculationItem.ComponentName = (string)((List<object>)grid.DataContext)[6];
                            componentCalculationItem.ComponentNamePurchase = textBoxComponentName.Text;
                            if ((Manufacturer)((ComboBox)grid.Children[3]).SelectedItem != null)
                            {
                                componentCalculationItem.ManufacturerIdPurchase = ((Manufacturer)((ComboBox)grid.Children[3]).SelectedItem).Id;
                            }
                            componentCalculationItem.ManufacturerId = (int?)((List<object>)grid.DataContext)[7];
                            if (sameComboBox != null && sameComboBox.SelectedItem != null)
                                componentCalculationItem.ComponentStateId = ((ComponentState)sameComboBox.SelectedItem).Id;
                            else
                            {
                                if ((ComponentState)((ComboBox)grid.Children[4]).SelectedItem != null)
                                {
                                    componentCalculationItem.ComponentStateId = ((ComponentState)((ComboBox)grid.Children[4]).SelectedItem).Id;
                                }
                            }
                            if (sameDatePicker != null && sameDatePicker.SelectedDate != null)
                                componentCalculationItem.Date = sameDatePicker.SelectedDate;
                            else
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
                            if ((Seller)((ComboBox)grid.Children[9]).SelectedItem != null)
                            {
                                componentCalculationItem.SellerIdPurchase = ((Seller)((ComboBox)grid.Children[9]).SelectedItem).Id;
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
                    successSave = false;
                    AutoClosingMessageBox.ShowAutoClosingMessageBox($"Дружище, ты сделал что-то неправильно...", "Информация", 1000);
                    break;
                }
            }
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView, CalculationPriceTextBlock, PurchasePriceTextBlock, Procurement);
            return successSave;
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

                    TextBox textBoxHeader = new TextBox() { Style = (Style)Application.Current.FindResource("AssemblyMapHeaderTextBox") };
                    if (componentCalculationHeader.ComponentHeaderType != null)
                        textBoxHeader.Text = componentCalculationHeader.ComponentHeaderType.Kind.ToString();
                    LoadColumnNames(textBoxHeader, 0);
                    TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("AssemblyMapHeaderTextBox") };
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
                        double[] columnWidths = { 30, 365, 30, 360};

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }
                        TextBox textBoxIdOfPosition = new TextBox() { Text = idOfPosition.ToString(), Style = (Style)Application.Current.FindResource("AssemblyMapTextBox") };
                        idOfPosition++;
                        TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentNamePurchase, Style = (Style)Application.Current.FindResource("AssemblyMapTextBox") };
                        TextBox textBoxCount = new TextBox() { Text = componentCalculation.CountPurchase.ToString(), Style = (Style)Application.Current.FindResource("AssemblyMapTextBox") };
                        TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("AssemblyMapTextBox") };

                        Grid.SetColumn(textBoxIdOfPosition, 0);
                        Grid.SetColumn(textBoxComponentName, 1);
                        Grid.SetColumn(textBoxCount, 2);
                        Grid.SetColumn(textBoxAssemblyMap, 3);

                        grid.Children.Add(textBoxIdOfPosition);
                        grid.Children.Add(textBoxComponentName);
                        grid.Children.Add(textBoxCount);
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
        public static void RemainingComponentCalculationsListViewInitialization(List<ComponentCalculation> componentCalculations, ListView listViewToInitialization, Procurement procurement)
        {
            List<StackPanel> stackPanels = new List<StackPanel>();
            int counterOfComponentCalculations = 1;


            // Получаем все дочерние тендеры и их компоненты
            var childProcurements = GET.View.ApplicationsBy(procurement.DisplayId);
            var childProcurementComponents = new List<ComponentCalculation>();
            foreach (var childProcurement in childProcurements)
            {
                var components = GET.View.ComponentCalculationsBy(childProcurement.Id);
                if (components != null)
                {
                    childProcurementComponents.AddRange(components);
                }
            }

            foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
            {
                StackPanel stackPanel = new StackPanel();
                if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsDeleted == false)
                {
                    Grid grid = new Grid() { DataContext = componentCalculationHeader };
                    double[] columnWidths = { 370, 55, 55 };
                    for (int i = 0; i < columnWidths.Length; i++)
                    {
                        ColumnDefinition columnDefinition = new ColumnDefinition();
                        columnDefinition.Width = new GridLength(columnWidths[i]);
                        grid.ColumnDefinitions.Add(columnDefinition);
                    }

                    TextBox textBoxHeader = new TextBox() { Text = componentCalculationHeader.ComponentHeaderType.Kind, Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox"), IsReadOnly = true };
                    LoadColumnNames(textBoxHeader, 0);
                    TextBox textBoxHeaderCount = new TextBox() { Text = componentCalculationHeader.Count.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox") };

                    // Вычисляем остаток для заголовка
                    var parentComponent = componentCalculations.FirstOrDefault(pc => pc.HeaderTypeId == componentCalculationHeader.HeaderTypeId && pc.IsDeleted == false);
                    var usedCount = childProcurementComponents.Where(c => c.ComponentName == componentCalculationHeader.ComponentName && c.Count == componentCalculationHeader.Count && c.IsDeleted == false).Sum(c => c.Count);
                    var remainingCount = parentComponent != null ? parentComponent.Count - usedCount : componentCalculationHeader.Count;

                    TextBox textBoxHeaderRemainingCount = new TextBox() { Text = procurement.IsUnitPrice == true ? "-" : remainingCount.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox"), IsReadOnly = true };

                    Grid.SetColumn(textBoxHeader, 0);
                    Grid.SetColumn(textBoxHeaderCount, 1);
                    Grid.SetColumn(textBoxHeaderRemainingCount, 2);

                    grid.Children.Add(textBoxHeader);
                    grid.Children.Add(textBoxHeaderCount);
                    grid.Children.Add(textBoxHeaderRemainingCount);
                    stackPanel.Children.Add(grid);
                }

                ListView listView = new();
                listView.Style = (Style)Application.Current.FindResource("ListView");

                foreach (ComponentCalculation componentCalculation in componentCalculations)
                {
                    if (componentCalculation.ParentName == componentCalculationHeader.Id && componentCalculation.ParentName != null && componentCalculation.IsDeleted == false)
                    {
                        Grid grid = new Grid() { DataContext = componentCalculation };
                        double[] columnWidths = { 40, 327, 55, 55 };

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }
                        

                        // Вычисляем остаток для компонента
                        var parentComponent = componentCalculations.FirstOrDefault(pc => pc.ComponentName == componentCalculation.ComponentName && pc.Count == componentCalculation.Count && pc.IsDeleted == false && pc.SellerId == componentCalculation.SellerId);
                        var usedCount = childProcurementComponents.Where(c => c.ComponentName == componentCalculation.ComponentName && c.Count == componentCalculation.Count && c.IsDeleted == false && c.SellerId == componentCalculation.SellerId).Sum(c => c.Count);
                        var remainingCount = parentComponent != null ? parentComponent.Count - usedCount : componentCalculation.Count;

                        TextBox textBoxCounter = new TextBox() { Text = counterOfComponentCalculations.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true };
                        TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true };
                        TextBox textBoxCount = new TextBox() { Text = remainingCount.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox") };

                        TextBox textBoxRemainingCount = new TextBox() { Text = procurement.IsUnitPrice == true ? "-" : remainingCount.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true };

                        Grid.SetColumn(textBoxCounter, 0);
                        Grid.SetColumn(textBoxComponentName, 1);
                        Grid.SetColumn(textBoxCount, 2);
                        Grid.SetColumn(textBoxRemainingCount, 3);

                        grid.Children.Add(textBoxCounter);
                        grid.Children.Add(textBoxComponentName);
                        grid.Children.Add(textBoxCount);
                        grid.Children.Add(textBoxRemainingCount);

                        listView.Items.Add(grid);

                        counterOfComponentCalculations++;
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
        public static void CardRemainingComponentCalculationsListViewInitialization(List<ComponentCalculation> componentCalculations, ListView listViewToInitialization, Procurement procurement)
        {
            List<StackPanel> stackPanels = new List<StackPanel>();
            int counterOfComponentCalculations = 1;


            var childProcurements = GET.View.ApplicationsBy(procurement.DisplayId);
            var childProcurementComponents = new List<ComponentCalculation>();
            foreach (var childProcurement in childProcurements)
            {
                var components = GET.View.ComponentCalculationsBy(childProcurement.Id);
                if (components != null)
                {
                    childProcurementComponents.AddRange(components);
                }
            }

            foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
            {
                StackPanel stackPanel = new StackPanel();
                if (componentCalculationHeader.IsHeader == true && componentCalculationHeader.IsDeleted == false)
                {
                    Grid grid = new Grid() { DataContext = componentCalculationHeader };
                    double[] columnWidths = { 550, 55};
                    for (int i = 0; i < columnWidths.Length; i++)
                    {
                        ColumnDefinition columnDefinition = new ColumnDefinition();
                        columnDefinition.Width = new GridLength(columnWidths[i]);
                        grid.ColumnDefinitions.Add(columnDefinition);
                    }

                    TextBox textBoxHeader = new TextBox()
                    {
                        Text = string.IsNullOrEmpty(componentCalculationHeader?.ComponentHeaderType?.Kind) ? "Необходимо выбрать заголовок!!!" : componentCalculationHeader.ComponentHeaderType.Kind,
                        Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox"),
                        IsReadOnly = true
                    };
                    LoadColumnNames(textBoxHeader, 0);

                    var parentComponent = componentCalculations.FirstOrDefault(pc => pc.HeaderTypeId == componentCalculationHeader.HeaderTypeId && pc.IsDeleted == false);
                    var usedCount = childProcurementComponents.Where(c => c.ComponentName == componentCalculationHeader.ComponentName && c.Count == componentCalculationHeader.Count && c.IsDeleted == false).Sum(c => c.Count);
                    var remainingCount = parentComponent != null ? parentComponent.Count - usedCount : componentCalculationHeader.Count;

                    TextBox textBoxHeaderRemainingCount = new TextBox() { Text = procurement.IsUnitPrice == true ? "-" : remainingCount.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationHeaderTextBox"), IsReadOnly = true };

                    Grid.SetColumn(textBoxHeader, 0);
                    Grid.SetColumn(textBoxHeaderRemainingCount, 1);

                    grid.Children.Add(textBoxHeader);
                    grid.Children.Add(textBoxHeaderRemainingCount);
                    stackPanel.Children.Add(grid);
                }

                ListView listView = new();
                listView.Style = (Style)Application.Current.FindResource("ListView");

                foreach (ComponentCalculation componentCalculation in componentCalculations)
                {
                    if (componentCalculation.ParentName == componentCalculationHeader.Id && componentCalculation.ParentName != null && componentCalculation.IsDeleted == false)
                    {
                        Grid grid = new Grid() { DataContext = componentCalculation };
                        double[] columnWidths = { 40, 508, 54};

                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }
                        TextBox textBoxCounter = new TextBox() { Text = counterOfComponentCalculations.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true };
                        TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true };

                        var parentComponent = componentCalculations.FirstOrDefault(pc => pc.ComponentName == componentCalculation.ComponentName && pc.Count == componentCalculation.Count && pc.IsDeleted == false && pc.SellerId == componentCalculation.SellerId);
                        var usedCount = childProcurementComponents.Where(c => c.ComponentName == componentCalculation.ComponentName && c.Count == componentCalculation.Count && c.IsDeleted == false && c.SellerId == componentCalculation.SellerId).Sum(c => c.Count);
                        var remainingCount = parentComponent != null ? parentComponent.Count - usedCount : componentCalculation.Count;

                        TextBox textBoxRemainingCount = new TextBox() { Text = procurement.IsUnitPrice == true ? "-" : remainingCount.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculationItemTextBox"), IsReadOnly = true };

                        Grid.SetColumn(textBoxCounter, 0);
                        Grid.SetColumn(textBoxComponentName, 1);
                        Grid.SetColumn(textBoxRemainingCount, 2);

                        grid.Children.Add(textBoxCounter);
                        grid.Children.Add(textBoxComponentName);
                        grid.Children.Add(textBoxRemainingCount);

                        listView.Items.Add(grid);

                        counterOfComponentCalculations++;
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
        public static void CopyComponentCalculationsToNewProcurement(Procurement newProcurement, ListView listViewToInitialization)
        {
            decimal? totalCalculatingAmount = 0;
            decimal? totalPurchaseAmount = 0;

            foreach (StackPanel stackPanel in listViewToInitialization.Items)
            {
                foreach (var child in stackPanel.Children)
                {
                    if (child is Grid headerGrid)
                    {
                        var componentCalculationHeader = (ComponentCalculation)headerGrid.DataContext;

                        TextBox countTextBox = null;
                        foreach (var gridChild in headerGrid.Children)
                        {
                            if (gridChild is TextBox textBox && Grid.GetColumn(textBox) == 1)
                            {
                                countTextBox = textBox;
                                break;
                            }
                        }

                        if (countTextBox != null && int.TryParse(countTextBox.Text, out int countPurchase))
                        {
                            ComponentCalculation newComponentCalculationHeader = new ComponentCalculation
                            {
                                ProcurementId = newProcurement.Id,
                                PartNumber = componentCalculationHeader.PartNumber,
                                HeaderTypeId = componentCalculationHeader.HeaderTypeId,
                                ComponentName = componentCalculationHeader.ComponentName,
                                ComponentNamePurchase = componentCalculationHeader.ComponentName,
                                ManufacturerId = componentCalculationHeader.ManufacturerId,
                                ManufacturerIdPurchase = componentCalculationHeader.ManufacturerId,
                                Count = countPurchase,
                                CountPurchase = countPurchase,
                                Price = componentCalculationHeader.Price,
                                PricePurchase = componentCalculationHeader.Price,
                                SellerId = componentCalculationHeader.SellerId,
                                SellerIdPurchase = componentCalculationHeader.SellerId,
                                Note = componentCalculationHeader.Note,
                                NotePurchase = componentCalculationHeader.Note,
                                AssemblyMap = componentCalculationHeader.AssemblyMap,
                                IsDeleted = componentCalculationHeader.IsDeleted,
                                IsHeader = componentCalculationHeader.IsHeader,
                                IsAdded = componentCalculationHeader.IsAdded,
                            };
                            PUT.ComponentCalculation(newComponentCalculationHeader);
                            int newHeaderId = newComponentCalculationHeader.Id;

                            if (stackPanel.Children.OfType<ListView>().FirstOrDefault() is ListView innerListView)
                            {
                                foreach (Grid grid in innerListView.Items)
                                {
                                    var componentCalculation = (ComponentCalculation)grid.DataContext;

                                    TextBox childCountTextBox = null;
                                    foreach (var gridChild in grid.Children)
                                    {
                                        if (gridChild is TextBox textBox && Grid.GetColumn(textBox) == 2)
                                        {
                                            childCountTextBox = textBox;
                                            break;
                                        }
                                    }

                                    if (childCountTextBox != null && int.TryParse(childCountTextBox.Text, out int childCountPurchase))
                                    {
                                        ComponentCalculation newComponentCalculation = new ComponentCalculation
                                        {
                                            ParentName = newHeaderId,
                                            PartNumber = componentCalculation.PartNumber,
                                            ComponentName = componentCalculation.ComponentName,
                                            ComponentNamePurchase = componentCalculation.ComponentName,
                                            ManufacturerId = componentCalculation.ManufacturerId,
                                            ManufacturerIdPurchase = componentCalculation.ManufacturerId,
                                            Count = childCountPurchase,
                                            CountPurchase = childCountPurchase,
                                            Price = componentCalculation.Price,
                                            PricePurchase = componentCalculation.Price,
                                            SellerId = componentCalculation.SellerId,
                                            SellerIdPurchase = componentCalculation.SellerId,
                                            Note = componentCalculation.Note,
                                            NotePurchase = componentCalculation.Note,
                                            AssemblyMap = componentCalculation.AssemblyMap,
                                            ProcurementId = newProcurement.Id,
                                            IsDeleted = componentCalculation.IsDeleted,
                                            IsHeader = componentCalculation.IsHeader,
                                            IsAdded = componentCalculation.IsAdded,
                                        };

                                        PUT.ComponentCalculation(newComponentCalculation);

                                        totalCalculatingAmount += componentCalculation.Price * childCountPurchase;
                                        totalPurchaseAmount += componentCalculation.Price * childCountPurchase;
                                    }
                                    else
                                    {
                                        // Handle case where childCountTextBox.Text is empty or not a valid integer
                                        // You may choose to log this or handle it differently based on your application's logic
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Handle case where countTextBox.Text is empty or not a valid integer
                            // You may choose to log this or handle it differently based on your application's logic
                        }
                    }
                }
            }
            newProcurement.CalculatingAmount = totalCalculatingAmount;
            newProcurement.PurchaseAmount = totalPurchaseAmount;
            PULL.Procurement(newProcurement);
        }
        public static List<string> CheckComponentCalculationsLimits(ListView listViewToInitialization)
        {
            var exceededComponents = new List<string>();

            foreach (StackPanel stackPanel in listViewToInitialization.Items)
            {
                foreach (var child in stackPanel.Children)
                {
                    if (child is Grid headerGrid)
                    {
                        var componentCalculationHeader = (ComponentCalculation)headerGrid.DataContext;

                        TextBox countTextBox = null;
                        TextBox remainingCountTextBox = null;
                        foreach (var gridChild in headerGrid.Children)
                        {
                            if (gridChild is TextBox textBox && Grid.GetColumn(textBox) == 1)
                            {
                                countTextBox = textBox;
                                break;
                            }
                        }
                        foreach (var gridChild in headerGrid.Children)
                        {
                            if (gridChild is TextBox remainingTextBox && Grid.GetColumn(remainingTextBox) == 2)
                            {
                                remainingCountTextBox = remainingTextBox;
                                break;
                            }
                        }

                        if (countTextBox != null && remainingCountTextBox != null)
                        {
                            if (int.TryParse(countTextBox.Text, out int countPurchase) && int.TryParse(remainingCountTextBox.Text, out int remainingCountPurchase))
                            {
                                if (remainingCountPurchase < countPurchase)
                                {
                                    exceededComponents.Add($"{componentCalculationHeader.ComponentNamePurchase}: количество {countPurchase}, остаток {remainingCountPurchase}");
                                }
                            }
                            else if (countTextBox.Text == "")
                            {
                                exceededComponents.Add($"В позиции {componentCalculationHeader.ComponentHeaderType.Kind} не указано количество. Если остаток пустой, укажите 0");
                            }
                        }

                        if (stackPanel.Children.OfType<ListView>().FirstOrDefault() is ListView innerListView)
                        {
                            foreach (Grid grid in innerListView.Items)
                            {
                                var componentCalculation = (ComponentCalculation)grid.DataContext;

                                TextBox childCountTextBox = null;
                                TextBox childRemainingCountTextBox = null;
                                foreach (var gridChild in grid.Children)
                                {
                                    if (gridChild is TextBox textBox && Grid.GetColumn(textBox) == 2)
                                    {
                                        childCountTextBox = textBox;
                                        break;
                                    }
                                }
                                foreach (var gridChild in grid.Children)
                                {
                                    if (gridChild is TextBox remainingTextBox && Grid.GetColumn(remainingTextBox) == 3)
                                    {
                                        childRemainingCountTextBox = remainingTextBox;
                                        break;
                                    }
                                }

                                if (childCountTextBox != null && childRemainingCountTextBox != null)
                                {
                                    if (int.TryParse(childCountTextBox.Text, out int childCountPurchase) && int.TryParse(childRemainingCountTextBox.Text, out int childRemainingCountPurchase))
                                    {
                                        if (childRemainingCountPurchase < childCountPurchase)
                                        {
                                            exceededComponents.Add($"{componentCalculation.ComponentNamePurchase}: количество {childCountPurchase}, остаток {childRemainingCountPurchase}");
                                        }
                                    }
                                    else if (childCountTextBox.Text == "")
                                    {
                                        exceededComponents.Add($"В позиции {componentCalculation.ComponentName} не указано количество. Если остаток пустой, укажите 0");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return exceededComponents;
        }

    }
}