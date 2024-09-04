using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    public static class ToolTipHelper
    {
        public static void SetToolTip(FrameworkElement element, string parameter)
        {
            var procurements = new List<Procurement>();
            var totalAmount = 0m;

            switch (parameter)
            {
                case "Выигран 1ч":
                    procurements = GET.View.ProcurementsBy("Выигран 1ч", GET.KindOf.ProcurementState);
                    break;
                case "Выигран 2ч":
                    procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
                    break;
                case "Applications":
                    procurements = GET.View.ProcurementsBy("", GET.KindOf.Applications);
                    break;
                case "Предыдущая":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    break;
                case "Текущая":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    break;
                case "Следующая":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    break;
                case "Через одну":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    break;
                case "Принят":
                    procurements = GET.View.ProcurementsBy("Принят", GET.KindOf.ProcurementState);
                    break;
                case "Приемка":
                    procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
                    break;
                case "CorrectionDate":
                    procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate);
                    break;
                case "NotPaid":
                    procurements = GET.View.ProcurementsNotPaid();
                    break;
                case "NotPaidOnTime":
                    procurements = GET.View.ProcurementsBy(false);
                    break;
                case "NotPaidDelay":
                    procurements = GET.View.ProcurementsBy(true);
                    break;
            }
            totalAmount = procurements.Sum(p => p.GetFinalAmount());

            var toolTip = new ToolTip
            {
                Content = $"{totalAmount:N2} р.",
                Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
            };

            ToolTipService.SetInitialShowDelay(element, 0);
            ToolTipService.SetShowDuration(element, 60000);
            ToolTipService.SetBetweenShowDelay(element, 200);

            element.ToolTip = toolTip;
        }
        public static void SetToolTipProcurementEmployee(FrameworkElement element, string parameter)
        {
            var procurements = new List<Procurement>();
            var procurementsEmployees = new List<ProcurementsEmployee>();
            var totalAmount = 0m;

            switch (parameter)
            {
                case "Выигран 1ч":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Выигран 2ч":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 2ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Applications":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("", GET.KindOf.Applications, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Предыдущая":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Текущая":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Следующая":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Через одну":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Принят":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Принят", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "Приемка":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "CorrectionDate":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.CorrectionDate, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "NotPaid":
                    procurementsEmployees = GET.View.ProcurementsEmployeesNotPaid(((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "NotPaidInTime":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
                case "NotPaidDelay":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id);
                    break;
            }
            procurements = Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees);
            totalAmount = procurements.Sum(p => p.GetFinalAmount());

            var toolTip = new ToolTip
            {
                Content = $"{totalAmount:N2} р.",
                Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
            };

            ToolTipService.SetInitialShowDelay(element, 0);
            ToolTipService.SetShowDuration(element, 60000);
            ToolTipService.SetBetweenShowDelay(element, 200);

            element.ToolTip = toolTip;
        }
    }
}
