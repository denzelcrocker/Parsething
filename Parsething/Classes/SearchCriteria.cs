using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    internal class SearchCriteria
    {
        private static SearchCriteria instance;

        private SearchCriteria()
        {

        }

        public static SearchCriteria Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SearchCriteria();
                }
                return instance;
            }
        }

        public void ClearData()
        {
            ProcurementId = null;
            ProcurementNumber = null;
            Law = null;
            ProcurementState = null;
            ProcurementStateSecond = null;
            INN = null;
            Employee = null;
            OrganizationName = null;
            LegalEntity = null;
            DateType = null;
            StartDate = null;
            EndDate = null;
            ComponentCalculation = null;
            ShipmentPlan = null;
            WaitingList = null;
            ContractNumber = null;
        }

        public string? ProcurementId { get; set; }
        public string? ProcurementNumber { get; set; }
        public string? Law { get; set; }
        public string? ProcurementState { get; set; }
        public string? ProcurementStateSecond { get; set; }
        public string? INN { get; set; }
        public string? Employee { get; set; }
        public string? OrganizationName { get; set; }
        public string? LegalEntity { get; set; }
        public string? DateType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? ComponentCalculation { get; set; }
        public string? ShipmentPlan { get; set; }
        public bool? WaitingList { get; set; }
        public string? ContractNumber { get; set; }
    }
}
