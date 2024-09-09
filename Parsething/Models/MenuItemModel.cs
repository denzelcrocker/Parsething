using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Models
{
    public class MenuItemModel
    {
        public string Header { get; set; } // Текст пункта меню
        public List<MenuItemModel> SubItems { get; set; } // Подменю
        public int? RecordCount { get; set; }  // Количество записей для подменю
        public ICommand Command { get; set; }  // Команда для выполнения действия
    }
}
