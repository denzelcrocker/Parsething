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
        Password.Visibility = Visibility.Hidden;
        VisiblePassword.Visibility = Visibility.Visible;
        VisiblePassword.Text = Password.Password;

    }

    private void PasswordVisibility_Unchecked(object sender, RoutedEventArgs e)
    {
        Password.Visibility = Visibility.Visible;
        VisiblePassword.Visibility = Visibility.Hidden;
        Password.Password = VisiblePassword.Text;
    }

    private void EnterButton_Click(object sender, RoutedEventArgs e)
    {
        Employee? employee = GET.Employee(UserName.Text, Password.Password);
        if (employee != null)
        {
            MainWindow mainWindow = new() { DataContext = employee };
            mainWindow.Show();
            Close();
        }
        else
        {
            _ = MessageBox.Show("Вы ввели неверные данные!");
        }
    }

    private void Password_TextInput(object sender, TextCompositionEventArgs e)
    {
        VisiblePassword.Text = Password.Password;
    }

    private void VisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
    {
        Password.Password = VisiblePassword.Text;
    }
}