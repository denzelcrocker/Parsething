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
                    StackPanel stackPanel = new StackPanel();
                    if (componentCalculationHeader.IsHeader == true)
                    {
                        Grid grid = new Grid() { DataContext = new List<object> { componentCalculationHeader.ProcurementId, componentCalculationHeader.Id, componentCalculationHeader.IsHeader, componentCalculationHeader.IsDeleted, componentCalculationHeader. IsAdded} };
                        double[] columnWidths = { 600, 400, 100, 100 };
                        for (int i = 0; i < columnWidths.Length; i++)
                        {
                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(columnWidths[i]);
                            grid.ColumnDefinitions.Add(columnDefinition);
                        }

                        TextBox textBoxHeader = new TextBox() { Text = componentCalculationHeader.ComponentName, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeader.LostFocus += TextBox_LostFocus;
                        TextBox textBoxHeaderAssemblyMap = new TextBox() { Text = componentCalculationHeader.AssemblyMap, Style = (Style)Application.Current.FindResource("ComponentCalculation.Header") };
                        textBoxHeaderAssemblyMap.LostFocus += TextBox_LostFocus;
                        Button buttonAdd = new Button();
                        buttonAdd.Click += ButtonAddPosition_Click;
                        buttonAdd.Content = "+";
                        Button buttonDelete = new Button();
                        buttonDelete.Click += ButtonDeleteDivision_Click;
                        buttonDelete.Content = "-";

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
                            buttonDelete.Click += ButtonDelete_Click;
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
                    stackPanels.Add(stackPanel);
                }
            }
            else
            {

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
        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateListView();
        }
        private static void UpdateListView()
        {
            foreach (StackPanel stackPanel in ListView.Items)
            {
                ComponentCalculation componentCalculation = new ComponentCalculation();
                try
                {
                    TextBox textBoxHeader = (TextBox)((Grid)stackPanel.Children[0]).Children[0];
                    TextBox textBoxAssemblyMap = (TextBox)((Grid)stackPanel.Children[0]).Children[1];

                    componentCalculation.Id = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[1];
                    componentCalculation.ProcurementId = (int)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[0];
                    componentCalculation.ComponentName = textBoxHeader.Text;
                    componentCalculation.AssemblyMap = textBoxAssemblyMap.Text;
                    componentCalculation.IsDeleted = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[3];
                    componentCalculation.IsAdded = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[4];
                    componentCalculation.IsHeader = (bool)((List<object>)((Grid)stackPanel.Children[0]).DataContext)[2]; ;
                    PULL.ComponentCalculation(componentCalculation);
                    ComponentCalculations = GET.View.ComponentCalculationsBy(componentCalculation.ProcurementId);
                }
                catch 
                {

                    //// Проверяем наличие элементов в ListView
                    //ListView innerListView = (ListView)((StackPanel)stackPanel.Children[1]).Children[0];
                    //if (innerListView.Items.Count > 0)
                    //{
                    //    // Получаем первый элемент
                    //    Grid innerGrid = (Grid)innerListView.Items[0];
                    //    TextBox textBoxPartNumber = (TextBox)innerGrid.Children[0];
                    //    TextBox textBoxComponentName = (TextBox)innerGrid.Children[1];
                    //    ComboBox comboBoxManufacturer = (ComboBox)innerGrid.Children[2];
                    //    // Другие элементы UI внутри подзаголовка

                    //    // Далее можно выполнить необходимые операции с подзаголовком, например, отправить его на сервер

                    //    // Вам также нужно создать объект ComponentCalculation и заполнить его данными из подзаголовка,
                    //    // чтобы потом его можно было отправить на сервер
                    //}
                }

            }
            ComponentCalculationsListViewInitialization(IsCalculation, ComponentCalculations, ListView);
        }
    }
}
