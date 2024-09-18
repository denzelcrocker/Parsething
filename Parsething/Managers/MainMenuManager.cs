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
        public void GetMenuForUser(string userType)
        {
            GlobalUsingValues.Instance.MainMenuItems.Clear();
            if (userType == "Администратор")
            {
                GlobalUsingValues.Instance.AddMainMenuItem(new MenuItemModel
                {
                    Header = "Сортировка",
                    ImageSource = "/Resources/Images/Gear.png",
                    Command = new RelayCommand(() => LoadList("Получены")),
                    SubItems = new List<MenuItemModel>
            {
                new MenuItemModel { Header = "Получены", RecordCount = 15 },
                new MenuItemModel
                {
                    Header = "План отгрузки",
                    RecordCount = 15,
                    SubItems = new List<MenuItemModel>
                    {
                        new MenuItemModel { Header = "Текущая неделя", RecordCount = 15 },
                        new MenuItemModel { Header = "Следующая неделя" }
                    }
                }
            }
                });

                GlobalUsingValues.Instance.AddMainMenuItem(new MenuItemModel
                {
                    Header = "Расчет и подача",
                    SubItems = new List<MenuItemModel>
            {
                new MenuItemModel { Header = "Получены" },
                new MenuItemModel
                {
                    Header = "План отгрузки",
                    SubItems = new List<MenuItemModel>
                    {
                        new MenuItemModel { Header = "Текущая неделя", RecordCount = 15 },
                        new MenuItemModel { Header = "Следующая неделя" }
                    }
                }
            }
                });
            }
            else if (userType == "Specialist")
            {
                GlobalUsingValues.Instance.AddMainMenuItem(new MenuItemModel
                {
                    Header = "Расчет",
                    SubItems = new List<MenuItemModel>
            {
                new MenuItemModel { Header = "В работе", RecordCount = 15, Command = new RelayCommand(() => LoadList("Получены")) },
                new MenuItemModel { Header = "На рассмотрении", RecordCount = 7, Command = new RelayCommand(() => LoadList("Получены")) }
            }
                });
            }
        }

        private void LoadList(string listName)
        {
            var procurements = GET.View.ProcurementsBy("Отбой", GET.KindOf.ProcurementState) ?? new List<Procurement>();
            GlobalUsingValues.Instance.AddProcurements(procurements);
            _viewModel.Procurements = new ObservableCollection<Procurement>(procurements);

        }
    }
}
