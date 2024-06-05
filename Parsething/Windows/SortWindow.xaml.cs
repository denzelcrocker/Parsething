using DatabaseLibrary.Entities.ProcurementProperties;
using DatabaseLibrary.Queries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для SortWindow.xaml
    /// </summary>
    public partial class SortWindow : Window
    {
        private Frame MainFrame { get; set; } = null!;
        private Procurement Procurement = new Procurement();
        private List<Employee>? Calculators { get; set; }
        private List<Procurement> Procurements { get; set; }
        private List<Region> Regions { get; set; }
        private List<Law> Laws { get; set; }
        private List<Organization> Organizations { get; set; }


        private bool IsSort;


        public SortWindow(List<Procurement> procurements, bool isSort)
        {
            InitializeComponent();

            Calculators = GET.View.EmployeesBy("Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов");
            AssignCombobox.ItemsSource = Calculators;
            Laws = GET.View.Laws();
            LawCombobox.ItemsSource = Laws;
            Organizations = GET.View.Organizations();
            OrganizationCombobox.ItemsSource = Organizations;
            Regions = GET.View.Regions();
            RegionCombobox.ItemsSource = Regions;

            Procurements = procurements;
            IsSort = isSort;
            SortInitialization();
        }
        private void SortInitialization()
        {
            if (IsSort)
            {
                if (Procurements.Count != 0)
                {
                    BalanceOfTendersTextBlock.Visibility = Visibility.Visible;
                    BalanceOfTendersTextBlock.Text = Procurements.Count.ToString();

                    Procurements = Procurements.OrderBy(p => p.Deadline).ToList();
                    Procurement = Procurements.First();

                    Id.Text = Procurement.Id.ToString();
                    Number.Text = Procurement.Number.ToString();
                    if (Procurement.Deadline != null && Procurement.TimeZone != null)
                        DeadLine.Text = $"{Procurement.Deadline} ({Procurement.TimeZone.Offset})";
                    if (Procurement.ResultDate != null)
                        ResultDate.Text = $"{Procurement.ResultDate}";
                    foreach (Law law in LawCombobox.ItemsSource)
                        if (law.Id == Procurement.LawId)
                        {
                            LawCombobox.SelectedItem = law;
                            break;
                        }
                    RequestUri.Text = Procurement.RequestUri.ToString();
                    InitialPrice.Text = Procurement.InitialPrice.ToString();
                    Object.Text = Procurement.Object.ToString();
                    foreach (Organization organization in OrganizationCombobox.ItemsSource)
                        if (organization.Id == Procurement.OrganizationId)
                        {
                            OrganizationCombobox.SelectedItem = organization;
                            break;
                        }
                    foreach (Region region in RegionCombobox.ItemsSource)
                        if (region.Id == Procurement.RegionId)
                        {
                            RegionCombobox.SelectedItem = region;
                            break;
                        }
                    Procurements.Remove(Procurement);
                }
                else
                { 
                    Close();
                }
            }
            else
            {
                BalanceOfTendersTextBlock.Visibility = Visibility.Hidden;
                SortBar.Visibility = Visibility.Hidden;
                AddButton.Visibility = Visibility.Visible;

                Number.IsReadOnly = false;

                DeadLine.IsReadOnly = false;
                ResultDate.IsReadOnly = false;
                LawCombobox.IsEnabled = true;
                RequestUri.IsReadOnly = false;
                InitialPrice.IsReadOnly = false;
                Object.IsReadOnly = false;
                OrganizationCombobox.IsEnabled = true;
                RegionCombobox.IsEnabled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame"); }
            catch { }
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void NavigateToProcurementEPlatform_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null && Procurement.Platform?.Address != null)
            {
                string url = Procurement.Platform.Address.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void NavigateToProcurementURL_Click(object sender, RoutedEventArgs e)
        {
            if (Procurement != null)
            {
                string url = Procurement.RequestUri.ToString();
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void AssignCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcurementsEmployee procurementsEmployee = new ProcurementsEmployee();
            procurementsEmployee.ProcurementId = Procurement.Id;
            procurementsEmployee.EmployeeId = ((Employee)AssignCombobox.SelectedItem).Id;
            PUT.ProcurementsEmployeesBy(procurementsEmployee, "Специалист отдела расчетов", "Заместитель руководителя отдела расчетов", "Руководитель отдела расчетов");

            Procurement.ProcurementStateId = 1;
            PULL.Procurement(Procurement);

            SortInitialization();
        }

        private void QueueButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement.ProcurementStateId = 1;
            PULL.Procurement(Procurement);

            SortInitialization();
        }

        private void RetreatButton_Click(object sender, RoutedEventArgs e)
        {
            Procurement.ProcurementStateId = 13;
            PULL.Procurement(Procurement);

            SortInitialization();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string warningMessage = "";

            if (!string.IsNullOrWhiteSpace(Number.Text))
            {
                Procurement.Number = Number.Text;
            }
            else
            {
                warningMessage += "Номер тендера не может быть пустым.\n";
            }

            if (DateTime.TryParse(DeadLine.Text, out DateTime deadline))
            {
                Procurement.Deadline = deadline;
            }
            else
            {
                warningMessage += "Некорректная дата окончания тендера.\n";
            }

            if (DateTime.TryParse(ResultDate.Text, out DateTime resultDate))
            {
                Procurement.ResultDate = resultDate;
            }
            else
            {
                warningMessage += "Некорректная дата результатов тендера.\n";
            }

            if (LawCombobox.SelectedItem != null)
            {
                Procurement.LawId = ((Law)LawCombobox.SelectedItem).Id;
            }
            else
            {
                warningMessage += "Выберите закон.\n";
            }

            if (!string.IsNullOrWhiteSpace(RequestUri.Text))
            {
                Procurement.RequestUri = RequestUri.Text;
            }
            else
            {
                warningMessage += "URI запроса не может быть пустым.\n";
            }

            if (!string.IsNullOrWhiteSpace(InitialPrice.Text))
            {
                if (decimal.TryParse(InitialPrice.Text, out decimal initialPrice))
                {
                    Procurement.InitialPrice = initialPrice;
                }
                else
                {
                    warningMessage += "Некорректная начальная цена.\n";
                }
            }
            else
            {
                warningMessage += "Начальная цена не может быть пустой.\n";
            }

            if (!string.IsNullOrWhiteSpace(Object.Text))
            {
                Procurement.Object = Object.Text;
            }
            else
            {
                warningMessage += "Объект тендера не может быть пустым.\n";
            }

            if (OrganizationCombobox.SelectedItem != null)
            {
                Procurement.OrganizationId = ((Organization)OrganizationCombobox.SelectedItem).Id;
            }
            else
            {
                warningMessage += "Выберите организацию.\n";
            }

            if (RegionCombobox.SelectedItem != null)
            {
                Procurement.RegionId = ((Region)RegionCombobox.SelectedItem).Id;
            }
            else
            {
                warningMessage += "Выберите регион.\n";
            }

            if (!string.IsNullOrEmpty(warningMessage))
            {
                MessageBox.Show(warningMessage, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                PUT.ProcurementSource(Procurement);
                MessageBox.Show("Тендер успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
