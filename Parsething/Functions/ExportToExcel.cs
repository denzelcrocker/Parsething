using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using static DatabaseLibrary.Queries.GET;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Drawing;
using Microsoft.Win32;
using System.Diagnostics;

namespace Parsething.Functions
{
    internal class ExportToExcel
    {
        public static void ExportSupplyMonitoringListToExcel(List<SupplyMonitoringList> supplyMonitoringList)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Установка контекста лицензирования
            if(supplyMonitoringList != null)
            {
                supplyMonitoringList = supplyMonitoringList.OrderBy(x => x.SupplierName).ToList();

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SupplyMonitoring");

                    string[] headers = { "Поставщик", "Производитель", "Наименование", "Статус товара", "Цена/Шт", "Количество", "Поставщик", "Номер тендера", "Общая сумма" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        ExcelRange headerCell = worksheet.Cells[1, i + 1];
                        headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#535353"));
                        headerCell.Style.Font.Color.SetColor(Color.White);
                    }

                    for (int i = 0; i < supplyMonitoringList.Count; i++)
                    {
                        SupplyMonitoringList item = supplyMonitoringList[i];
                        worksheet.Cells[i + 2, 1].Value = item.SupplierName;
                        worksheet.Cells[i + 2, 2].Value = item.ManufacturerName;
                        worksheet.Cells[i + 2, 3].Value = item.ComponentName;
                        worksheet.Cells[i + 2, 4].Value = item.ComponentStatus;
                        worksheet.Cells[i + 2, 5].Value = $"{item.AveragePrice:N2} р.";
                        worksheet.Cells[i + 2, 6].Value = item.TotalCount;
                        worksheet.Cells[i + 2, 7].Value = item.SellerName;
                        worksheet.Cells[i + 2, 8].Value = item.TenderNumber;
                        worksheet.Cells[i + 2, 9].Value = $"{item.TotalAmount:N2} р.";
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                        package.SaveAs(excelFile);
                    }
                }
            }
        }
    }
}