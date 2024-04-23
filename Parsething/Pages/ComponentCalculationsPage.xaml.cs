using DatabaseLibrary.Entities.ProcurementProperties;
using OfficeOpenXml.ConditionalFormatting.Contracts;
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

        private List<ComponentState>? ComponentStates = new List<ComponentState>();

        private List<Manufacturer>? Manufacturers = new List<Manufacturer>();

        private List<Seller>? Sellers = new List<Seller>();

        SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xBD, 0x14, 0x14));

        private Procurement? Procurement { get; set; }

        private bool IsSearch;

        public ComponentCalculationsPage(Procurement procurement,List<Procurement> procurements ,bool isCalculation, bool isSearch)
        {
            InitializeComponent();
            IsSearch = isSearch;
            Procurements = procurements;
            Manufacturers = GET.View.Manufacturers();
            Sellers = GET.View.Sellers();
            ComponentStates = GET.View.ComponentStates();
            decimal? calculatingAmount = 0;
            decimal? purchaseAmount = 0;
            if(procurement != null)
            {
                Procurement = procurement;
                Id.Text = Procurement.Id.ToString();

                

                Comments = GET.View.CommentsBy(procurement.Id);
                CommentsListView.ItemsSource = Comments;


                ComponentCalculations = GET.View.ComponentCalculationsBy(procurement.Id);

                if (isCalculation)
                {
                    List<StackPanel> stackPanels = new();

                    foreach (ComponentCalculation componentCalculationHeader in ComponentCalculations)
                    {
                        StackPanel stackPanel = new StackPanel {};
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
                            buttonAdd.Click += ButtonAdd_Click;
                            buttonAdd.DataContext = "+";

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
                            if (componentCalculation.ParentName == componentCalculationHeader.ComponentName)
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

                                TextBox textBoxPartNumber = new TextBox() { Text = componentCalculation.PartNumber};
                                TextBox textBoxComponentName = new TextBox() { Text = componentCalculation.ComponentName };
                                ComboBox comboBoxManufacturer = new ComboBox() { ItemsSource = Manufacturers, DisplayMemberPath = "ManufacturerName"};
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
                    ComponentCalculationsListView.ItemsSource = stackPanels;
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
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            //StackPanel parentStackPanel = (StackPanel)((Button)sender).Parent;
            //Grid grid = (Grid)parentStackPanel.Children[0];
            //string partNumber = ((TextBox)grid.Children[0]).Text;
            //string componentName = ((TextBox)grid.Children[1]).Text;
            //string assemblyMap = ((TextBox)grid.Children[2]).Text;
            //ComponentCalculation newComponentCalculation = new ComponentCalculation
            //{
            //    PartNumber = partNumber,
            //    ComponentName = componentName,
            //    AssemblyMap = assemblyMap,
            //    // Устанавливаем остальные свойства по умолчанию или в зависимости от вашей логики
            //};

            //// Добавляем новый объект в список ComponentCalculations
            //ComponentCalculations.Add(newComponentCalculation);

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
