using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using Parsething.Classes;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Parsething.Functions
{
    public class ProfitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return DependencyProperty.UnsetValue;

            decimal? contractAmount = values[0] as decimal?;
            decimal? reserveContractAmount = values[1] as decimal?;
            decimal? amount = values[2] as decimal?;
            string? type = values[3].ToString();

            if (reserveContractAmount == null && contractAmount != null)
            {
                if (amount != null)
                    return ((decimal)contractAmount - (decimal)amount).ToString("N2") + " р.";
                else
                    return DependencyProperty.UnsetValue; // Обработка случая, если amount == null
            }
            else if (reserveContractAmount != null)
            {
                if (amount != null && type == "Purchase")
                    return ((decimal)reserveContractAmount - (decimal)amount).ToString("N2") + " р.";
                else if (amount != null && type == "Calculating" && contractAmount != null)
                    return ((decimal)contractAmount - (decimal)amount).ToString("N2") + " р.";
                else
                    return DependencyProperty.UnsetValue; // Обработка случая, если amount == null
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class PercentageProfitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return DependencyProperty.UnsetValue;

            decimal? contractAmount = values[0] as decimal?;
            decimal? reserveContractAmount = values[1] as decimal?;
            decimal? amount = values[2] as decimal?;
            string? type = values[3].ToString();


            if (reserveContractAmount == null && contractAmount != null)
            {
                if (amount != null && amount != 0)
                    return (((decimal)contractAmount - (decimal)amount) / (decimal)amount * 100).ToString("N0") + "%";
                else
                    return DependencyProperty.UnsetValue; // Обработка случая, если amount == null
            }
            else if (reserveContractAmount != null)
            {
                if (amount != null && type == "Purchase" && amount != 0)
                    return (((decimal)reserveContractAmount - (decimal)amount) / (decimal)amount * 100).ToString("N0") + "%";
                else if (amount != null && type == "Calculating" && contractAmount != null && amount != 0)
                    return (((decimal)contractAmount - (decimal)amount) / (decimal)amount * 100).ToString("N0") + "%";
                return DependencyProperty.UnsetValue; // Обработка случая, если amount == null
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class PercentageBetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return DependencyProperty.UnsetValue;

            decimal? calculateAmount = values[0] as decimal?;
            decimal? bet = values[1] as decimal?;

            if (calculateAmount != null && bet != null && bet != 0)
            {
                decimal percentage = ((decimal)bet - (decimal)calculateAmount) / (decimal)calculateAmount * 100;
                return percentage.ToString("N1") + "%";
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class AmountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return DependencyProperty.UnsetValue;

            decimal? contractAmount = values[0] as decimal?;
            decimal? reserveContractAmount = values[1] as decimal?;
            if (reserveContractAmount != null && contractAmount != null)
                return ((decimal)reserveContractAmount).ToString("N2") + " р.";
            else if (reserveContractAmount == null && contractAmount != null)
                return ((decimal)contractAmount).ToString("N2") + " р.";
            else return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string input = value.ToString();
            int index = input.LastIndexOf(" ");
            if (index != -1)
            {
                int secondIndex = input.LastIndexOf(" ", index - 1);
                if (secondIndex != -1)
                {
                    string numberPart = input.Substring(0, secondIndex);
                    return $"{numberPart} ";
                }
            }
            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                TimeSpan timeRemaining = date - DateTime.Now;

                if (timeRemaining.TotalDays > 3)
                {
                    return "Green";
                }
                else if (timeRemaining.TotalDays <= 3 && timeRemaining.TotalDays >= 1)
                {
                    return "Yellow";
                }
                else
                {
                    return "Red";
                }
            }
            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ShipmentPlanColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string kind)
            {

                if (kind == "Пред-предыдущая" || kind == "Предыдущая")
                {
                    return "Black";
                }
                else if (kind == "Текущая")
                {
                    return "Red";
                }
                else if (kind == "Следующая")
                {
                    return "Orange";
                }
                else
                {
                    return "Green";
                }
            }
            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateTimeWithTimeZoneConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return DependencyProperty.UnsetValue;

            if (values[0] is DateTime deadline && values[1] is string timeZoneOffset)
            {
                if (timeZoneOffset == "МСК" && !timeZoneOffset.Contains("МСК+") && !timeZoneOffset.Contains("МСК-") || timeZoneOffset == "стандартное")
                {
                    return deadline.ToString("dd.MM.yyyy HH:mm:ss");
                }

                string[] parts = timeZoneOffset.Split('+', '-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int offsetHours))
                {
                    int sign = timeZoneOffset.Contains('+') ? -1 : 1;
                    TimeSpan offset = TimeSpan.FromHours(sign * offsetHours);

                    DateTime dateTimeWithOffset = deadline + offset;
                    return dateTimeWithOffset.ToString("dd.MM.yyyy HH:mm:ss");
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateInfoMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3)
            {
                return DependencyProperty.UnsetValue;
            }

            DateTime signingDeadline = values[0] as DateTime? ?? DateTime.MinValue;
            DateTime signingDate = values[1] as DateTime? ?? DateTime.MinValue;
            DateTime conclusionDate = values[2] as DateTime? ?? DateTime.MinValue;

            if (signingDeadline == DateTime.MinValue && signingDate == DateTime.MinValue && conclusionDate == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                if (!DateTime.Equals(conclusionDate, DateTime.MinValue))
                {
                    return conclusionDate.ToString("dd.MM.yyyy");
                }
                else if (!DateTime.Equals(signingDate, DateTime.MinValue))
                {
                    return signingDate.ToString("dd.MM.yyyy");
                }
                else if (!DateTime.Equals(signingDeadline, DateTime.MinValue))
                {
                    return signingDeadline.ToString("dd.MM.yyyy");
                }
            }

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateInfoAdditionalInfoConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3)
            {
                return DependencyProperty.UnsetValue;
            }

            DateTime signingDeadline = values[0] as DateTime? ?? DateTime.MinValue;
            DateTime signingDate = values[1] as DateTime? ?? DateTime.MinValue;
            DateTime conclusionDate = values[2] as DateTime? ?? DateTime.MinValue;

            if (conclusionDate != DateTime.MinValue)
            {
                return "Заключен";
            }
            else if (signingDate != DateTime.MinValue)
            {
                return "Подписан";
            }
            else if (signingDeadline != DateTime.MinValue)
            {
                TimeSpan remainingDays = signingDeadline - DateTime.Today;
                if (remainingDays.TotalDays < 0)
                {
                    return "Просрочен";
                }
                else if (remainingDays.TotalDays == 0)
                {
                    return "Сегодня";
                }
                else
                {
                    return $"Осталось {Math.Floor(remainingDays.TotalDays)} д";
                }
            }
            else
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                TimeSpan remainingDays = date - DateTime.Today;
                if (remainingDays.TotalDays < 0)
                {
                    return "Просрочен";
                }
                else if (remainingDays.TotalDays == 0)
                {
                    return "Сегодня";
                }
                else
                {
                    return $"Осталось {Math.Floor(remainingDays.TotalDays)} д";
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class ComponentStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<ComponentStateCount> componentStates)
            {
                var seriesCollection = new SeriesCollection();
                foreach (var componentState in componentStates)
                {
                    var color = StatusColorProvider.GetColor(componentState.State);
                    seriesCollection.Add(new ColumnSeries
                    {
                        Title = componentState.State,
                        Values = new ChartValues<int> { componentState.Count },
                        Fill = color
                    });
                }
                return seriesCollection;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CalculatingCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id)
            {
                GET.Aggregate.ComponentClculationCountBy(id);
                return GET.Aggregate.ComponentClculationCountBy(id);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ApplicationsVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return Visibility.Collapsed;

            bool applications = values[0] != null && (bool)values[0];
            int? parentProcurementId = values[1] as int?;

            if (parentProcurementId != null)
                return Visibility.Visible;
            else if (applications)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ApplicationsOrNumberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return null;

            bool applications = values[0] != null && (bool)values[0];
            int? procurementId = values[1] as int?;
            int? applicationNumber = values[2] as int?;

            if (applications && procurementId.HasValue)
            {
                int count = GET.Aggregate.CountOfApplications(procurementId.Value);
                return count.ToString();
            }

            return applicationNumber?.ToString() ?? "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ParentProcurementIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? parentProcurementId = value as int?;

            if (parentProcurementId.HasValue)
            {
                return parentProcurementId.Value.ToString();
            }
            else
            {
                return "Parent";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrEmpty(base64String))
            {
                try
                {
                    byte[] imageBytes = System.Convert.FromBase64String(base64String);
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    return bitmap;
                }
                catch
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/PlaceholderEmployeePhotoPreview.png"));
                }
            }

            return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/PlaceholderEmployeePhotoPreview.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class InverseScaleYConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ScaleTransform(1, -1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RepeatedStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Procurement procurement)
            {
                var newStatusCount = GET.Aggregate.CountNewStatusByProcurementId(procurement.Id);

                if (newStatusCount >= 2 && procurement.ProcurementState.Kind == "Новый")
                {
                    return "Red";
                }
            }
            else if (value is ProcurementsEmployee procurementsEmployee)
            {
                var newStatusCount = GET.Aggregate.CountNewStatusByProcurementId(procurementsEmployee.Procurement.Id);

                if (newStatusCount >= 2 && procurementsEmployee.Procurement.ProcurementState.Kind == "Новый")
                {
                    return "Red";
                }
            }
            return "Transperent"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
