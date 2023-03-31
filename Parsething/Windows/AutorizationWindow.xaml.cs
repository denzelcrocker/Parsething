using DatabaseLibrary.Functions;
using Parsething.Resources;

namespace Parsething.Windows;

public partial class AutorizationWindow : Window
{
    public AutorizationWindow()
    {
        InitializeComponent();
    }

    private Window? MainWindow { get; set; }

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
        MainWindow = new();

        Managment.CurrentEmployee = GET.Employee(UserName.Text, Password.Password);

        if (Managment.CurrentEmployee != null)
        {
            MainWindow.Show();
        }
        else
        {
            MessageBox.Show("Вы ввели неверные данные!");
        }

    }

    private void Password_TextInput(object sender, TextCompositionEventArgs e)
    {
        VisiblePassword.Text = Password.Password;
    }

    private void VisiblePassword_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        Password.Password = VisiblePassword.Text;
    }
}