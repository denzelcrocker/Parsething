using Parsething.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Parsething.Managers;
using System.Collections.ObjectModel;
using Parsething.Interfaces;

namespace Parsething.ViewModels
{
    public class AdministratorPageViewModel : INotifyPropertyChanged, IProcurementViewModel
    {
        private readonly MainMenuManager _mainMenuManager;

        public AdministratorPageViewModel()
        {
            _mainMenuManager = new MainMenuManager(this);
            MenuItems = _mainMenuManager.GetMenuForUser("Администратор");
        }

        private List<MenuItemModel> _menuItems;
        public List<MenuItemModel> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Procurement> _procurements;

        public ObservableCollection<Procurement> Procurements
        {
            get => _procurements;
            set
            {
                _procurements = value;
                OnPropertyChanged(nameof(Procurements)); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
