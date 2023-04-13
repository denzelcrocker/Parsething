namespace Parsething.Pages;

public partial class CalculatorPage : Page
{
    public CalculatorPage() =>
        InitializeComponent();

    private Frame MainFrame { get; set; } = null!;

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

        ProcurementsEmployeesCalculated = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Посчитан");
        if (ProcurementsEmployeesCalculated != null)
            Calculated.Text = ProcurementsEmployeesCalculated.Count.ToString();

        ProcurementsEmployeesDrawUp = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Оформить");
        if (ProcurementsEmployeesDrawUp != null)
            DrawUp.Text = ProcurementsEmployeesDrawUp.Count.ToString();
    }
}