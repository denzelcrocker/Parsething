﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Functions
{
    internal class Conversion
    {
        private static List<Procurement> procurements = new List<Procurement>();

        public static List<Procurement> ConponentCalculationsConversion(List<ComponentCalculation> componentCalculations)
        {
            procurements = new List<Procurement>();
            if (componentCalculations != null)
            {
                foreach (var componentCalculation in componentCalculations)
                {
                    Procurement procurement = componentCalculation.Procurement;
                    procurements.Add(procurement);
                }
            }
            return procurements;
        }
        public static List<Procurement> ProcurementsEmployeesConversion(List<ProcurementsEmployee> procurementsEmployees)
        {
            procurements = new List<Procurement>();
            if (procurementsEmployees != null)
            {
                foreach (var procurementsEmployee in procurementsEmployees)
                {
                    Procurement procurement = procurementsEmployee.Procurement;
                    procurements.Add(procurement);
                }
            }
            return procurements;
        }
    }
}
