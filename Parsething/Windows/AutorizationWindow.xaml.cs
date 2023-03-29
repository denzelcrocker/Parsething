using Microsoft.EntityFrameworkCore.Diagnostics;

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

        string userName = UserName.Text;
        string password = Password.Password;
        string visiblePassword = VisiblePassword.Text;
        int countOfEmployees = db.Employees.Count();
        bool checkAuthorization = false;
        employees = db.Employees.ToList();
        for(int i = 0; i < countOfEmployees; i++)
        {

            if (employees[i].UserName == userName && employees[i].Password==password)
            {
                employee = employees[i];
                checkAuthorization= true;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }

        }
        if(checkAuthorization==false)
        {
            MessageBox.Show("Вы ввели неверный пароль!");
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