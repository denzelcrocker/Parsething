using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    internal class GlobalUsingValues : INotifyPropertyChanged
    {
        private static readonly Lazy<GlobalUsingValues> _lazyInstance = new Lazy<GlobalUsingValues>(() => new GlobalUsingValues());

        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _lockObject = new();

        // Конструктор
        private GlobalUsingValues()
        {
            CurrentSortingField = "ActualDeliveryDate";
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Procurements = new List<Procurement>();
            Comments = new List<Comment>();
            Comments = GET.View.CommentsForLazyLoading();
            StartAutoUpdateComments();

        }

        // Реализация синглтона
        public static GlobalUsingValues Instance => _lazyInstance.Value;

        // Событие, которое уведомляет об изменениях свойств
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

        private List<Comment> _comments;
        public List<Comment> Comments
        {
            get { return _comments; }
            private set
            {
                if (_comments != value)
                {
                    _comments = value;
                    OnPropertyChanged(nameof(Comments)); // Уведомление об изменении
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
            lock (_lockObject)
            {
                Procurements.Clear();

                if (procurements != null && procurements.Any())
                {
                    Procurements.AddRange(procurements);
                }
            }
        }

        public void AddProcurement(Procurement procurement)
        {
            lock (_lockObject)
            {
                Procurements.Add(procurement);
            }
        }

        public void ChangeCurrentSortingField(string field)
        {
            CurrentSortingField = field;
        }

        // Методы для работы с комментариями
        public async void StartAutoUpdateComments()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), _cancellationTokenSource.Token);
                    UpdateComments();
                }
            }
            catch (TaskCanceledException)
            {
                // Обновление остановлено
            }
        }

        public void StopAutoUpdateComments()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void UpdateComments()
        {
            var newComments = GET.View.CommentsForLazyLoading();

            lock (_lockObject)
            {
                Comments = newComments;
            }
        }
    }
}