namespace Parsething.Windows;

public partial class AutorizationWindow : Window
{
    public AutorizationWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void MinimizeAction_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseAction_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void PasswordVisibility_Checked(object sender, RoutedEventArgs e)
    {

    }

    private void PasswordVisibility_Unchecked(object sender, RoutedEventArgs e)
    {

    }
}