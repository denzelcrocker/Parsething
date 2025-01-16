using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Parsething.Classes
{
    internal class GlobalUsingValues : INotifyPropertyChanged
    {
        private static readonly Lazy<GlobalUsingValues> _lazyInstance = new Lazy<GlobalUsingValues>(() => new GlobalUsingValues());

        // Конструктор
        private GlobalUsingValues()
        {
            CurrentSortingField = "ActualDeliveryDate";
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Procurements = new List<Procurement>();
        }

        // Реализация синглтона
        public static GlobalUsingValues Instance => _lazyInstance.Value;

        // Событие, которое уведомляет о изменениях свойств
        public event PropertyChangedEventHandler PropertyChanged;

        // Метод для уведомления об изменении свойства
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Свойства
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            private set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate)); // Уведомление об изменении
                }
            }
        }

        private List<Procurement> _procurements;
        public List<Procurement> Procurements
        {
            get { return _procurements; }
            private set
            {
                if (_procurements != value)
                {
                    _procurements = value;
                    OnPropertyChanged(nameof(Procurements)); // Уведомление об изменении
                }
            }
        }

        private string _currentSortingField;
        public string CurrentSortingField
        {
            get { return _currentSortingField; }
            private set
            {
                if (_currentSortingField != value)
                {
                    _currentSortingField = value;
                    OnPropertyChanged(nameof(CurrentSortingField)); // Уведомление об изменении
                }
            }
        }

        // Методы для работы с данными
        public void AddProcurements(List<Procurement> procurements)
        {
            Procurements.Clear();

            if (procurements != null && procurements.Any())
            {
                Procurements.AddRange(procurements);
            }
        }
        public void ChangeCurrentSortingField(string field)
        {
            CurrentSortingField = field;
        }
        public void AddProcurement(Procurement procurement)
        {
            Procurements.Add(procurement);
        }
    }
}