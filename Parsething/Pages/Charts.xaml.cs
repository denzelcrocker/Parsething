using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DatabaseLibrary.Entities.ProcurementProperties;
using LiveCharts;
using LiveCharts.Wpf;
using Parsething.Classes;

namespace Parsething.Pages
{
    public partial class Charts : Page
    {
        private Frame MainFrame { get; set; } = null!;
        private CartesianChart barChart;
        private double maxValue;
        private List<History>? Histories;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthWins;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthSends;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthIssued;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthCalculated;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthNews;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthRetreat;
        private List<Tuple<int, int, decimal, int, List<Procurement>>>? monthRejected;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
            }
            catch { }
        }
        private async void LoadDataAsync(int year)
        {
            try
            {
                LoadingIndicator.Visibility = Visibility.Visible;
                Histories = await Task.Run(() => GET.View.Histories().Where(h => h.Date.Year == year).ToList());

                if (Histories != null)
                {
                    // Получаем данные для каждого типа тендеров
                    var tasks = new List<Task>
                    {
                        Task.Run(() => monthWins = GET.View.HistoryGroupBy("Выигран 2ч", Histories)),
                        Task.Run(() => monthSends = GET.View.HistoryGroupBy("Отправлен", Histories)),
                        Task.Run(() => monthIssued = GET.View.HistoryGroupBy("Оформлен", Histories)),
                        Task.Run(() => monthCalculated = GET.View.HistoryGroupBy("Посчитан", Histories)),
                        Task.Run(() => monthNews = GET.View.HistoryGroupBy("Новый", Histories)),
                        Task.Run(() => monthRetreat = GET.View.HistoryGroupBy("Отбой", Histories)),
                        Task.Run(() => monthRejected = GET.View.HistoryGroupBy("Отклонен", Histories))
                    };

                    // Ожидание завершения всех задач
                    await Task.WhenAll(tasks);

                    // Формируем полный список месяцев за текущий год для выравнивания столбцов
                    var months = Enumerable.Range(1, 12).Select(m => new DateTime(year, m, 1)).ToList();

                    // Обработка пустых месяцев
                    monthWins = EnsureAllMonths(monthWins, months);
                    monthSends = EnsureAllMonths(monthSends, months);
                    monthIssued = EnsureAllMonths(monthIssued, months);
                    monthCalculated = EnsureAllMonths(monthCalculated, months);
                    monthNews = EnsureAllMonths(monthNews, months);
                    monthRetreat = EnsureAllMonths(monthRetreat, months);
                    monthRejected = EnsureAllMonths(monthRejected, months);

                    // Расчет максимального значения для оси Y
                    if (monthCalculated.Count != 0 && monthSends.Count != 0 && monthWins.Count != 0 && monthNews.Count != 0)
                    {
                        maxValue = Math.Max((double)monthNews.Max(entry => entry.Item4), Math.Max((double)monthSends.Max(entry => entry.Item4), (double)monthWins.Max(entry => entry.Item4)));

                        // Создаем диаграмму столбцов
                        barChart = CreateBarChart(monthWins, monthSends, monthIssued, monthCalculated, monthNews, monthRetreat, monthRejected, months);

                        // Создаем линейную диаграмму

                        // Добавляем диаграммы на страницу
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            ChartContent.Content = barChart;
                            LoadingIndicator.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        public Charts()
        {
            InitializeComponent();
            InitializeYearComboBox();
        }
        private void InitializeYearComboBox()
        {
            // Добавляем годы в ComboBox
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 5; year <= currentYear + 5; year++) // Пример: последние 5 лет и следующие 5
            {
                YearComboBox.Items.Add(year);
            }
            YearComboBox.SelectedItem = currentYear; // Устанавливаем текущий год по умолчанию
        }
        // Метод для заполнения пустых месяцев нулевыми значениями
        private List<Tuple<int, int, decimal, int, List<Procurement>>> EnsureAllMonths(List<Tuple<int, int, decimal, int, List<Procurement>>> data, List<DateTime> months)
        {
            var completeData = months.Select(m =>
            {
                var existing = data.FirstOrDefault(d => d.Item1 == m.Year && d.Item2 == m.Month);
                return existing ?? Tuple.Create(m.Year, m.Month, 0m, 0, new List<Procurement>());
            }).ToList();

            return completeData;
        }

        // Метод для создания диаграммы столбцов
        private CartesianChart CreateBarChart(
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthWins,
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthSends,
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthIssued,
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthCalculated,
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthNews,
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthRetreat,
        List<Tuple<int, int, decimal, int, List<Procurement>>>? monthRejected,
        List<DateTime> months)
        {
            var barChart = new CartesianChart
            {
                Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Отклоненные тендеры",
                    Values = new ChartValues<int>(monthRejected.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c0392b")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumRejected = monthRejected?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthRejected[(int)point.X].Item4} ( НМЦК ~{sumRejected.ToString("N2")} р. )";
                    },
                    
                    
                },
                new ColumnSeries
                {
                    Title = "Выигранные тендеры",
                    Values = new ChartValues<int>(monthWins.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8e44ad")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumWins = monthWins?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthWins[(int)point.X].Item4} ( Контракты ~{sumWins.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Отправленные тендеры",
                    Values = new ChartValues<int>(monthSends.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9b59b6")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumSends = monthSends?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthSends[(int)point.X].Item4} ( НМЦК ~{sumSends.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Оформленные тендеры",
                    Values = new ChartValues<int>(monthIssued.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumIssued = monthIssued?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthIssued[(int)point.X].Item4} ( НМЦК ~{sumIssued.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Посчитанные тендеры",
                    Values = new ChartValues<int>(monthCalculated.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ecc71")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumCalculated = monthCalculated?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthCalculated[(int)point.X].Item4} ( НМЦК ~ {sumCalculated.ToString("N2")}  р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Тендеры в отбое",
                    Values = new ChartValues<int>(monthRetreat.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumRetreat = monthRetreat?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthRetreat[(int)point.X].Item4} ( НМЦК ~{sumRetreat.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Новые тендеры",
                    Values = new ChartValues<int>(monthNews.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498db")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumNews = monthNews?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthNews[(int)point.X].Item4} ( НМЦК ~{sumNews.ToString("N2")} р. )";
                    }
                }
            },
                AxisX = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Месяцы",
                        Labels = months.Select(m => $"{m.ToString("MMM", CultureInfo.CurrentCulture)} {m.Year}").ToArray(),
                        LabelsRotation = 45,
                        Foreground = (Brush)FindResource("TextBrush"),
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            Stroke = (Brush)FindResource("GridLineChartBrush") // Применение цвета сетки для оси X
                        }
                    }
                },
                AxisY = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Количество",
                        MinValue = 0,
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            Step = 100,
                            Stroke = (Brush)FindResource("GridLineChartBrush"), // Применение цвета сетки для оси Y
                        },
                        Foreground = (Brush)FindResource("TextBrush")
                    }
                }
            };
            barChart.DataTooltip = new DefaultTooltip
            {
                Style = (Style)FindResource("BarChartTooltipStyle") // Применение стиля для Tooltip
            };
            barChart.MouseDown += BarChart_MouseDown;

            return barChart;
        }

        private void BarChart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var months = Enumerable.Range(1, 12).Select(m => new DateTime(DateTime.Now.Year, m, 1)).ToList();
            var chart = sender as CartesianChart;
            var point = e.GetPosition(chart);

            // Получаем ширину каждого столбца
            double columnWidth = chart.ActualWidth / (chart.Series.Count * 12); // 12 - количество месяцев

            // Вычисляем индекс столбца по X позиции
            int columnIndex = (int)(point.X / columnWidth);

            // Получаем месяц и статус на основе индекса
            int monthIndex = (columnIndex / 7) % 12; // Получаем индекс месяца (0-11)
            int statusIndex = columnIndex % 7; // Индекс статуса (0-6)

            string monthName = new DateTime(DateTime.Now.Year, monthIndex + 1, 1).ToString("MMMM", CultureInfo.CurrentCulture);
            string status = GetStatusFromColumnIndex(statusIndex);

            // Выводим информацию о месяце и статусе

            // Получение списка тендеров на основе статуса и месяца
            var procurements = GetTendersForStatusAndMonth(status, monthIndex + 1);

            GlobalUsingValues.Instance.AddProcurements(procurements);
            if (GlobalUsingValues.Instance.Procurements.Count > 0)
                MainFrame.Navigate(new SearchPage());
        }

        // Получаем тендеры на основе статуса и месяца
        private List<Procurement> GetTendersForStatusAndMonth(string status, int month)
        {
            List<Procurement> procurements = new List<Procurement>();

            switch (status)
            {
                case "Отклоненные тендеры":
                    if (monthRejected != null)
                    {
                        procurements.AddRange(monthRejected
                            .Where(p => p.Item2 == month) // Фильтрация по Item2
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Выигранные тендеры":
                    if (monthWins != null)
                    {
                        procurements.AddRange(monthWins
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Отправленные тендеры":
                    if (monthSends != null)
                    {
                        procurements.AddRange(monthSends
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Оформленные тендеры":
                    if (monthIssued != null)
                    {
                        procurements.AddRange(monthIssued
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Посчитанные тендеры":
                    if (monthCalculated != null)
                    {
                        procurements.AddRange(monthCalculated
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Тендеры в отбое":
                    if (monthRetreat != null)
                    {
                        procurements.AddRange(monthRetreat
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Новые тендеры":
                    if (monthNews != null)
                    {
                        procurements.AddRange(monthNews
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;
            }

            return procurements;
        }

        private string GetStatusFromColumnIndex(int statusIndex)
        {
            return statusIndex switch
            {
                0 => "Отклоненные тендеры",
                1 => "Выигранные тендеры",
                2 => "Отправленные тендеры",
                3 => "Оформленные тендеры",
                4 => "Посчитанные тендеры",
                5 => "Тендеры в отбое",
                6 => "Новые тендеры",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearComboBox.SelectedItem is int selectedYear)
            {
                LoadDataAsync(selectedYear); // Загружаем данные для выбранного года
            }
        }
    }
}
