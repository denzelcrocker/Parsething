using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
                else if (amount != null && type == "Calculating")
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
                if (amount != null && type == "Purchase")
                    return (((decimal)reserveContractAmount - (decimal)amount) / (decimal)amount * 100).ToString("N0") + "%";
                else if (amount != null && type == "Calculating")
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
}
