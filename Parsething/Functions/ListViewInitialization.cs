using DatabaseLibrary.Entities.ComponentCalculationProperties;
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

        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));
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
                    StackPanel stackPanel = new StackPanel { };
                    if (componentCalculationHeader.IsHeader == true)
                    {
                        Grid grid = new Grid();
                        double[] columnWidths = { 600, 400, 100 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }

                        TextBox textBoxHeader = new TextBox() { Text = componentCalculationHeader.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        Button buttonAdd = new Button();
                        buttonAdd.Click += (sender, e) => ButtonAdd_Click(sender, e, stackPanels);
                        buttonAdd.Content = "+";

                        Grid.SetColumn(textBoxHeader, 0);
                        Grid.SetColumn(textBoxHeaderAssemblyMap, 1);
                        Grid.SetColumn(buttonAdd, 2);

                        grid.Children.Add(textBoxHeader);
                        grid.Children.Add(textBoxHeaderAssemblyMap);
                        grid.Children.Add(buttonAdd);
                        stackPanel.Children.Add(grid);
                    }

                    ListView listView = new();
                    listView.Style = (Style)Application.Current.FindResource("ListView");

                    foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                    {
                        if (componentCalculation.ParentName == componentCalculationHeader.ComponentName && componentCalculation.ParentName != null)
                        {
                            Grid grid = new Grid();
                            double[] columnWidths = { 60, 450, 110, 70, 50, 85, 90, 150, 150 };

                            for (int i = 0; i < columnWidths.Length; i++)
                            {
                                ColumnDefinition columnDefinition = new ColumnDefinition();
                                columnDefinition.Width = new GridLength(columnWidths[i]);
                                grid.ColumnDefinitions.Add(columnDefinition);
                                Border border = new Border();
                                border.BorderThickness = new Thickness(1);
                                border.BorderBrush = (Brush)Application.Current.FindResource("Text.Foreground");
                                Grid.SetColumn(border, i);
                                grid.Children.Add(border);
                            }
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });

                            TextBox textBoxPartNumber = new TextBox() { Text = componentCalculation.PartNumber };
                            TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName };
                            ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName" };
                            foreach (Manufacturer manufacturer in Manufacturers)
                                if (manufacturer.Id == componentCalculation.ManufacturerId)
                                {
                                    comboBoxManufacturer.SelectedItem = manufacturer;
                                    break;
                                }
                            TextBox textBoxPrice = new TextBox() { Text = componentCalculation.Price.ToString() };
                            TextBox textBoxCount = new TextBox() { Text = componentCalculation.Count.ToString() };
                            ComboBox comboBoxSeller = new ComboBox() { ItemsSource = Sellers, DisplayMemberPath = "Name" };
                            foreach (Seller seller in Sellers)
                                if (seller.Id == componentCalculation.SellerId)
                                {
                                    comboBoxSeller.SelectedItem = seller;
                                    break;
                                }
                            TextBox textBoxReserve = new TextBox() { Text = componentCalculation.Reserve };
                            TextBox textBoxNote = new TextBox() { Text = componentCalculation.Note };
                            TextBox textBoxAssemblyMap = new TextBox() { Text = componentCalculation.AssemblyMap };
                            Button buttonDelete = new Button();
                            buttonDelete.Click += (sender, e) => ButtonAdd_Click(sender, e, stackPanels);
                            buttonDelete.Content = "Удалить";

                            Grid.SetColumn(textBoxPartNumber, 0);
                            Grid.SetColumn(textBoxComponentName, 1);
                            Grid.SetColumn(comboBoxManufacturer, 2);
                            Grid.SetColumn(textBoxPrice, 3);
                            Grid.SetColumn(textBoxCount, 4);
                            Grid.SetColumn(comboBoxSeller, 5);
                            Grid.SetColumn(textBoxReserve, 6);
                            Grid.SetColumn(textBoxNote, 7);
                            Grid.SetColumn(textBoxAssemblyMap, 8);

                            grid.Children.Add(textBoxPartNumber);
                            grid.Children.Add(textBoxComponentName);
                            grid.Children.Add(comboBoxManufacturer);
                            grid.Children.Add(textBoxPrice);
                            grid.Children.Add(textBoxCount);
                            grid.Children.Add(comboBoxSeller);
                            grid.Children.Add(textBoxReserve);
                            grid.Children.Add(textBoxNote);
                            grid.Children.Add(textBoxAssemblyMap);

                            listView.Items.Add(grid);
                        }

                    }
                    stackPanel.Children.Add(listView);
                    stackPanels.Add(stackPanel);
                }
                //PurchaseOrCalculatiing.Text = "Расчет";
                //CalculatingPanel.Visibility = Visibility.Visible;
                //ColumnsNamesCalculating.Visibility = Visibility.Visible;
                //PurchasePanel.Visibility = Visibility.Hidden;
                //ColumnsNamesPurchase.Visibility = Visibility.Hidden;


                //foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                //{
                //    if (componentCalculation.Price != null && componentCalculation.Count != null)
                //        calculatingAmount += (componentCalculation.Price * componentCalculation.Count);
                //}

                //if (calculatingAmount > Procurement.InitialPrice)
                //    CalculationPrice.Foreground = Red;
                //MaxPrice.Text = Procurement.InitialPrice.ToString();
                //CalculationPrice.Text = calculatingAmount.ToString();

            }
            else
            {
                    //PurchaseOrCalculatiing.Text = "Закупка";
                    //PurchasePanel.Visibility = Visibility.Visible;
                    //PurchaseListView.Visibility = Visibility.Visible;
                    //ColumnsNamesPurchase.Visibility = Visibility.Visible;
                    //CalculatingPanel.Visibility = Visibility.Hidden;
                    //CalculatingListView.Visibility = Visibility.Hidden;
                    //ColumnsNamesCalculating.Visibility = Visibility.Hidden;
                    //PurchaseListView.Items.Clear();

                    //PurchaseListView.ItemsSource = ComponentCalculations;

                    //foreach (ComponentCalculation componentCalculation in ComponentCalculations)
                    //{
                    //    if (componentCalculation.PricePurchase != null && componentCalculation.Count != null)
                    //        purchaseAmount += (componentCalculation.PricePurchase * componentCalculation.Count);
                    //}

                    //if (purchaseAmount > Procurement.ContractAmount)
                    //    PurchasePrice.Foreground = Red;
                    //ContractPrice.Text = Procurement.ContractAmount.ToString();
                    //PurchasePrice.Text = purchaseAmount.ToString();
            }
            ListView.ItemsSource = stackPanels;
        }
        private static void ButtonAdd_Click(object sender, RoutedEventArgs e, List<StackPanel> stackPanels)
        {
            StackPanel parentStackPanel = (StackPanel)((Grid)((Button)sender).Parent).Parent;
            Grid grid = (Grid)parentStackPanel.Children[0];
            ComponentCalculation newComponentCalculation = new ComponentCalculation
            {
                ParentName = ((TextBox)grid.Children[0]).Text
            };
            ComponentCalculations.Add(newComponentCalculation);
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
        }
        private static void ButtonDelete_Click(object sender, RoutedEventArgs e, List<StackPanel> stackPanels)
        {
            StackPanel parentStackPanel = (StackPanel)((Grid)((Button)sender).Parent).Parent;
            Grid grid = (Grid)parentStackPanel.Children[0];
            //ComponentCalculations.Remove((Grid)parentStackPanel.Children[0]);
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
        }
    }
}
