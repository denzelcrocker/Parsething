namespace Parsething.Pages;

public partial class CalculatorPage : Page
{
    public CalculatorPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        List<ProcurementsEmployee>? procurementsEmployees = GET.View.ProcurementsEmployeesBy(((Employee)Application.Current.MainWindow.DataContext).Id, "Специалист отдела расчетов", "Получен");
        if (procurementsEmployees != null)
        {
            View.ItemsSource = procurementsEmployees;
            NewCount.Text = procurementsEmployees.Count.ToString();
        }
    }
}