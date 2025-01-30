using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Parsething.Functions
{
    class FindPopup
    {
        public static Popup FindPopupByProcurementId(int procurementId, Button button)
        {
            Grid grid = FindParent<Grid>(button);

            foreach (var child in FindVisualChildren<Popup>(grid))
            {
                if (child.DataContext is Procurement procurement && procurement.Id == procurementId)
                {
                    return child;
                }
            }

            return null;
        }
        public static Popup FindPopupByProcurementId(int procurementId, Button button, string name)
        {
            Grid grid = FindParent<Grid>(button);

            foreach (var child in FindVisualChildren<Popup>(grid))
            {
                if (child.DataContext is Procurement procurement && procurement.Id == procurementId && child.Name == name)
                {
                    return child;
                }
            }

            return null;
        }
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T descendant in FindVisualChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }
    }
}
