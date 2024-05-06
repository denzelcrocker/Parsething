using System;
using System.Collections.Generic;
using System.Globalization;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace Parsething.Pages
{
    /// <summary>
    /// Логика взаимодействия для Charts.xaml
    /// </summary>
    public partial class Charts : Page
    {
        private CartesianChart chart;
        private CartesianChart barChart;
        private double maxValue;
        public Charts()
        {
            InitializeComponent();

            
            var monthWins = GET.View.HistoryGroupByWins();
            var monthSends = GET.View.HistoryGroupBySended();
            var monthCalculated = GET.View.HistoryGroupByCalculations();
            
            if(monthCalculated.Count != 0 && monthSends.Count != 0 && monthWins.Count != 0)
            {
                maxValue = Math.Max((double)monthCalculated.Max(entry => entry.Item3), Math.Max((double)monthCalculated.Max(entry => entry.Item3), (double)monthCalculated.Max(entry => entry.Item3)));

                barChart = new CartesianChart
                {
                    Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Количество выигрышей",
                        Values = new ChartValues<int>(monthWins.Select(entry => entry.Item3)),
                        Fill = Brushes.Red,
                    },
                    new ColumnSeries
                    {
                        Title = "Количество отправленных тендеров",
                        Values = new ChartValues<int>(monthSends.Select(entry => entry.Item3)),
                        Fill = Brushes.Blue,
                    },
                    new ColumnSeries
                    {
                        Title = "Количество посчитанных тендеров",
                        Values = new ChartValues<int>(monthCalculated.Select(entry => entry.Item3)),
                        Fill = Brushes.Purple,
                    }
                },
                    AxisX = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Месяцы",
                        Labels = monthWins.Select(entry => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(entry.Item2)} {entry.Item1}").ToArray(),
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
                        LabelFormatter = value => value.ToString(),
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            Step = 10,
                            IsEnabled = true
                        },
                        Foreground = Brushes.Black // Черный цвет текста для оси X
                    }
                }
                };

                Grid.SetRow(barChart, 0);
                Grid.SetColumn(barChart, 0);

                MainGrid.Children.Add(barChart);

                chart = new CartesianChart
                {
                    Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Количество выигрышей",
                        Values = new ChartValues<int>(monthWins.Select(entry => entry.Item3)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Red, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество отправленных тендеров",
                        Values = new ChartValues<int>(monthSends.Select(entry => entry.Item3)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Blue, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    },
                    new LineSeries
                    {
                        Title = "Количество посчитанных тендеров",
                        Values = new ChartValues<int>(monthCalculated.Select(entry => entry.Item3)),
                        Fill = Brushes.Transparent, // Прозрачный фон
                        Stroke = Brushes.Purple, // Цвет линии для выигранных тендеров
                        PointGeometry = DefaultGeometries.Circle, // Форма точек на кривой
                        PointGeometrySize = 10 // Размер точек на кривой
                    }
                },
                    AxisX = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Месяцы",
                        Labels = monthWins.Select(entry => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(entry.Item2)} {entry.Item1}").ToArray(),
                        LabelsRotation = 45,
                        Foreground = System.Windows.Media.Brushes.Black
                    }
                },
                    AxisY = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Количество",
                        MinValue = 0,
                        MaxValue = Math.Ceiling(maxValue / 10) * 10,
                        LabelFormatter = value => value.ToString(),
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            Step = 10,
                            IsEnabled = true
                        },
                        Foreground = System.Windows.Media.Brushes.Black // Черный цвет текста для оси X
                    }
                }
                };
                Grid.SetRow(chart, 0);
                Grid.SetColumn(chart, 0);

                MainGrid.Children.Add(chart);
                barChart.Visibility = Visibility.Visible;
                chart.Visibility = Visibility.Hidden;
            }
            
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
