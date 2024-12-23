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
        private CartesianChart chart;
        private CartesianChart barChart;
        private double maxValue;
        private List<History>? Histories;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthWins;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthSends;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthIssued;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthCalculated;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthNews;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthRetreat;
        private (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthRejected;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
            }
            catch { }
        }
        private async void LoadDataAsync()
        {
            try
            {
                MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
                Histories = await Task.Run(() => GET.View.Histories());

                if (Histories != null)
                {
                    // Получаем данные для каждого типа тендеров
                    await Task.Run(() =>
                    {
                        monthWins = GET.View.HistoryGroupBy("Выигран 2ч", Histories);
                        monthSends = GET.View.HistoryGroupBy("Отправлен", Histories);
                        monthIssued = GET.View.HistoryGroupBy("Оформлен", Histories);
                        monthCalculated = GET.View.HistoryGroupBy("Посчитан", Histories);
                        monthNews = GET.View.HistoryGroupBy("Новый", Histories);
                        monthRetreat = GET.View.HistoryGroupBy("Отбой", Histories);
                        monthRejected = GET.View.HistoryGroupBy("Отклонен", Histories);
                    });

                    // Формируем полный список месяцев за текущий год для выравнивания столбцов
                    var months = Enumerable.Range(1, 12).Select(m => new DateTime(DateTime.Now.Year, m, 1)).ToList();

                    // Обработка пустых месяцев
                    monthWins.Item1 = EnsureAllMonths(monthWins.Item1, months);
                    monthSends.Item1 = EnsureAllMonths(monthSends.Item1, months);
                    monthIssued.Item1 = EnsureAllMonths(monthIssued.Item1, months);
                    monthCalculated.Item1 = EnsureAllMonths(monthCalculated.Item1, months);
                    monthNews.Item1 = EnsureAllMonths(monthNews.Item1, months);
                    monthRetreat.Item1 = EnsureAllMonths(monthRetreat.Item1, months);
                    monthRejected.Item1 = EnsureAllMonths(monthRejected.Item1, months);

                    // Расчет максимального значения для оси Y
                    if (monthCalculated.Item1.Count != 0 && monthSends.Item1.Count != 0 && monthWins.Item1.Count != 0 && monthNews.Item1.Count != 0)
                    {
                        maxValue = Math.Max((double)monthNews.Item1.Max(entry => entry.Item4), Math.Max((double)monthSends.Item1.Max(entry => entry.Item4), (double)monthWins.Item1.Max(entry => entry.Item4)));

                        // Создаем диаграмму столбцов
                        barChart = CreateBarChart(monthWins, monthSends, monthIssued, monthCalculated, monthNews, monthRetreat, monthRejected, months);

                        // Создаем линейную диаграмму
                        chart = CreateLineChart(monthWins, monthSends, monthIssued, monthCalculated, monthNews, monthRetreat, monthRejected, months);

                        // Добавляем диаграммы на страницу
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Grid.SetRow(barChart, 0);
                            Grid.SetColumn(barChart, 0);
                            MainGrid.Children.Add(barChart);

                            Grid.SetRow(chart, 0);
                            Grid.SetColumn(chart, 0);
                            MainGrid.Children.Add(chart);

                            barChart.Visibility = Visibility.Visible;
                            chart.Visibility = Visibility.Hidden;
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
            LoadDataAsync();
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
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthWins,
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthSends,
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthIssued,
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthCalculated,
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthNews,
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthRetreat,
        (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthRejected,
        List<DateTime> months)
        {
            var barChart = new CartesianChart
            {
                Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Отклоненные тендеры",
                    Values = new ChartValues<int>(monthRejected.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c0392b")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumRejected = monthRejected.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthRejected.Item1[(int)point.X].Item4} ( НМЦК ~{sumRejected.ToString("N2")} р. )";
                    },
                    
                    
                },
                new ColumnSeries
                {
                    Title = "Выигранные тендеры",
                    Values = new ChartValues<int>(monthWins.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8e44ad")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumWins = monthWins.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthWins.Item1[(int)point.X].Item4} ( Контракты ~{sumWins.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Отправленные тендеры",
                    Values = new ChartValues<int>(monthSends.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9b59b6")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumSends = monthSends.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthSends.Item1[(int)point.X].Item4} ( НМЦК ~{sumSends.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Оформленные тендеры",
                    Values = new ChartValues<int>(monthIssued.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumIssued = monthIssued.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthIssued.Item1[(int)point.X].Item4} ( НМЦК ~{sumIssued.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Посчитанные тендеры",
                    Values = new ChartValues<int>(monthCalculated.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ecc71")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumCalculated = monthCalculated.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthCalculated.Item1[(int)point.X].Item4} ( НМЦК ~ {sumCalculated.ToString("N2")}  р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Тендеры в отбое",
                    Values = new ChartValues<int>(monthRetreat.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumRetreat = monthRetreat.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthRetreat.Item1[(int)point.X].Item4} ( НМЦК ~{sumRetreat.ToString("N2")} р. )";
                    }
                },
                new ColumnSeries
                {
                    Title = "Новые тендеры",
                    Values = new ChartValues<int>(monthNews.Item1.Select(entry => entry.Item4)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498db")),
                    LabelPoint = point =>
                    {
                        var month = months[(int)point.X];
                        decimal sumNews = monthNews.Item1?.Where(entry => entry.Item2 == month.Month).Sum(entry => entry.Item3) ?? 0;
                        return $"{monthNews.Item1[(int)point.X].Item4} ( НМЦК ~{sumNews.ToString("N2")} р. )";
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
                    Foreground = Brushes.Black
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
                        Step = 100
                    },
                    Foreground = Brushes.Black
                }
            }
                };

            return barChart;
        }

        // Метод для создания линейной диаграммы
        private CartesianChart CreateLineChart(
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthWins,
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthSends,
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthIssued,
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthCalculated,
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthNews,
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthRetreat,
            (List<Tuple<int, int, decimal, int, List<Procurement>>>?, List<Procurement>?) monthRejected,
            List<DateTime> months)
        {
            var lineChart = new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Количество отклоненных тендеров",
                        Values = new ChartValues<int>(monthRejected.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Black, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество выигрышей",
                        Values = new ChartValues<int>(monthWins.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Green, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество отправленных тендеров",
                        Values = new ChartValues<int>(monthSends.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Blue, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество оформленных тендеров",
                        Values = new ChartValues<int>(monthIssued.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Blue, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество посчитанных тендеров",
                        Values = new ChartValues<int>(monthCalculated.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Purple, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество отбоев",
                        Values = new ChartValues<int>(monthRetreat.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Red, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество новых тендеров",
                        Values = new ChartValues<int>(monthNews.Item1.Select(entry => entry.Item4)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.BlueViolet, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                },
                AxisX = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Месяцы",
                        Labels = months.Select(m => $"{m.ToString("MMM", CultureInfo.CurrentCulture)} {m.Year}").ToArray(),
                        LabelsRotation = 45,
                        Foreground = Brushes.Black
                    }
                },
                AxisY = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Количество",
                        MinValue = 0,
                        MaxValue = Math.Ceiling(maxValue / 10) * 10,
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            Step = 100
                        },
                        Foreground = Brushes.Black
                    }
                }
            };
            barChart.MouseDown += BarChart_MouseDown;

            return lineChart;
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
                    if (monthRejected.Item1 != null)
                    {
                        procurements.AddRange(monthRejected.Item1
                            .Where(p => p.Item2 == month) // Фильтрация по Item2
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Выигранные тендеры":
                    if (monthWins.Item1 != null)
                    {
                        procurements.AddRange(monthWins.Item1
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Отправленные тендеры":
                    if (monthSends.Item1 != null)
                    {
                        procurements.AddRange(monthSends.Item1
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Оформленные тендеры":
                    if (monthIssued.Item1 != null)
                    {
                        procurements.AddRange(monthIssued.Item1
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Посчитанные тендеры":
                    if (monthCalculated.Item1 != null)
                    {
                        procurements.AddRange(monthCalculated.Item1
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Тендеры в отбое":
                    if (monthRetreat.Item1 != null)
                    {
                        procurements.AddRange(monthRetreat.Item1
                            .Where(p => p.Item2 == month)
                            .SelectMany(p => p.Item5));
                    }
                    break;

                case "Новые тендеры":
                    if (monthNews.Item1 != null)
                    {
                        procurements.AddRange(monthNews.Item1
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
        private void BarChartButton_Click(object sender, RoutedEventArgs e)
        {
            if (chart != null && barChart != null)
            {
                barChart.Visibility = Visibility.Visible;
                chart.Visibility = Visibility.Hidden;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (chart != null && barChart != null)
            {
                barChart.Visibility = Visibility.Hidden;
                chart.Visibility = Visibility.Visible;
            }
        }

        
    }
}
