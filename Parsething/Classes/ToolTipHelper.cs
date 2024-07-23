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
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Выигран 2ч":
                    procurements = GET.View.ProcurementsBy("Выигран 2ч", GET.KindOf.ProcurementState);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Applications":
                    procurements = GET.View.ProcurementsBy("", GET.KindOf.Applications);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Предыдущая":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Текущая":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Следующая":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Через одну":
                    procurements = GET.View.ProcurementsBy(parameter, GET.KindOf.ShipmentPlane);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "Принят":
                    procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.ProcurementState);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "CorrectionDate":
                    procurements = GET.View.ProcurementsBy("Приемка", GET.KindOf.CorrectionDate);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "NotPaid":
                    procurements = GET.View.ProcurementsNotPaid();
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "NotPaidInTime":
                    procurements = GET.View.ProcurementsBy(false);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
                case "NotPaidDelay":
                    procurements = GET.View.ProcurementsBy(true);
                    totalAmount = procurements.Sum(p => p.GetFinalAmount());
                    break;
            }

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
