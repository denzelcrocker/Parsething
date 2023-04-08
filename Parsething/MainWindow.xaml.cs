namespace Parsething;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (((Employee)DataContext).Position.Kind == "Администратор")
        {
            _ = MainFrame.Navigate(new Pages.AdministratorPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Руководитель отдела расчетов" || ((Employee)DataContext).Position.Kind == "Заместитель руководителя отдела расчетов")
        {
            _ = MainFrame.Navigate(new Pages.HeadsOfCalculatorsPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Специалист отдела расчетов")
        {
            _ = MainFrame.Navigate(new Pages.CalculatorPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Руководитель тендерного отдела" || ((Employee)DataContext).Position.Kind == "Заместитель руководителя тендреного отдела")
        {
            _ = MainFrame.Navigate(new Pages.HeadsOfManagersPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Специалист по работе с электронными площадками")
        {
            _ = MainFrame.Navigate(new Pages.EPlatformSpecialistPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Специалист тендерного отдела")
        {
            _ = MainFrame.Navigate(new Pages.ManagerPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Руководитель отдела производства" || ((Employee)DataContext).Position.Kind == "Заместитель руководителя отдела производства" || ((Employee)DataContext).Position.Kind == "Специалист по производству")
        {
            _ = MainFrame.Navigate(new Pages.AssemblyPage());
        }
        else if (((Employee)DataContext).Position.Kind == "Юрист")
        {
            _ = MainFrame.Navigate(new Pages.LawyerPage());
        }
        else
        {

        }
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        switch (WindowState)
        {
            case WindowState.Normal:
                MaximizeAction.Content = "";
                break;
            case WindowState.Maximized:
                MaximizeAction.Content = "";
                break;
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        WindowState = WindowState.Normal;
        DragMove();
    }

    private void MinimizeAction_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeAction_Click(object sender, RoutedEventArgs e)
    {
        switch (WindowState)
        {
            case WindowState.Normal:
                WindowState = WindowState.Maximized;
                break;
            case WindowState.Maximized:
                WindowState = WindowState.Normal;
                break;
        }
    }

    private void CloseAction_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ProfilePreview_Click(object sender, RoutedEventArgs e)
    {

    }
}