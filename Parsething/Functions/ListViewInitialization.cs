using DatabaseLibrary.Entities.ComponentCalculationProperties;
using DatabaseLibrary.Entities.ProcurementProperties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using static Parsething.Pages.ComponentCalculationsPage;

namespace Parsething.Functions
{
    internal class ListViewInitialization
    {
        private static List<ComponentCalculation>? ComponentCalculations = new List<ComponentCalculation>();

        private static List<ComponentState>? ComponentStates = new List<ComponentState>();

        private static List<Manufacturer>? Manufacturers = new List<Manufacturer>();

        private static List<Seller>? Sellers = new List<Seller>();

        private static bool IsCalculation;

        private static ListView ListView;

        private static string[] columnNames = { "Заголовок", "Карта сборки" };

        public static void ComponentCalculationsListViewInitialization(bool isCalculation, List<ComponentCalculation> componentCalculations, ListView listViewToInitialization)
        {
            
            ListView = listViewToInitialization;
            IsCalculation = isCalculation;
            Manufacturers = GET.View.Manufacturers();
            Sellers = GET.View.Sellers();
            ComponentStates = GET.View.ComponentStates();
            ComponentCalculations = componentCalculations;
            List<StackPanel> stackPanels = new();

            if (IsCalculation)
            {

                foreach (ComponentCalculation componentCalculationHeader in componentCalculations)
                {
                    StackPanel stackPanel = new StackPanel();
                    if (componentCalculationHeader.IsHeader == true)
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
                        textBoxHeader.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeader, 0);
                        textBoxHeader.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeader, 0);
                        LoadColumnNames(textBoxHeader, 0);
                        TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeaderAssemblyMap.LostFocus += (sender, e) => TextBox_LostFocus(sender, e, textBoxHeaderAssemblyMap,1);
                        textBoxHeaderAssemblyMap.GotFocus += (sender, e) => TextBox_GotFocus(sender, e, textBoxHeaderAssemblyMap, 1);
                        LoadColumnNames(textBoxHeaderAssemblyMap, 1);
                        Button buttonAdd = new Button();
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.Content = "";
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
                        if (componentCalculation.ParentName == componentCalculationHeader.ComponentName && componentCalculation.ParentName != null)
                        {
                            Grid grid = new Grid() { DataContext = new List<object> { componentCalculation.Id, componentCalculation.ProcurementId ,componentCalculation.IsHeader, componentCalculation.ParentName } };
                            double[] columnWidths = { 60, 450, 115, 70, 50, 90, 90, 150, 150 };

                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                            TextBox textBoxPartNumber = new TextBox() { Text = componentCalculation.PartNumber, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") };
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerId)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.Price.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.Count.ToString(), Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name", Style = (Style)Application.Current.FindResource("ComboBoxBase.ComponentCalculationItem") }; 
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerId)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.Reserve, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.Note, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Item") };
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
                    if (componentCalculationHeader.IsHeader == true)
                    {
                        stackPanels.Add(stackPanel);
                    }
                }
            }
            else
            {

            }
            ListView.ItemsSource = null;
            ListView.ItemsSource = stackPanels;
        }

        private static void ButtonDeleteDivision_Click(object sender, RoutedEventArgs e)
        {
            if (IsCalculation)
            {
                int idToDelete = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[1]);
                for (int i = ComponentCalculations.Count - 1; i >= 0; i--)
                {
                    if (ComponentCalculations[i].Id == idToDelete)
                    {
                        ComponentCalculations.RemoveAt(i);
                        break; // Выходим из цикла после удаления элемента
                    }
                }
                DELETE.ComponentCalculation(idToDelete);
                ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
            }
        }

        private static void ButtonAddPosition_Click(object sender, RoutedEventArgs e)
        {
            StackPanel parentStackPanel = (StackPanel)((Grid)((Button)sender).Parent).Parent;
            Grid grid = (Grid)parentStackPanel.Children[0];
            ComponentCalculation newComponentCalculation = new ComponentCalculation
            {
                ProcurementId = Convert.ToInt32(((List<object>)((Grid)((Button)sender).Parent).DataContext)[0]),
                ParentName = ((TextBox)grid.Children[0]).Text,
                IsAdded = IsCalculation ? false : true,
                IsDeleted = false,
                IsHeader = false,
            };
            ComponentCalculations.Add(newComponentCalculation);
            PUT.ComponentCalculation(newComponentCalculation);
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
        }
        private static void ButtonDelete_Click(object sender, RoutedEventArgs e)
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
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
        }
        public static void ButtonAddDivision_Click(object sender, RoutedEventArgs e, Procurement procurement)
        {
            if (IsCalculation)
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
                ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
            }
            // добавить для закупки
        }
        private static void TextBox_LostFocus(object sender, RoutedEventArgs e, TextBox textBox, int headerId)
        {
            UpdateListView();
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = columnNames[headerId];
                textBox.Foreground = Brushes.Gray;
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
                    TextBox textBoxAssemblyMap = (TextBox)((Grid)stackPanel.Children[0]).Children[1];

                    componentCalculationHeader.Id = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[1];
                    componentCalculationHeader.ProcurementId = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[0];
                    componentCalculationHeader.ComponentName = textBoxHeader.Text;
                    componentCalculationHeader.AssemblyMap = textBoxAssemblyMap.Text;
                    componentCalculationHeader.IsDeleted = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[3];
                    componentCalculationHeader.IsAdded = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[4];
                    componentCalculationHeader.IsHeader = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[2]; ;
                    PULL.ComponentCalculation(componentCalculationHeader);
                    ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculationHeader.ProcurementId);
                    ListView innerListView = (ListView)stackPanel.Children[1];
                    foreach (Grid item in innerListView.Items)
                    {
                        ComponentCalculation componentCalculationItem = new ComponentCalculation();
                        componentCalculationItem.Id = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[1];
                        TextBox textBox = (TextBox)item.Children[1];
                        int comboBox = ((Seller)((ComboBox)item.Children[5]).Items.CurrentItem).Id;
                    }
                }
                catch 
                {

                }

            }
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
        }
    }
}
