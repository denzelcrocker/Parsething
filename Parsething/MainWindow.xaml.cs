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

namespace Parsething
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AutorizationWindow autorization = new();
            if (autorization.ShowDialog() == true)
                DataContext = autorization.Employee;
            else Application.Current.Shutdown();
            InitializeComponent();
            Closing += MainWindow_Closing;
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
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            GlobalUsingValues.Instance.Procurements.Clear();
            PULL.ClosingActiveSessionsByEmployee(((Employee)Application.Current.MainWindow.DataContext).Id);
            SearchCriteria.Instance.ClearData();

            _ = MainFrame.Navigate(new Pages.SearchPage());
        }
    }
}