using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Parsething.Classes;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace Parsething.Pages;

public partial class CalculatorPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    SolidColorBrush buttonBrush = new SolidColorBrush();

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public CalculatorPage()
    {
        InitializeComponent();
        DataContext = this;
        string theme = UserConfig.LoadTheme();
        Color defaultColor = (Color)ColorConverter.ConvertFromString(theme == "Dark" ? "#383838" : "#D9D9D9");
        buttonBrush = new SolidColorBrush(defaultColor);
        LoadPageData();
    }


    private Frame MainFrame { get; set; } = null!;

    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesGroupings { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesNew { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesCalculated { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesDrawUp { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesWonPartOne { get; set; }

    private DateTime StartDate = new DateTime();

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
        catch { }

        NavigationState.AddLastSelectedProcurement(View);
    }

    private void LoadPageData()
    {
        var globalUsingValues = GlobalUsingValues.Instance;
        StartDate = globalUsingValues.StartDate;

        // Update Counts
        UpdateCount(NewCount, GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый", "Appoint"));
        UpdateCount(CheckCount, GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Проверка", "Appoint"));

        // Assuming you need to handle the queue differently, if it's not a List<ProcurementsEmployee>
        var procurementsQueue = GET.View.ProcurementsQueue();
        Queue.Text = procurementsQueue?.Count.ToString() ?? "0"; // Keep it simple if it's a different type

        var procurementGroup = GET.View.ProcurementsEmployeesGroupBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
        Overall.Text = procurementGroup?.FirstOrDefault()?.CountOfProcurements.ToString() ?? "0";

        UpdateCount(Calculated, GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан", "Appoint"));
        UpdateCount(DrawUp, GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить", "Appoint"));
        UpdateCount(WonPartOne, GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint"));
    }

    private void UpdateCount(TextBlock textBlock, List<ProcurementsEmployee> procurements)
    {
        textBlock.Text = procurements?.Count.ToString() ?? "0";
    }

    private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
    {
        Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
        if (procurement != null)
        {
            string url = procurement.RequestUri.ToString();
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый", "Appoint") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = buttonBrush;
        CheckButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
        LoadPageData();
    }

    private void CalculatedButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан", "Appoint") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.Transparent;
        CalculatedButton.Background = buttonBrush;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
        LoadPageData();
    }

    private void DrawUpButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить", "Appoint") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = buttonBrush;
        WonPartOneButton.Background = Brushes.Transparent;
        LoadPageData();
    }

    private void EditProcurement_Click(object sender, RoutedEventArgs e)
    {
        Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
        if (procurement != null)
        {
            NavigationState.LastSelectedProcurement = procurement;

            _ = MainFrame.Navigate(new CardOfProcurement(procurement));
        }
    }

    private void QueueButton_Click(object sender, RoutedEventArgs e)
    {
        PUT.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
        LoadPageData();
    }
    private void Image_MouseEnter(object sender, MouseEventArgs e)
    {
        var image = sender as FrameworkElement;

        if (image != null)
        {
            var parameter = image.Tag as string;
            ToolTipHelper.SetToolTip(image, parameter);
        }
    }

    private void Calculating_Click(object sender, RoutedEventArgs e)
    {
        Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
        if (procurement != null)
        {
            NavigationState.LastSelectedProcurement = procurement;

            _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true));
        }
    }

    private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline); 
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.Transparent;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = buttonBrush;
        LoadPageData();
    }

    private void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Проверка", "Appoint") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = buttonBrush;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
        LoadPageData();
    }

    private void ServiceId_MouseDown(object sender, MouseButtonEventArgs e)
    {
        TextBlock? textBlock = sender as TextBlock;
        if (textBlock != null)
        {
            Procurement? procurement = textBlock.DataContext as Procurement;
            if (procurement != null)
            {
                Clipboard.SetText(procurement.DisplayId.ToString());
                AutoClosingMessageBox.ShowAutoClosingMessageBox("Данные скопированы в буфер обмена", "Оповещение", 900);
            }
        }
    }
}