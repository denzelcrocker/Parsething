using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    public static class NavigationState
    {
        public static Procurement? LastSelectedProcurement { get; set; }
        public static void AddLastSelectedProcurement(ListView listView)
        {
            if (LastSelectedProcurement != null)
            {
                var selectedProcurement = GlobalUsingValues.Instance.Procurements
                    .FirstOrDefault(p => p.Id == LastSelectedProcurement.Id);

                if (selectedProcurement != null)
                {
                    listView.SelectedItem = selectedProcurement;
                }

                LastSelectedProcurement = null;
            }
        }
    }
}
