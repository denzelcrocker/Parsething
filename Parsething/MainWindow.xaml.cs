using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Parsething.Classes;
using Parsething.Windows;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Reflection;
using System.Net;
using Parsething.Pages;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Timers;
using System.Windows.Threading;

namespace Parsething
{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon; // Иконка в системном трее
        private DispatcherTimer _timer; // Таймер для проверки уведомлений
        List<EmployeeNotification> EmployeeNotifications;
        private bool _isWindowOpen; // Флаг для проверки, открыто ли окно

        public MainWindow()
        {
            AutorizationWindow autorization = new();
            if (autorization.ShowDialog() == true)
                DataContext = autorization.Employee;
            else Application.Current.Shutdown();

            InitializeComponent();

            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = SystemIcons.Information, // Иконка уведомлений
                Visible = true,
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
            };

            ReloadNotifications(); // Первая загрузка уведомлений

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15) // 30 секунд
            };
            _timer.Tick += TimerElapsed;
            _timer.Start();

            Closing += MainWindow_Closing;
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            ReloadNotifications();
        }

        private void ReloadNotifications()
        {
            int employeeId = ((Employee)Application.Current.Dispatcher.Invoke(() => DataContext)).Id;

            Task.Run(() =>
            {
                // Получаем уведомления в фоновом потоке
                var notifications = GET.View.UnreadEmployeeNotificationsBy(employeeId);
                // Возвращаемся в UI-поток, чтобы показать уведомления
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateNotificationIcon(employeeId);
                    foreach (EmployeeNotification employeeNotification in notifications)
                    {
                        ShowNotification(employeeNotification.Notification.Text);
                        PULL.EmployeeNotificationMarkAsRead(employeeNotification);
                    }
                });
            });
        }

        private void ShowNotification(string message)
        {
            _notifyIcon.BalloonTipTitle = "Новое уведомление";
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.ShowBalloonTip(1000); // Показываем уведомление в течение 3 секунд
        }
        private async Task UpdateNotificationIcon(int employeeId)
        {
            bool hasUnreadNotifications = await GET.View.HasUnDeletedNotifications(employeeId);

            string iconPath = hasUnreadNotifications
                ? "/Resources/Images/ActiveBell.png"  // Иконка с единицей
                : "/Resources/Images/InactiveBell.png";            // Обычная иконка

            NotificationImage.Source = new BitmapImage(new Uri(iconPath, UriKind.Relative));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string? s = ((Employee)DataContext).Photo;
            if (s != null)
            {
                byte[] bytes = Convert.FromBase64String(s);
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(bytes);
                bitmap.EndInit();
                EmployeePhoto.Fill = new ImageBrush { ImageSource = bitmap };
            }


            FullName.Content = ((Employee)DataContext).FullName;
            Position.Content = ((Employee)DataContext).Position?.Kind;

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
            else if (((Employee)DataContext).Position.Kind == "Руководитель отдела закупки" || ((Employee)DataContext).Position.Kind == "Заместитель руководителя отдела закупок" || ((Employee)DataContext).Position.Kind == "Специалист закупки")
            {
                _ = MainFrame.Navigate(new Pages.PurchaserPage());
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
            UpdateNotificationIcon(((Employee)DataContext).Id);
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    MaximizeAction.Content = ""; // Иконка для максимизации
                    break;
                case WindowState.Maximized:
                    MaximizeAction.Content = ""; // Иконка для восстановления
                    break;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeRestore();
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void MaximizeAction_Click(object sender, RoutedEventArgs e)
        {
            MaximizeRestore();
        }

        private void MaximizeRestore()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                MaximizeWithoutCoveringTaskbar();
            }
        }

        private void MaximizeWithoutCoveringTaskbar()
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            if (GetMonitorInfo(monitor, ref monitorInfo))
            {
                MaxWidth = monitorInfo.rcWork.right - monitorInfo.rcWork.left + 20;
                MaxHeight = monitorInfo.rcWork.bottom - monitorInfo.rcWork.top + 20;

                Left = monitorInfo.rcWork.left;
                Top = monitorInfo.rcWork.top;
            }

            WindowState = WindowState.Maximized;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
        private void CloseAction_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            PULL.ClosingActiveSessionsByEmployee(((Employee)Application.Current.MainWindow.DataContext).Id);
        }

        private void SwitchUser_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            MainWindow mainWindow = new();
            Close();
            mainWindow.Show();
            Application.Current.MainWindow = mainWindow;
            if ((Employee)Application.Current.MainWindow.DataContext != null)
                PULL.ClosingActiveSessionsByEmployee(((Employee)Application.Current.MainWindow.DataContext).Id);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((Employee)Application.Current.MainWindow.DataContext != null)
                PULL.ClosingActiveSessionsByEmployee(((Employee)Application.Current.MainWindow.DataContext).Id);
            _notifyIcon.Visible = false; // Скрываем иконку уведомлений из трея при закрытии приложения
            _timer.Stop(); // Останавливаем таймер
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            PULL.ClosingActiveSessionsByEmployee(((Employee)Application.Current.MainWindow.DataContext).Id);
            SearchCriteria.Instance.ClearData();

            if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Администратор")
            {
                _ = MainFrame.Navigate(new Pages.AdministratorPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела расчетов" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела расчетов")
            {
                _ = MainFrame.Navigate(new Pages.HeadsOfCalculatorsPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист отдела расчетов")
            {
                _ = MainFrame.Navigate(new Pages.CalculatorPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель тендерного отдела" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя тендреного отдела")
            {
                _ = MainFrame.Navigate(new Pages.HeadsOfManagersPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист по работе с электронными площадками")
            {
                _ = MainFrame.Navigate(new Pages.EPlatformSpecialistPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист тендерного отдела")
            {
                _ = MainFrame.Navigate(new Pages.ManagerPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела закупки" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела закупок" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист закупки")
            {
                _ = MainFrame.Navigate(new Pages.PurchaserPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Руководитель отдела производства" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Заместитель руководителя отдела производства" || ((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Специалист по производству")
            {
                _ = MainFrame.Navigate(new Pages.AssemblyPage());
            }
            else if (((Employee)Application.Current.MainWindow.DataContext).Position.Kind == "Юрист")
            {
                _ = MainFrame.Navigate(new Pages.LawyerPage());
            }
            else
            {
                // Add any additional roles here
            }
            UpdateNotificationIcon(((Employee)DataContext).Id);
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            PULL.ClosingActiveSessionsByEmployee(((Employee)Application.Current.MainWindow.DataContext).Id);
            SearchCriteria.Instance.ClearData();

            _ = MainFrame.Navigate(new Pages.SearchPage());
        }

        private void GoCharts_Click(object sender, RoutedEventArgs e)
        {
            _ = MainFrame.Navigate(new Pages.Charts());
        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            NotificationsPopUp.IsOpen = !NotificationsPopUp.IsOpen;
            if (NotificationsPopUp.IsOpen)
            {
                // Загрузить уведомления
                NotificationsListView.ItemsSource = GET.View.EmployeeNotificationsBy(((Employee)DataContext).Id);
            }
        }
        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ListView listView)
            {
                var scrollViewer = GetScrollViewer(listView);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true; // Предотвращаем дальнейшую обработку события
                }
            }
        }

        // Метод для получения ScrollViewer из ListView
        private ScrollViewer GetScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer)
                return obj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        private void DeleteNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeNotification employeeNotification = (sender as Button)?.DataContext as EmployeeNotification ?? new EmployeeNotification();
            if (employeeNotification != null)
            {
                PULL.EmployeeNotificationMarkAsDeleted(employeeNotification);
                NotificationsListView.ItemsSource = GET.View.EmployeeNotificationsBy(((Employee)DataContext).Id);
                UpdateNotificationIcon(((Employee)DataContext).Id);
            }

        }

        private void EditProcurement_Click(object sender, RoutedEventArgs e)
        {
            Procurement procurement = null;

            if (sender is MenuItem menuItem)
                procurement = menuItem.DataContext as Procurement;
            else if (sender is Button button)
                procurement = button.DataContext as Procurement;

            if (procurement != null)
            {
                NavigationState.LastSelectedProcurement = procurement;

                _ = MainFrame.Navigate(new CardOfProcurement(procurement));
            }
        }

        private void StarredProcurements_Click(object sender, RoutedEventArgs e)
        {
            StarredProcurementsPopUp.IsOpen = !StarredProcurementsPopUp.IsOpen;
            if (StarredProcurementsPopUp.IsOpen)
            {
                StarredProcurementsListView.ItemsSource = Functions.Conversion.ProcurementsEmployeesConversion(GET.View.ProcurementsEmployeesBy(((Employee)DataContext).Id, "Starred"));
            }
        }
        private void Calculating_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
            {
                NavigationState.LastSelectedProcurement = procurement;
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, true));
            }
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
            {
                NavigationState.LastSelectedProcurement = procurement;
                _ = MainFrame.Navigate(new ComponentCalculationsPage(procurement, false));
            }
        }
        private void NavigateToEPlatformURL_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem?.DataContext is Procurement procurement)
            {
                if (procurement.Platform?.Address != null)
                {
                    string url = procurement.Platform.Address.ToString();
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
        }
        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is Procurement procurement)
            {
                if (procurement.RequestUri != null)
                {
                    string url = procurement.RequestUri.ToString();
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
        }
        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Procurement procurement = menuItem.DataContext as Procurement;
                if (procurement != null)
                {
                    AssemblyMap assemblyMap = new AssemblyMap(procurement);
                    assemblyMap.Show();
                }
            }
        }
    }
}