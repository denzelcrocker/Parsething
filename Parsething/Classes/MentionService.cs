using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Parsething.Classes
{
    class MentionService
    {
        private TextBox _textBox;
        private Popup _popup;
        private Procurement _procurement;
        private ListBox _listBox;
        private List<Employee> _employees;
        private List<Position> _positions;

        public MentionService(TextBox textBox, Popup popup, ListBox listBox, List<Employee> employees, List<Position> positions, Procurement procurement)
        {
            _textBox = textBox;
            _popup = popup;
            _listBox = listBox;
            _employees = employees;
            _positions = positions;
            _procurement = procurement;

            _textBox.TextChanged += CommentsTextBox_TextChanged;
            _textBox.PreviewKeyDown += CommentsTextBox_PreviewKeyDown;
            _listBox.SelectionChanged += EmployeesListBox_SelectionChanged;
            _listBox.MouseLeftButtonUp += EmployeesListBox_MouseLeftButtonUp;
        }

        private void CommentsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = _textBox.Text;
            int atIndex = text.LastIndexOf('@');

            if (atIndex != -1 && (atIndex == 0 || text[atIndex - 1] == ' '))
            {
                string searchText = text.Substring(atIndex + 1).ToLower();

                var filteredUsers = _employees
                    .Where(e => e.FullName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                    .Select(e => new SuggestionItem { Name = e.FullName, Type = "User", Data = e });

                var filteredRoles = _positions
                    .Where(p => p.Kind.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                    .Select(p => new SuggestionItem { Name = p.Kind, Type = "Role", Data = p });

                var combinedSuggestions = filteredUsers.Concat(filteredRoles).ToList();

                _listBox.ItemsSource = combinedSuggestions;
                _popup.IsOpen = combinedSuggestions.Count > 0;
            }
            else
            {
                _popup.IsOpen = false;
            }
        }

        private void CommentsTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                string text = _textBox.Text;
                int atIndex = text.LastIndexOf('@');

                if (atIndex != -1)
                {
                    string searchText = text.Substring(atIndex + 1).ToLower();
                    var matchingEmployee = _employees.FirstOrDefault(u => u.FullName.ToLower().StartsWith(searchText));

                    if (matchingEmployee != null)
                    {
                        string newText = text.Substring(0, atIndex + 1) + matchingEmployee.UserName + " ";
                        _textBox.Text = newText;

                        _textBox.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            _textBox.CaretIndex = _textBox.Text.Length;
                        }));
                    }
                }
            }
        }

        private void EmployeesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InsertSelectedUser();
        }

        private void EmployeesListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InsertSelectedUser();
        }

        private void InsertSelectedUser()
        {
            if (_listBox.SelectedItem is SuggestionItem suggestionItem)
            {
                string selectedText = suggestionItem.Type == "User"
                    ? ((Employee)suggestionItem.Data).UserName
                    : ((Position)suggestionItem.Data).Kind;

                InsertMentionedUser(selectedText);
            }
        }

        private void InsertMentionedUser(string mention)
        {
            string text = _textBox.Text;
            int atIndex = text.LastIndexOf('@');

            if (atIndex != -1)
            {
                _textBox.Text = text.Substring(0, atIndex + 1) + mention + " ";
                _textBox.Dispatcher.BeginInvoke((Action)(() =>
                {
                    _textBox.CaretIndex = _textBox.Text.Length;
                }));
            }

            _popup.IsOpen = false;
        }
        private List<Employee> GetMentionedEmployees(string commentText)
        {
            List<Employee> mentionedUsers = new List<Employee>();
            var words = commentText.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (word.StartsWith("@"))
                {
                    string mention = word.Substring(1); // Убираем '@'

                    // Ищем среди пользователей по UserName
                    Employee? employee = _employees.FirstOrDefault(e => e.UserName.Equals(mention, StringComparison.OrdinalIgnoreCase));
                    if (employee != null)
                    {
                        mentionedUsers.Add(employee);
                        continue;
                    }

                    // Ищем среди ролей
                    Position? position = _positions.FirstOrDefault(p => p.Kind.Equals(mention, StringComparison.OrdinalIgnoreCase));
                    if (position != null)
                    {
                        List<Employee> employeesInRole = _employees.Where(e => e.PositionId == position.Id).ToList();
                        mentionedUsers.AddRange(employeesInRole);
                    }
                }
            }
            return mentionedUsers.Distinct().ToList(); // Убираем дубликаты
        }
        public void NotifyMentionedUsers(string commentText)
        {
            List<Employee> mentionedEmployees = GetMentionedEmployees(commentText);
            foreach (Employee employee in mentionedEmployees)
            {
                SendNotification(employee, commentText);
            }
        }
        private void SendNotification(Employee employee, string commentText)
        {
            Notification notification = new Notification()
            {
                Title = $"Вас упомянули в комментарии к тендеру {_procurement.DisplayId}",
                Text = commentText,
                EmployeeId = ((Employee)Application.Current.MainWindow.DataContext).Id,
                DateCreated = DateTime.Now,
            };
            if (PUT.Notification(notification))
                notification = GET.Entry.Notification(notification.Title, notification.Text, notification.EmployeeId);
            // Создание EmployeeNotification на основе Notification
            EmployeeNotification employeeNotification = new EmployeeNotification
            {
                EmployeeId = employee.Id, // Используем Value для получения int
                NotificationId = notification.Id, // Используем полученный Id
                IsRead = false, // Установите значение по умолчанию
                IsDeleted = false, // Установите значение по умолчанию
                DateRead = null // Установите значение по умолчанию
            };

            // Отправка EmployeeNotification
            PUT.EmployeeNotification(employeeNotification);
        }
    }

    public class SuggestionItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
    }
}


