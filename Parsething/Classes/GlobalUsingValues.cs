using Parsething.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Parsething.Classes
{
    internal class GlobalUsingValues
    {
        private static readonly Lazy<GlobalUsingValues> _lazyInstance = new Lazy<GlobalUsingValues>(() => new GlobalUsingValues());

        private GlobalUsingValues()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Procurements = new List<Procurement>();
            MainMenuItems = new List<MenuItemModel>();
        }

        public static GlobalUsingValues Instance => _lazyInstance.Value;

        public DateTime StartDate { get; private set; }
        public List<Procurement> Procurements { get; private set; }
        public List<MenuItemModel> MainMenuItems { get; private set; }


        public void AddProcurements(List<Procurement> procurements)
        {
            Procurements.Clear();

            if (procurements != null && procurements.Any())
            {
                Procurements.AddRange(procurements);
            }
        }
        public void AddProcurement(Procurement procurement)
        {
            Procurements.Add(procurement);
        }
        public void AddMainMenuItem(MenuItemModel menuItemModel)
        {
            MainMenuItems.Add(menuItemModel);
        }
        public void DeselectAll(List<MenuItemModel> menuItems)
        {
            foreach (var item in menuItems)
            {
                DeselectRecursive(item);
            }
        }

        private void DeselectRecursive(MenuItemModel item)
        {
            item.IsSelected = false;
            if (item.SubItems != null)
            {
                foreach (var subItem in item.SubItems)
                {
                    DeselectRecursive(subItem);
                }
            }
        }
    }
}
