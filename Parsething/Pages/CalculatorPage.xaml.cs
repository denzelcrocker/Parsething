using System.Diagnostics;
using System.Windows.Media;

namespace Parsething.Pages;

public partial class CalculatorPage : Page
{
    public CalculatorPage() =>
        InitializeComponent();

    private Frame MainFrame { get; set; } = null!;

    private List<ProcurementsEmployee>? ProcurementsEmployeesQueue { get; set; }
    private List<GET.ProcurementsEmployeesGrouping>? ProcurementsEmployeesGroupings { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesNew { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesCalculated { get; set; }
    private List<ProcurementsEmployee>? ProcurementsEmployeesDrawUp { get; set; }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
        catch { }

        ProcurementsEmployeesNew = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый");
        if (ProcurementsEmployeesNew != null)
        {
            View.ItemsSource = ProcurementsEmployeesNew;
            NewCount.Text = ProcurementsEmployeesNew.Count.ToString();
        }

        //ProcurementsEmployeesQueue = GET.View.ProcurementsEmployeesQueue();
        //if (ProcurementsEmployeesQueue != null)
        //    Queue.Text = ProcurementsEmployeesQueue.Count.ToString();

        ProcurementsEmployeesGroupings = GET.View.ProcurementsEmployeesGroupBy(((Employee)Application.Current.MainWindow.DataContext).Id);
        if (ProcurementsEmployeesGroupings != null)
            Overall.Text = ProcurementsEmployeesGroupings[0].CountOfProcurements.ToString();

        ProcurementsEmployeesCalculated = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан");
        if (ProcurementsEmployeesCalculated != null)
            Calculated.Text = ProcurementsEmployeesCalculated.Count.ToString();

        ProcurementsEmployeesDrawUp = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить");
        if (ProcurementsEmployeesDrawUp != null)
            DrawUp.Text = ProcurementsEmployeesDrawUp.Count.ToString();
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
    }

    private void CalculatedButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = ProcurementsEmployeesCalculated;
    }

    private void DrawUpButton_Click(object sender, RoutedEventArgs e)
    {
        View.ItemsSource = ProcurementsEmployeesDrawUp;
    }

    private void EditProcurement_Click(object sender, RoutedEventArgs e)
    {
        ProcurementsEmployee procurementsEmployee = (sender as Button)?.DataContext as ProcurementsEmployee;
        if(procurementsEmployee != null)
        {
            Procurement procurement = procurementsEmployee.Procurement;
            _ = MainFrame.Navigate(new CardOfProcurement(procurement, null, false));
        }
    }

    private void Calculating_Click(object sender, RoutedEventArgs e)
    {
        ProcurementsEmployee procurementsEmployee = (sender as Button)?.DataContext as ProcurementsEmployee;
        if (procurementsEmployee != null)
        {
            Procurement procurement = procurementsEmployee.Procurement;
            _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement,null, true, false));
        }
    }
}