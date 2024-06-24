using Parsething.Classes;
using System.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;

namespace Parsething.Windows
{
    public partial class AutorizationWindow : Window
    {
        public AutorizationWindow()
        {
            InitializeComponent();
            var (username, password) = UserConfig.LoadCredentials();
            UserName.Text = username;
            Password.Password = password;
        }

        public Employee? Employee { get; private set; }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;

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
            Employee = GET.Entry.Employee(UserName.Text, Password.Password);
            if (Employee != null)
            {
                DialogResult = true;

                string computerName = Dns.GetHostName();

                string ipAddress = string.Empty;
                foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var addressInfo in networkInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipAddress = addressInfo.Address.ToString();
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        break;
                    }
                }
                string loginInfo = $"{Employee.FullName}, IP: {ipAddress}, Компьютер: {computerName}";
                History? history = new History { EmployeeId = Employee.Id, Date = DateTime.Now, EntityType = "Login", EntryId = 0, Text = $"{loginInfo}" };
                PUT.History(history);

                string username = UserName.Text;
                string password = Password.Password;
                UserConfig.SaveCredentials(username, password);
            }
            else
            {
                MessageBox.Show("Вы ввели неверные данные!");
            }
        }

        private void Password_TextInput(object sender, TextCompositionEventArgs e) =>
            VisiblePassword.Text = Password.Password;

        private void VisiblePassword_TextChanged(object sender, TextChangedEventArgs e) =>
            Password.Password = VisiblePassword.Text;
    }
}