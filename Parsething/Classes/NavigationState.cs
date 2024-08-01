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
        public static void RestoreSelection(IEnumerable<Procurement> procurements, ListView listView)
        {
            if (LastSelectedProcurement != null)
            {
                var selectedProcurement = procurements
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
