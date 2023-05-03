using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Functions
{
    public class Profile
    {
        
    }
    class MyClassComparer : IEqualityComparer<ComponentCalculation>
    {
        public bool Equals(ComponentCalculation x, ComponentCalculation y)
        {
            return x.ProcurementId == y.ProcurementId; // MyProperty - это свойство, по которому нужно удалять дубликаты
        }

        public int GetHashCode(ComponentCalculation obj)
        {
            return obj.ProcurementId.GetHashCode();
        }
    }
}
