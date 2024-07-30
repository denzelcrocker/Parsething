using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    internal class GlobalUsingValues
    {
        private static readonly Lazy<GlobalUsingValues> _lazyInstance = new Lazy<GlobalUsingValues>(() => new GlobalUsingValues());

        private GlobalUsingValues()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Procurements = new List<Procurement>();
        }

        public static GlobalUsingValues Instance => _lazyInstance.Value;

        public DateTime StartDate { get; private set; }
        public List<Procurement> Procurements { get; private set; }

        //public void AddProcurement(Procurement procurement)
        //{
        //    Procurements.Add(procurement);
        //}

        //public Procurement GetProcurementById(int id)
        //{
        //    return Procurements.FirstOrDefault(p => p.Id == id);
        //}
    }
}
