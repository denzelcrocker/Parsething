using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Parsething.Models
{
    public class MenuItemModel : INotifyPropertyChanged
    {
        public string Header { get; set; }
        public string ImageSource { get; set; }
        public List<MenuItemModel> SubItems { get; set; }
        public ICommand Command { get; set; }
        public int RecordCount { get; set; }

        public bool HasSubItems => SubItems != null && SubItems.Any();
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}