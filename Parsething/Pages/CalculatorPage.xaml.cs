namespace Parsething.Pages;

public partial class CalculatorPage : Page
{
    private Frame MainFrame { get; set; } = null!;
    public CalculatorPage()
    {
        InitializeComponent();
    }
    List<ProcurementsEmployee>? procurementsEmployeesNew;
    List<ProcurementsEmployee>? procurementsEmployeesCalculated;
    List<ProcurementsEmployee>? procurementsEmployeesDrawUp;

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
        }
        catch { }
        procurementsEmployeesNew = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Новый");
        if (procurementsEmployeesNew != null)
        {
            View.ItemsSource = procurementsEmployeesNew;
            NewCount.Text = procurementsEmployeesNew.Count.ToString();
        }
        procurementsEmployeesCalculated = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан");
        if (procurementsEmployeesCalculated != null)
        {
            Calculated.Text = procurementsEmployeesCalculated.Count.ToString();
        }
        procurementsEmployeesDrawUp = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить");
        if (procurementsEmployeesDrawUp != null)
        {
            DrawUp.Text = procurementsEmployeesDrawUp.Count.ToString();
        }
    }
}