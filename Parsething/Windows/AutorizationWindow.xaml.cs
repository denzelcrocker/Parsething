using Parsething.Classes;
using System.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;

namespace Parsething.Windows
{
    public partial class AutorizationWindow : Window
    {
        public AutorizationWindow()
        {
            InitializeComponent();

            // Загружаем учётные данные и тему
            var (username, password, theme) = UserConfig.LoadCredentials();

            UserName.Text = username;
            Password.Password = password;

            // Применяем тему
            ChangeTheme(theme);
        }

        private void ChangeTheme(string theme)
        {
            // Получаем текущую тему (это не должно менять учетные данные)
            string currentTheme = UserConfig.LoadCredentials().Theme;

            // Здесь мы не сохраняем пустые значения для учетных данных, только изменяем тему
            UserConfig.SaveCredentials(UserConfig.LoadCredentials().Username, UserConfig.LoadCredentials().Password, theme);

            // Очищаем текущие темы
            var dictionariesToRemove = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source.ToString().Contains("Dark") || d.Source.ToString().Contains("Light"))
                .ToList();

            // Удаляем старые ресурсы
            foreach (var dict in dictionariesToRemove)
            {
                Application.Current.Resources.MergedDictionaries.Remove(dict);
            }

            // Подключаем нужную тему
            if (theme == "Dark")
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/DarkBrushes.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/Windows.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/Labels.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/Buttons.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/CheckBoxes.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/Charts.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Dark/ComboBoxes.xaml", UriKind.Relative) });
            }
            else if (theme == "Light")
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/LightBrushes.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/Windows.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/Labels.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/Buttons.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/CheckBoxes.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/Charts.xaml", UriKind.Relative) });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Resources/Themes/Light/ComboBoxes.xaml", UriKind.Relative) });
            }
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
                if (!IsUpdateRequired())
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

                    var currentTheme = UserConfig.LoadCredentials().Theme;  // Загружаем текущую тему
                    UserConfig.SaveCredentials(UserName.Text, Password.Password, currentTheme);  // Сохраняем учётные данные с текущей темой
                }
            }
            else
            {
                MessageBox.Show("Вы ввели неверные данные!");
            }
        }
        public bool IsUpdateRequired()
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";

            var latestVersion = GET.View.GetLatestVersion(); // Получаем последнюю версию из БД

            if (latestVersion.VersionNumber != currentVersion)
            {
                if (latestVersion.IsMandatory)
                {
                    MessageBox.Show("Доступна новая версия! Обновите приложение", "Обязательное обновление!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DialogResult = false;
                    return true; // Блокируем работу
                }
                else
                {
                    MessageBox.Show("Доступна новая версия! Обновите приложение", "Обновление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return false;
        }
        private void Password_TextInput(object sender, TextCompositionEventArgs e) =>
            VisiblePassword.Text = Password.Password;

        private void VisiblePassword_TextChanged(object sender, TextChangedEventArgs e) =>
            Password.Password = VisiblePassword.Text;
    }
}