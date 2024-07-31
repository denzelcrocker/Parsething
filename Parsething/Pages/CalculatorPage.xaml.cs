using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows.Media;

namespace Parsething.Pages;

public partial class CalculatorPage : Page
{
    public CalculatorPage()
    {
        InitializeComponent();
        LoadPageData();
    }


    private Frame MainFrame { get; set; } = null!;

    private List<Procurement>? ProcurementsQueue { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesGroupings { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesNew { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesCalculated { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesDrawUp { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesWonPartOne { get; set; }

    private DateTime StartDate = new DateTime();

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void LoadPageData()
    {
        try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
        catch { }

        var globalUsingValues = Classes.GlobalUsingValues.Instance;
        StartDate = globalUsingValues.StartDate;

        ProcurementsEmployeesNew = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый");
        if (ProcurementsEmployeesNew != null)
        {
            View.ItemsSource = ProcurementsEmployeesNew;
            NewCount.Text = ProcurementsEmployeesNew.Count.ToString();
        }

        ProcurementsQueue = GET.View.ProcurementsQueue();
        if (ProcurementsQueue != null)
            Queue.Text = ProcurementsQueue.Count.ToString();


        ProcurementsEmployeesGroupings = GET.View.ProcurementsEmployeesGroupBy(((Employee)Application.Current.MainWindow.DataContext).Id);
        if (ProcurementsEmployeesGroupings.Count != 0)
            Overall.Text = ProcurementsEmployeesGroupings[0].CountOfProcurements.ToString();

        ProcurementsEmployeesCalculated = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан");
        if (ProcurementsEmployeesCalculated != null)
            Calculated.Text = ProcurementsEmployeesCalculated.Count.ToString();

        ProcurementsEmployeesDrawUp = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить");
        if (ProcurementsEmployeesDrawUp != null)
            DrawUp.Text = ProcurementsEmployeesDrawUp.Count.ToString();

        ProcurementsEmployeesWonPartOne = GET.View.ProcurementsEmployeesBy("Выигран 1ч", StartDate, ((Employee)Application.Current.MainWindow.DataContext).Id);
        if (ProcurementsEmployeesWonPartOne != null)
            WonPartOne.Text = ProcurementsEmployeesWonPartOne.Count.ToString();

        NewButton.Background = Brushes.LightGray;
    }

    private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
    {
        ProcurementsEmployee procurementsEmployee = (sender as Button)?.DataContext as ProcurementsEmployee;
        if(procurementsEmployee != null)
        {
            Procurement procurement = procurementsEmployee.Procurement;
            string url = procurement.RequestUri.ToString();
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = ProcurementsEmployeesNew;
        NewButton.Background = Brushes.LightGray;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
    }

    private void CalculatedButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = ProcurementsEmployeesCalculated;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.LightGray;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.Transparent;
    }

    private void DrawUpButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = ProcurementsEmployeesDrawUp;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.LightGray;
        WonPartOneButton.Background = Brushes.Transparent;
    }

    private void EditProcurement_Click(object sender, RoutedEventArgs e)
    {
        ProcurementsEmployee procurementsEmployee = (sender as Button)?.DataContext as ProcurementsEmployee;
        if(procurementsEmployee != null)
        {
            Procurement procurement = procurementsEmployee.Procurement;
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
        ProcurementsEmployee procurementsEmployee = (sender as Button)?.DataContext as ProcurementsEmployee;
        if (procurementsEmployee != null)
        {
            Procurement procurement = procurementsEmployee.Procurement;
            _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true, false));
        }
    }

    private void WonPartOneButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = ProcurementsEmployeesWonPartOne;
        NewButton.Background = Brushes.Transparent;
        CalculatedButton.Background = Brushes.Transparent;
        DrawUpButton.Background = Brushes.Transparent;
        WonPartOneButton.Background = Brushes.LightGray;
    }
}