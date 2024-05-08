using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

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
                if (amount != null)
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
                    return "Green"; // Зеленый, если остается более 3 дней
                }
                else if (timeRemaining.TotalDays <= 3 && timeRemaining.TotalDays >= 1)
                {
                    return "Yellow"; // Желтый, если остается от 1 до 3 дней
                }
                else
                {
                    return "Red"; // Красный, если остается менее 1 дня
                }
            }
            return "Black"; // Черный по умолчанию
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
                if (timeZoneOffset == "МСК" && !timeZoneOffset.Contains("МСК+") && !timeZoneOffset.Contains("МСК-"))
                {
                    return deadline.ToString("dd.MM.yyyy HH:mm:ss");
                }

                string[] parts = timeZoneOffset.Split('+', '-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int offsetHours))
                {
                    int sign = timeZoneOffset.Contains('+') ? 1 : -1;
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
}
