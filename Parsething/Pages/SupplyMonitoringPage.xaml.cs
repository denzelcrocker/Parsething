using DatabaseLibrary.Entities.ProcurementProperties;
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
using DatabaseLibrary.Queries;
using static DatabaseLibrary.Queries.GET;
using System.Windows.Controls.Primitives;
using Parsething.Classes;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для SupplyMonitoringPage.xaml
    /// </summary>
    public partial class SupplyMonitoringPage : Page
    {
        private List<SupplyMonitoringList> supplyMonitoringList { get; set; }
        private List<ComponentState> componentStates { get; set; }
        private List<Seller> sellers { get; set; }
        private decimal? overAllPrice = 0;
        private Frame MainFrame { get; set; } = null!;


        public SupplyMonitoringPage()
        {
            InitializeComponent();
            componentStates = GET.View.ComponentStates();
            sellers = GET.View.Sellers();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = Entry.ProcurementBy(Convert.ToInt32(((Button)sender).DataContext));
            NavigationService.Navigate(new ComponentCalculationsPage(procurement, false));
        }

        private Popup CreatePopup(Button targetButton, List<SupplyMonitoringList> supplyMonitoringList, string header)
        {
            var datePicker = new DatePicker
            {
                Name = "DatePicker",
                Style = (Style)Application.Current.FindResource("ComponentCalculations.DatePickerStyle"),
                Width = 150,
                Height = 40
            };

            var comboBox = new ComboBox
            {
                Name = "ComboBox",
                DisplayMemberPath = "Kind",
                Style = (Style)Application.Current.FindResource("EditableMainComboBox"),
                ItemsSource = componentStates,
                Width = 150,
                Height = 35
            };

            var saveButton = new Button
            {
                Content = "Save",
                Style = (Style)Application.Current.FindResource("BaseButton"),
                Width = 150,
                Height = 35,
                FontSize = 13,
                Tag = header,
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel
            {
                Children =
                {
                    datePicker,
                    comboBox,
                    saveButton
                }
            };

            var border = new Border
            {
                Background = (Brush)Application.Current.FindResource("TextBoxBackgroundBrush"),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Child = stackPanel
            };

            var popup = new Popup
            {
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                Placement = PlacementMode.Bottom,
                StaysOpen = false,
                PlacementTarget = targetButton,
                Child = border
            };

            saveButton.Tag = popup;
            saveButton.Click += (s, args) => SaveButton_Click(s, args, supplyMonitoringList, datePicker, comboBox);

            return popup;
        }

        // Обработчик события PreviewMouseWheel для ListView
        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Получаем родительский ScrollViewer
            var scrollViewer = FindVisualParent<ScrollViewer>((DependencyObject)sender);

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            }
        }

        // Метод для поиска родительского элемента указанного типа в визуальном дереве
        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;

            return FindVisualParent<T>(parentObject);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e, List<SupplyMonitoringList> supplyMonitoringList, DatePicker datePicker, ComboBox comboBox)
        {
            if (supplyMonitoringList == null) return;

            var procurementIds = supplyMonitoringList.Select(s => s.TenderNumber).Distinct().ToList();
            string header = (string)((Button)sender).DataContext;

            var componentStatuses = supplyMonitoringList.Select(s => s.ComponentStatus).Distinct().ToList();

            var componentCalculations = GET.View.ComponentCalculationsBy(procurementIds, componentStatuses);

            DateTime? selectedDate = datePicker.SelectedDate;
            string selectedStatus = ((comboBox.SelectedItem as ComponentState)?.Kind) ?? string.Empty;

            var sellerId = sellers.FirstOrDefault(s => s.Name == header)?.Id;

            var filteredComponentCalculations = componentCalculations.Where(cc => cc.SellerIdPurchase == sellerId).ToList();

            UpdateComponentCalculations(filteredComponentCalculations, selectedDate, selectedStatus);

            if (sender is Button button && button.Tag is Popup popup)
            {
                popup.IsOpen = false;
            }
        }
        private void UpdateComponentCalculations(List<ComponentCalculation> componentCalculations, DateTime? selectedDate, string selectedStatus)
        {
            foreach (var calculation in componentCalculations)
            {
                if (selectedDate.HasValue)
                {
                    calculation.Date = selectedDate.Value;
                }

                if (!string.IsNullOrEmpty(selectedStatus))
                {
                    using ParsethingContext db = new();
                    var state = db.ComponentStates.FirstOrDefault(s => s.Kind == selectedStatus);
                    if (state != null)
                    {
                        calculation.ComponentStateId = state.Id;
                    }
                }

                PULL.ComponentCalculation(calculation);
            }
        }

        private void CopyToClipBoardButton_Click(object sender, RoutedEventArgs e, string header, decimal? totalAmount)
        {
            if (supplyMonitoringList == null) return;

            var recordsForHeader = supplyMonitoringList.Where(s => s.SupplierName == header).GroupBy(s => new { s.ComponentName, s.ComponentStatus }).ToList();
            StringBuilder clipboardText = new StringBuilder();

            clipboardText.AppendLine($"{header} - {totalAmount}");
            foreach (var record in recordsForHeader)
            {
                clipboardText.AppendLine($"{record.Key.ComponentName}      {record.Sum(x => x.TotalCount)}");
            }

            Clipboard.SetText(clipboardText.ToString());
        }

        private void OverallInfoButton_Click(object sender, RoutedEventArgs e)
        {
            OverAllInfoPopUp.IsOpen = !OverAllInfoPopUp.IsOpen;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrEmpty(searchText))
                SearchSupplyMonitoringList(searchText);
            else
                CreateSupplyMonitoringList();
        }
        private void SearchSupplyMonitoringList(string searchText)
        {
            string dateType = (SupplyMonitoringListSelectionCombobox.SelectedItem as ComboBoxItem)?.Tag as string ?? string.Empty;
            var componentStatuses = new List<string>();

            switch (dateType)
            {
                case "CommonList":
                    componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    break;
                case "BySuppliers":
                    componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    break;
                case "WarehouseAndReserve":
                    componentStatuses = new List<string> { "На складе", "В резерве" };
                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    break;
                case "OnTheWay":
                    componentStatuses = new List<string> { "В пути" };
                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    break;
                case "ToBuy":
                    componentStatuses = new List<string> { "Купить", "Согласование средств", "Счет оплачен" };
                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    break;
                case "ExportToExcel":
                    break;
            }
            // Получаем все элементы из оригинального списка (группировка должна быть сохранена)
            var originalList = supplyMonitoringList
                .GroupBy(x => new { x.ComponentName, x.SupplierName, x.ComponentStatus })
                .ToList();

            // Фильтруем элементы по введенному тексту
            var filteredList = originalList.Where(group =>
                group.Key.ComponentName.ToLower().Contains(searchText.ToLower()) ||
                group.Key.SupplierName.ToLower().Contains(searchText.ToLower()) ||
                group.Key.ComponentStatus.ToLower().Contains(searchText.ToLower()) ||
                group.Sum(x => x.TotalCount).ToString().Contains(searchText) ||
                group.Sum(x => x.TotalAmount).ToString().Contains(searchText)
            ).ToList();

            // Преобразуем отфильтрованные группы обратно в список SupplyMonitoringList
            supplyMonitoringList = filteredList.SelectMany(group => group).ToList();


            // Оптимизированный switch
            switch (dateType)
            {
                case "CommonList":
                    overAllPrice = 0;
                    InitializeCommonSupplyMonitoringList();
                    break;
                case "BySuppliers":
                case "WarehouseAndReserve":
                case "OnTheWay":
                case "ToBuy":
                    overAllPrice = 0;
                    InitializeOtherSupplyMonitoringList();
                    break;
            }
        }

        private void SupplyMonitoringListSelectionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateSupplyMonitoringList();
        }
        private void CreateSupplyMonitoringList() 
        {
            string dateType = (SupplyMonitoringListSelectionCombobox.SelectedItem as ComboBoxItem)?.Tag as string ?? string.Empty;
            var componentStatuses = new List<string>();
            switch (dateType)
            {
                case "CommonList":
                    componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
                    overAllPrice = 0;

                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    InitializeCommonSupplyMonitoringList();
                    break;
                case "BySuppliers":
                    componentStatuses = new List<string> { "Купить", "Оплатить", "Транзит", "Наличие", "Заказ" };
                    overAllPrice = 0;

                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    InitializeOtherSupplyMonitoringList();
                    break;
                case "WarehouseAndReserve":
                    componentStatuses = new List<string> { "На складе", "В резерве" };
                    overAllPrice = 0;

                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    InitializeOtherSupplyMonitoringList();
                    break;
                case "OnTheWay":
                    componentStatuses = new List<string> { "В пути" };
                    overAllPrice = 0;

                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    InitializeOtherSupplyMonitoringList();
                    break;
                case "ToBuy":
                    componentStatuses = new List<string> { "Купить", "Согласование средств", "Счет оплачен" };
                    overAllPrice = 0;

                    supplyMonitoringList = View.GetSupplyMonitoringLists(GlobalUsingValues.Instance.Procurements, componentStatuses);
                    InitializeOtherSupplyMonitoringList();
                    break;
                case "ExportToExcel":
                    Functions.ExportToExcel.ExportSupplyMonitoringListToExcel(supplyMonitoringList);
                    break;
            }
        }
        private void InitializeCommonSupplyMonitoringList()
        {
            var groupedSupplyMonitoringList = supplyMonitoringList
                .GroupBy(x => new { x.ComponentName, x.SupplierName, x.ComponentStatus })
                .OrderBy(g => g.Key.ComponentName)
                .ToList();
            List<StackPanel> stackPanels = new();
            StackPanel stackPanel = new();

            ListView list = new();
            list.Style = (Style)Application.Current.FindResource("ListView");

            foreach (var group in groupedSupplyMonitoringList)
            {
                Grid grid = new Grid();
                double[] columnWidths = { 150, 780, 100, 150, 100, 100, 110, 170 };

                for (int i = 0; i < columnWidths.Length; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new GridLength(columnWidths[i]);
                    grid.ColumnDefinitions.Add(columnDefinition);
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    Grid.SetColumn(border, i);
                    grid.Children.Add(border);
                }
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });

                // Средняя цена по группе
                decimal averagePrice = group.Average(x => x.AveragePrice ?? 0);

                // Создаём TextBox для отображения данных группы
                TextBox textBlockManufacturerName = new TextBox() { Text = group.First().ManufacturerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockComponentName = new TextBox() { Text = group.First().ComponentName, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockComponentStatus = new TextBox() { Text = group.First().ComponentStatus, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockShipmentPlan = new TextBox() { Text = $"{group.First().ShipmentPlan}".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockAveragePrice = new TextBox() { Text = $"{averagePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockTotalCount = new TextBox() { Text = group.Sum(x => x.TotalCount).ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockSellerName = new TextBox() { Text = group.First().SupplierName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                TextBox textBlockTotalAmount = new TextBox() { Text = $"{group.Sum(x => x.TotalAmount):N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };

                // Добавляем элементы в grid
                Grid.SetColumn(textBlockManufacturerName, 0);
                Grid.SetColumn(textBlockComponentName, 1);
                Grid.SetColumn(textBlockComponentStatus, 2); // Статус компонента
                Grid.SetColumn(textBlockShipmentPlan, 3);
                Grid.SetColumn(textBlockAveragePrice, 4);
                Grid.SetColumn(textBlockTotalCount, 5);
                Grid.SetColumn(textBlockSellerName, 6);
                Grid.SetColumn(textBlockTotalAmount, 7);

                grid.Children.Add(textBlockManufacturerName);
                grid.Children.Add(textBlockComponentName);
                grid.Children.Add(textBlockComponentStatus);
                grid.Children.Add(textBlockShipmentPlan);
                grid.Children.Add(textBlockAveragePrice);
                grid.Children.Add(textBlockTotalCount);
                grid.Children.Add(textBlockSellerName);
                grid.Children.Add(textBlockTotalAmount);

                // Для каждой записи в группе создаём кнопку с номером тендера
                StackPanel buttonPanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(0, 2, 0, 2) }; // Вертикальная панель для кнопок с отступом

                foreach (var supplyMonitoring in group)
                {
                    Button button = new Button() 
                    {
                        Content = supplyMonitoring.DisplayId,
                        Style = (Style)Application.Current.FindResource("BaseButton"),
                        Height = 30,
                        FontSize = 13,
                        Width = 110,
                        Padding = new Thickness(0),
                        Margin = new Thickness(5, 1, 5, 1)
                    };
                    button.Click += Button_Click;
                    button.DataContext = supplyMonitoring.TenderNumber;

                    // Добавляем отступы между кнопками
                    button.Margin = new Thickness(0, 5, 0, 5);

                    // Добавляем кнопку в панель для кнопок
                    buttonPanel.Children.Add(button);
                }

                // Добавляем панель с кнопками в последнюю колонку
                Grid.SetColumn(buttonPanel, 8);
                grid.Children.Add(buttonPanel);

                if (group.First().ComponentStatus == "Купить")
                {
                    textBlockComponentStatus.Foreground = new SolidColorBrush(Colors.Red); // Устанавливаем красный цвет для статуса "Купить"
                }

                list.Items.Add(grid);
                overAllPrice += group.Sum(x => x.TotalAmount);
            }

            stackPanel.Children.Add(list);
            stackPanels.Add(stackPanel);
            listViewSupplyMonitoring.ItemsSource = stackPanels;
            OverAllInfoTextBlock.Text = overAllPrice.ToString() + " p.";
        }
        private void InitializeOtherSupplyMonitoringList()
        {
            List<string> headers = new();
            List<StackPanel> stackPanels = new();

            // Сбор уникальных поставщиков для заголовков
            foreach (SupplyMonitoringList supplyMonitoring in supplyMonitoringList)
            {
                if (!headers.Contains(supplyMonitoring.SupplierName))
                {
                    headers.Add(supplyMonitoring.SupplierName);
                }
            }

            // Обработка данных для каждого поставщика
            foreach (string header in headers)
            {
                StackPanel stackPanel = new StackPanel();
                DockPanel dockPanel = new DockPanel();

                decimal? totalAmount = 0;
                foreach (SupplyMonitoringList supplyMonitoringList in supplyMonitoringList)
                {
                    if (supplyMonitoringList.SupplierName == header)
                        totalAmount += supplyMonitoringList.TotalAmount;
                }

                // Добавляем заголовок поставщика
                stackPanel.Children.Add(dockPanel);
                dockPanel.Children.Add(new TextBlock()
                {
                    Text = $"\t{header} - {totalAmount:N2} р.",
                    Style = (Style)Application.Current.FindResource("TextBlock.SupplyMonitoring.Header"),
                    Width = 1650
                });

                // Кнопки действий
                Button saveButton = new Button()
                {
                    Content = "↓",
                    Style = (Style)Application.Current.FindResource("BaseButton"),
                    DataContext = header,
                    Height = 30,
                    Width = 60,
                    FontSize = 13,
                    Margin = new Thickness(10,0,0,0)
                };
                saveButton.Click += (s, args) =>
                {
                    string supplierHeader = (string)((Button)s).DataContext;
                    Popup popup = CreatePopup(saveButton, supplyMonitoringList, supplierHeader);
                    popup.IsOpen = true;
                };
                dockPanel.Children.Add(saveButton);

                Button copyToClipBoardButton = new Button()
                {
                    Content = "Copy",
                    Style = (Style)Application.Current.FindResource("BaseButton"),
                    Height = 30,
                    Width = 60,
                    Margin = new Thickness(0, 0, 5, 0),
                    FontSize = 13
                };
                copyToClipBoardButton.Click += (sender, e) =>
                {
                    CopyToClipBoardButton_Click(sender, e, header, totalAmount);
                };
                dockPanel.Children.Add(copyToClipBoardButton);

                // Создание ListView для списка записей поставщика
                ListView list = new ListView() { Style = (Style)Application.Current.FindResource("ListView") };

                // Группируем данные по наименованию и статусу
                var groupedByComponentAndStatus = supplyMonitoringList
                    .Where(s => s.SupplierName == header)
                    .GroupBy(s => new { s.ComponentName, s.ComponentStatus });

                // Отображаем сгруппированные записи
                foreach (var group in groupedByComponentAndStatus)
                {
                    Grid grid = new Grid();
                    double[] columnWidths = { 150, 780, 100, 150, 100, 100, 110, 160 };

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
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140) });


                    decimal averagePrice = group.Average(x => x.AveragePrice ?? 0);
                    // Добавляем колонки данных
                    TextBox textBlockManufacturerName = new TextBox() { Text = group.First().ManufacturerName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockComponentName = new TextBox() { Text = group.Key.ComponentName, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockComponentStatus = new TextBox() { Text = group.Key.ComponentStatus, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockShipmentPlan = new TextBox() { Text = $"{group.First().ShipmentPlan}".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockAveragePrice = new TextBox() { Text = $"{averagePrice:N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockTotalCount = new TextBox() { Text = group.Sum(x => x.TotalCount).ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockSellerName = new TextBox() { Text = group.First().SupplierName, Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };
                    TextBox textBlockTotalAmount = new TextBox() { Text = $"{group.Sum(x => x.TotalAmount):N2} р.".ToString(), Style = (Style)Application.Current.FindResource("SupplyMonitoringTextBox") };

                    // Добавляем элементы в grid
                    Grid.SetColumn(textBlockManufacturerName, 0);
                    Grid.SetColumn(textBlockComponentName, 1);
                    Grid.SetColumn(textBlockComponentStatus, 2); // Статус компонента
                    Grid.SetColumn(textBlockShipmentPlan, 3);
                    Grid.SetColumn(textBlockAveragePrice, 4);
                    Grid.SetColumn(textBlockTotalCount, 5);
                    Grid.SetColumn(textBlockSellerName, 6);
                    Grid.SetColumn(textBlockTotalAmount, 7);

                    grid.Children.Add(textBlockManufacturerName);
                    grid.Children.Add(textBlockComponentName);
                    grid.Children.Add(textBlockComponentStatus);
                    grid.Children.Add(textBlockShipmentPlan);
                    grid.Children.Add(textBlockAveragePrice);
                    grid.Children.Add(textBlockTotalCount);
                    grid.Children.Add(textBlockSellerName);
                    grid.Children.Add(textBlockTotalAmount);

                    // Контейнер для кнопок (в одну строку)
                    StackPanel buttonPanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(0, 2, 0, 2) }; // Вертикальная панель для кнопок с отступом

                    foreach (var supply in group)
                    {
                        Button button = new Button()
                        {
                            Content = supply.DisplayId,
                            Style = (Style)Application.Current.FindResource("BaseButton"),
                            Height = 30,
                            FontSize = 13,
                            Width = 120,
                            Padding = new Thickness(0),
                            DataContext = supply.TenderNumber,
                            Margin = new Thickness(5,1,5,1)
                        };
                        button.Click += Button_Click;
                        buttonPanel.Children.Add(button);
                    }

                    Grid.SetColumn(buttonPanel, 8);
                    grid.Children.Add(buttonPanel);

                    // Если статус "Купить", то выделяем красным цветом
                    if (group.Key.ComponentStatus == "Купить")
                    {
                        textBlockComponentStatus.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    list.Items.Add(grid);
                    overAllPrice += group.Sum(s => s.TotalAmount);
                }

                stackPanel.Children.Add(list);
                stackPanels.Add(stackPanel);
            }

            listViewSupplyMonitoring.ItemsSource = stackPanels;
            OverAllInfoTextBlock.Text = overAllPrice.ToString() + " р.";
        }
    }
}
