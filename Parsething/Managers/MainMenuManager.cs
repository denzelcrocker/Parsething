using Parsething.Classes;
using Parsething.Commands;
using Parsething.Interfaces;
using Parsething.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Managers
{
    public class MainMenuManager
    {
        private readonly IProcurementViewModel _viewModel;

        public MainMenuManager(IProcurementViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        public List<MenuItemModel> GetMenuForUser(string userType)
        {
            if (userType == "Администратор")
            {
                return new List<MenuItemModel>
            {
                new MenuItemModel
                {
                    Header = "Сортировка",
                    SubItems = new List<MenuItemModel>
                    {
                        new MenuItemModel { Header = "Получены", RecordCount = GET.Aggregate.ProcurementsCountBy("Отбой", GET.KindOf.ProcurementState), Command = new RelayCommand(() => LoadList("Получены")) },
                        new MenuItemModel { Header = "Неразобранные", RecordCount = 5, Command = new RelayCommand(() => LoadList("Unsorted")) }
                    }
                },
                new MenuItemModel
                {
                    Header = "Расчет и подача",
                    SubItems = new List<MenuItemModel>
                    {
                        new MenuItemModel { Header = "Получены", RecordCount = 10, Command = new RelayCommand(() => LoadList("Получены")) },
                        new MenuItemModel { Header = "Неразобранные", RecordCount = 5, Command = new RelayCommand(() => LoadList("Получены")) },
                    }
                },
            };
            }
            else if (userType == "Specialist")
            {
                return new List<MenuItemModel>
            {
                new MenuItemModel
                {
                    Header = "Расчет",
                    SubItems = new List<MenuItemModel>
                    {
                        new MenuItemModel { Header = "В работе", RecordCount = 15, Command = new RelayCommand(() => LoadList("Получены")) },
                        new MenuItemModel { Header = "На рассмотрении", RecordCount = 7, Command = new RelayCommand(() => LoadList("Получены")) }
                    }
                }
            };
            }

            return new List<MenuItemModel>();
        }

        private void LoadList(string listName)
        {
            var procurements = GET.View.ProcurementsBy("Отбой", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            _viewModel.Procurements = new ObservableCollection<Procurement>(procurements);

        }
    }
}
