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
    }

    private void LoadPageData()
    {
        var globalUsingValues = Classes.GlobalUsingValues.Instance;
        StartDate = globalUsingValues.StartDate;

        NewCount.Text = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый").Count.ToString();
        Queue.Text = GET.View.ProcurementsQueue().Count.ToString();
        Overall.Text = GET.View.ProcurementsEmployeesGroupBy(((Employee)Application.Current.MainWindow.DataContext).Id)[0].CountOfProcurements.ToString();
        Calculated.Text = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан").Count.ToString();
        DrawUp.Text = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить").Count.ToString();
        WonPartOne.Text = GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id).Count.ToString();
        View.ItemsSource = GlobalUsingValues.Instance.Procurements;
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
        View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        NewButton.Background = Brushes.LightGray;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
    }

    private void CalculatedButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.LightGray;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
    }

    private void DrawUpButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить") ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements; NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.LightGray;
        WonPartOneButton.Background = Brushes.Transparent;
    }

    private void EditProcurement_Click(object sender, RoutedEventArgs e)
    {
        Procurement procurement = (sender as Button)?.DataContext as Procurement ?? new Procurement();
        if (procurement != null)
        {
            _ = MainFrame.Navigate(new CardOfProcurement(procurement, false));
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
            _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true, false));
        }
    }

    private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = null;
        var procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id) ?? new List<ProcurementsEmployee>();
        GlobalUsingValues.Instance.AddProcurements(Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees));
        View.ItemsSource = GlobalUsingValues.Instance.Procurements; NewButton.Background = Brushes.Transparent;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.LightGray;
    }
}