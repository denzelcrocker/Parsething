using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using static Parsething.Functions.ListViewInitialization;

namespace Parsething.Windows
{
    /// <summary>
    /// Логика взаимодействия для AssemblyMap.xaml
    /// </summary>
    public partial class AssemblyMap : Window
    {
        private Procurement Procurement = new Procurement();
        private List<ComponentCalculation>? ComponentCalculations { get; set; }

        private List<Comment> Comments { get; set; }

        public AssemblyMap(Procurement procurement)
        {
            InitializeComponent();

            Procurement = procurement;
            ProcurementId.Text = procurement.DisplayId.ToString();
            OrganizationNameTextBlock.Text = "Организация: " + procurement.Organization.Name;
            PostalAddressTextBlock.Text = "Адрес доставки: " + procurement.Location;

            ComponentCalculations = GET.View.ComponentCalculationsBy(procurement.Id).Where(cc => cc.IsDeleted == false).ToList();

            Comments = GET.View.CommentsBy(procurement.Id).Where(c => c.IsTechnical == true).ToList();
            CommentsListView.ItemsSource = Comments;

            AssemblyMapListViewInitialization(procurement, ComponentCalculations, AssemblyMapListView);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            DragMove();

        private void MinimizeAction_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseAction_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void PrintAssemblyMap_Click(object sender, RoutedEventArgs e)
        {
            // Создаем PDF-документ
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Лист комплектаций";

            // Добавляем новую страницу с размерами страницы A4
            PdfPage pdfPage = pdf.AddPage();
            pdfPage.Width = XUnit.FromMillimeter(210);
            pdfPage.Height = XUnit.FromMillimeter(297);

            // Создаем объект XGraphics для рисования на странице
            XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

            // Создаем изображение из PrintGrid
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)PrintGrid.ActualWidth, (int)PrintGrid.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(PrintGrid);

            // Конвертируем изображение в формат XImage
            MemoryStream memoryStream = new MemoryStream();
            BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            bitmapEncoder.Save(memoryStream);
            memoryStream.Position = 0;
            XImage xImage = XImage.FromStream(memoryStream);

            // Рисуем изображение на PDF-странице
            gfx.DrawImage(xImage, 0, 0, pdfPage.Width, pdfPage.Height);

            // Открываем диалоговое окно сохранения файла
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.Title = "Save PDF File";
            saveFileDialog.FileName = $"Лист_комплектаций_{Procurement.DisplayId}.pdf"; // Имя файла по умолчанию

            // Если пользователь выбрал местоположение и нажал "Сохранить"
            if (saveFileDialog.ShowDialog() == true)
            {
                // Сохраняем PDF-файл по указанному пути
                pdf.Save(saveFileDialog.FileName);
            }
        }
    }
}
