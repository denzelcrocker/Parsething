using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Parsething.Classes
{
    internal class MainViewModel
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public MainViewModel()
        {
            // Инициализируйте SeriesCollection и Labels данными
            SeriesCollection = new SeriesCollection();
            Labels = new List<string>();

            // Пример данных для графика
            var monthlyWins = GET.View.HistoryGroupByWins();

            // Заполняем данные для графика
            foreach (var win in monthlyWins)
            {
                SeriesCollection.Add(new ColumnSeries
                {
                    Title = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(win.Item2)} {win.Item1}",
                    Values = new ChartValues<int> { win.Item3 }
                });

                Labels.Add($"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(win.Item2)} {win.Item1}");
            }
        }
    }
}
