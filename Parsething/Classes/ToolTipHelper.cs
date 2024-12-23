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
                case "Текущая":
                case "Следующая":
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
                case "Очередь":
                    procurements = GET.View.ProcurementsQueue();

                    // Логика для "Очередь" (оставляем как есть)
                    var currentDate = DateTime.Now.Date;
                    var futureProcurements = procurements
                        .Where(p => p.Deadline.HasValue &&
                                    p.Deadline.Value.Date >= currentDate &&
                                    p.Deadline.Value.Date <= currentDate.AddDays(5))
                        .GroupBy(p => p.Deadline.Value.Date)
                        .ToDictionary(g => g.Key, g => g.Count());

                    var queueInfo = new StringBuilder();
                    for (int i = 0; i <= 5; i++)
                    {
                        var date = currentDate.AddDays(i);
                        var count = futureProcurements.ContainsKey(date) ? futureProcurements[date] : 0;
                        queueInfo.AppendLine($"{date:dd.MM.yyyy} - {count} тендеров");
                    }

                    var queueToolTip = new ToolTip
                    {
                        Content = queueInfo.ToString(),
                        Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
                    };

                    ToolTipService.SetInitialShowDelay(element, 0);
                    ToolTipService.SetShowDuration(element, 60000);
                    ToolTipService.SetBetweenShowDelay(element, 200);

                    element.ToolTip = queueToolTip;
                    return;

                case "Новые":
                    procurements = GET.View.ProcurementsBy("Новый", GET.KindOf.ProcurementState);

                    currentDate = DateTime.Now.Date;
                    var newProcurements = procurements
                        .Where(p => p.Deadline.HasValue &&
                                    p.Deadline.Value.Date >= currentDate &&
                                    p.Deadline.Value.Date <= currentDate.AddDays(5))
                        .GroupBy(p => p.Deadline.Value.Date)
                        .ToDictionary(g => g.Key, g => g.Count());

                    var newProcurementsInfo = new StringBuilder();
                    for (int i = 0; i <= 5; i++)
                    {
                        var date = currentDate.AddDays(i);
                        var count = newProcurements.ContainsKey(date) ? newProcurements[date] : 0;
                        newProcurementsInfo.AppendLine($"{date:dd.MM.yyyy} - {count} тендеров");
                    }

                    var newProcurementsToolTip = new ToolTip
                    {
                        Content = newProcurementsInfo.ToString(),
                        Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
                    };

                    ToolTipService.SetInitialShowDelay(element, 0);
                    ToolTipService.SetShowDuration(element, 60000);
                    ToolTipService.SetBetweenShowDelay(element, 200);

                    element.ToolTip = newProcurementsToolTip;
                    return;
            }

            // Логика для NotPaid, NotPaidOnTime, NotPaidDelay
            if (parameter == "NotPaid" || parameter == "NotPaidOnTime" || parameter == "NotPaidDelay")
            {
                var totalContractAmount = procurements.Sum(p => p.GetFinalAmount());
                var totalPaidAmount = procurements.Sum(p => p.Amount); // Предполагается, что Amount хранит сумму оплаты
                var amountLeftToPay = totalContractAmount - totalPaidAmount;

                var toolTipContent = $"Осталось оплатить - {amountLeftToPay:N2} р.";
                var toolTip = new ToolTip
                {
                    Content = toolTipContent,
                    Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
                };

                ToolTipService.SetInitialShowDelay(element, 0);
                ToolTipService.SetShowDuration(element, 60000);
                ToolTipService.SetBetweenShowDelay(element, 200);

                element.ToolTip = toolTip;
                return;
            }

            // Для остальных случаев - общая сумма контрактов
            totalAmount = procurements.Sum(p => p.GetFinalAmount());
            var contractSumToolTip = new ToolTip
            {
                Content = $"Сумма контрактов - {totalAmount:N2} р.",
                Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
            };

            ToolTipService.SetInitialShowDelay(element, 0);
            ToolTipService.SetShowDuration(element, 60000);
            ToolTipService.SetBetweenShowDelay(element, 200);

            element.ToolTip = contractSumToolTip;
        }
        public static void SetToolTipProcurementEmployee(FrameworkElement element, string parameter)
        {
            var procurements = new List<Procurement>();
            var procurementsEmployees = new List<ProcurementsEmployee>();
            var totalAmount = 0m;

            switch (parameter)
            {
                case "Выигран 1ч":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 1ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Выигран 2ч":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Выигран 2ч", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Applications":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("", GET.KindOf.Applications, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Предыдущая":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Текущая":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Следующая":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Через одну":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(parameter, GET.KindOf.ShipmentPlane, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Принят":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Принят", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "Приемка":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.ProcurementState, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "CorrectionDate":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy("Приемка", GET.KindOf.CorrectionDate, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "NotPaid":
                    procurementsEmployees = GET.View.ProcurementsEmployeesNotPaid(((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "NotPaidOnTime":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(false, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
                case "NotPaidDelay":
                    procurementsEmployees = GET.View.ProcurementsEmployeesBy(true, ((Employee)Application.Current.MainWindow.DataContext).Id, "Appoint");
                    break;
            }
            procurements = Functions.Conversion.ProcurementsEmployeesConversion(procurementsEmployees);
            if (parameter == "NotPaid" || parameter == "NotPaidOnTime" || parameter == "NotPaidDelay")
            {
                var totalContractAmount = procurements.Sum(p => p.GetFinalAmount());
                var totalPaidAmount = procurements.Sum(p => p.Amount); // Предполагается, что Amount хранит сумму оплаты
                var amountLeftToPay = totalContractAmount - totalPaidAmount;

                var toolTipContent = $"Осталось оплатить - {amountLeftToPay:N2} р.";
                var toolTip = new ToolTip
                {
                    Content = toolTipContent,
                    Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
                };

                ToolTipService.SetInitialShowDelay(element, 0);
                ToolTipService.SetShowDuration(element, 60000);
                ToolTipService.SetBetweenShowDelay(element, 200);

                element.ToolTip = toolTip;
                return;
            }

            // Для остальных случаев - общая сумма контрактов
            totalAmount = procurements.Sum(p => p.GetFinalAmount());
            var contractSumToolTip = new ToolTip
            {
                Content = $"Сумма контрактов - {totalAmount:N2} р.",
                Style = (Style)Application.Current.Resources["ComponentCalculation.ToolTip"]
            };

            ToolTipService.SetInitialShowDelay(element, 0);
            ToolTipService.SetShowDuration(element, 60000);
            ToolTipService.SetBetweenShowDelay(element, 200);

            element.ToolTip = contractSumToolTip;
        }
    }
}
