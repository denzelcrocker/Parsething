using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdministratorPage.xaml
    /// </summary>
    public partial class AdministratorPage : Page
    {
        private Frame MainFrame { get; set; } = null;
        public AdministratorPage(Employee? mainEmployee)
        {
            InitializeComponent();
            Parsed.Text = Convert.ToString(DatabaseLibrary.Functions.GET.CountOfParsed());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
        }
    }
}
