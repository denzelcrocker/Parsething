using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Functions
{
    public static class PULL
    {
        public static void ProcurementUpdate(Procurement procurementUpdate)
        {
            using ParsethingContext db = new();
            Procurement? procurement = db.Procurements.Where(e => e.Number == procurementUpdate.Number).First();
            procurement.RequestUri = procurementUpdate.RequestUri;
            procurement.Number = procurementUpdate.Number;
            procurement.LawId = procurementUpdate.LawId;
            procurement.Object = procurementUpdate.Object;
            procurement.InitialPrice = procurementUpdate.InitialPrice;
            procurement.OrganizationId = procurementUpdate.OrganizationId;
            procurement.MethodId = procurementUpdate.MethodId;
            procurement.PlatformId = procurementUpdate.PlatformId;
            procurement.Location = procurementUpdate.Location;
            procurement.StartDate = procurementUpdate.StartDate;
            procurement.Deadline = procurementUpdate.Deadline;
            procurement.TimeZoneId = procurementUpdate.TimeZoneId;
            procurement.Securing = procurementUpdate.Securing;
            procurement.Enforcement = procurementUpdate.Enforcement;
            procurement.Warranty = procurementUpdate.Warranty;
            procurement.RegionId = procurementUpdate.RegionId;
            procurement.OrganizationContractName = procurementUpdate.OrganizationContractName;
            procurement.OrganizationContractPostalAddress = procurementUpdate.OrganizationContractPostalAddress;
            procurement.ContactPerson = procurementUpdate.ContactPerson;
            procurement.ContactPhone = procurementUpdate.ContactPhone;
            procurement.DeliveryDetails = procurementUpdate.DeliveryDetails;
            procurement.DeadlineAndType = procurementUpdate.DeadlineAndType;
            procurement.DeliveryDeadline = procurementUpdate.DeliveryDeadline;
            procurement.AcceptanceDeadline = procurementUpdate.AcceptanceDeadline;
            procurement.ContractDeadline = procurementUpdate.ContractDeadline;
            procurement.Indefinitely = procurementUpdate.Indefinitely;
            procurement.AnotherDeadline = procurementUpdate.AnotherDeadline;
            procurement.DeadlineAndOrder = procurementUpdate.DeadlineAndOrder;
            procurement.RepresentativeTypeId = procurementUpdate.RepresentativeTypeId;
            procurement.CommissioningWorksId = procurementUpdate.CommissioningWorksId;
            procurement.PlaceCount = procurementUpdate.PlaceCount;
            procurement.FinesAndPennies = procurementUpdate.FinesAndPennies;
            procurement.PenniesPerDay = procurementUpdate.PenniesPerDay;
            procurement.TerminationConditions = procurementUpdate.TerminationConditions;
            procurement.EliminationDeadline = procurementUpdate.EliminationDeadline;
            procurement.GuaranteePeriod = procurementUpdate.GuaranteePeriod;
            procurement.Inn = procurementUpdate.Inn;
            procurement.ContractNumber = procurementUpdate.ContractNumber;
            //employeeId
            procurement.AssemblyNeed = procurementUpdate.AssemblyNeed;
            procurement.MinopttorgId = procurementUpdate.MinopttorgId;
            procurement.LegalEntityId = procurementUpdate.LegalEntityId;
            procurement.Applications = procurementUpdate.Applications;
            procurement.Bet = procurementUpdate.Bet;
            procurement.MinimalPrice = procurementUpdate.MinimalPrice;
            procurement.ContractAmount = procurementUpdate.ContractAmount;
            procurement.ReserveContractAmount = procurementUpdate.ReserveContractAmount;
            procurement.ProtocolDate = procurementUpdate.ProtocolDate;
            procurement.ShipmentPlanId = procurementUpdate.ShipmentPlanId;
            procurement.WaitingList = procurementUpdate.WaitingList;
            procurement.Calculating = procurementUpdate.Calculating;
            procurement.Purchase = procurementUpdate.Purchase;
            procurement.ExecutionStateId = procurementUpdate.ExecutionStateId;
            procurement.WarrantyStateId = procurementUpdate.WarrantyStateId;
            procurement.SigningDeadline = procurementUpdate.SigningDeadline;
            procurement.SigningDate = procurementUpdate.SigningDate;
            procurement.ConclusionDate = procurementUpdate.ConclusionDate;
            procurement.ActualDeliveryDate = procurementUpdate.ActualDeliveryDate;
            procurement.DepartureDate = procurementUpdate.DepartureDate;
            procurement.DeliveryDate = procurementUpdate.DeliveryDate;
            procurement.MaxAcceptanceDate = procurementUpdate.MaxAcceptanceDate;
            procurement.CorrectionDate = procurementUpdate.CorrectionDate;
            procurement.ActDate = procurementUpdate.ActDate;
            procurement.MaxDueDate = procurementUpdate.MaxDueDate;
            procurement.ClosingDate = procurementUpdate.ClosingDate;
            procurement.RealDueDate = procurementUpdate.RealDueDate;
            procurement.Amount = procurementUpdate.Amount;
            procurement.SignedOriginalId = procurementUpdate.SignedOriginalId;
            procurement.Judgment = procurementUpdate.Judgment;
            procurement.Fas = procurementUpdate.Fas;
            procurement.ProcurementStateId = procurementUpdate.ProcurementStateId;
            db.SaveChanges();
        }
    }
}
