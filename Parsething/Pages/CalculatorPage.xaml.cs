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

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public CalculatorPage()
    {
        InitializeComponent();
        DataContext = this;
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
        var procurementsEmployeesNew = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый");
        if (procurementsEmployeesNew != null && procurementsEmployeesNew.Count > 0)
            NewCount.Text = procurementsEmployeesNew.Count.ToString();
        var procurementsEmployeesCheck = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Проверка");
        if (procurementsEmployeesCheck != null && procurementsEmployeesCheck.Count > 0)
            CheckCount.Text = procurementsEmployeesCheck.Count.ToString();
        var procurementsQueue = GET.View.ProcurementsQueue();
        if (procurementsQueue != null && procurementsQueue.Count > 0)
            Queue.Text = procurementsQueue.Count.ToString();
        var procurementGroup = GET.View.ProcurementsEmployeesGroupBy(((Employee)Application.Current.MainWindow.DataContext).Id);
        if (procurementGroup != null && procurementGroup.Count > 0)
            Overall.Text = procurementGroup[0].CountOfProcurements.ToString();
        var procurementsEmployeeCalculated = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан");
        if (procurementsEmployeeCalculated != null && procurementsEmployeeCalculated.Count > 0)
            Calculated.Text = procurementsEmployeeCalculated.Count.ToString();
        var procurementsEmployeeDrawUp = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить");
        if (procurementsEmployeeDrawUp != null && procurementsEmployeeDrawUp.Count > 0)
            DrawUp.Text = procurementsEmployeeDrawUp.Count.ToString();
        var procurementsEmployeeWonPartOne = GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id);
        if (procurementsEmployeeWonPartOne != null && procurementsEmployeeWonPartOne.Count > 0)
            WonPartOne.Text = procurementsEmployeeWonPartOne.Count.ToString();
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
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.LightGray;
        CheckButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
        LoadPageData();
    }

    private void CalculatedButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.LightGray;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
        LoadPageData();
    }

    private void DrawUpButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.LightGray;
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
        PUT.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id);
        LoadPageData();
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
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id) ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline); 
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.Transparent;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.LightGray;
        LoadPageData();
    }

    private void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Проверка") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements.OrderBy(p => p.Deadline);
        NewButton.Background = Brushes.Transparent;
        CheckButton.Background = Brushes.LightGray;
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