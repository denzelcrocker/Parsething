using Parsething.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething
{
    internal class CurrentList
    {
        public static Employee employee;
        public static List<Employee> employees = new List<Employee>();
        public static ParsethingContext db = new ParsethingContext();
    }
}
