using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Parsething.Classes
{
    class StatusColorProvider
    {
        public static readonly Dictionary<string, SolidColorBrush> StatusColors = new Dictionary<string, SolidColorBrush>
    {
        { "Проблема", new SolidColorBrush(Color.FromRgb(255, 105, 97)) },           // Ярко-красный
        { "ТО: Обработка", new SolidColorBrush(Color.FromRgb(255, 165, 0)) },       // Ярко-оранжевый
        { "ТО: Согласовано", new SolidColorBrush(Color.FromRgb(50, 205, 50)) },     // Ярко-зелёный
        { "Наличие", new SolidColorBrush(Color.FromRgb(30, 144, 255)) },            // Ярко-голубой
        { "Купить", new SolidColorBrush(Color.FromRgb(75, 0, 130)) },               // Индиго
        { "Запрос цен", new SolidColorBrush(Color.FromRgb(238, 130, 238)) },        // Ярко-фиолетовый
        { "Оплатить", new SolidColorBrush(Color.FromRgb(255, 255, 0)) },            // Ярко-желтый
        { "Заказ", new SolidColorBrush(Color.FromRgb(0, 128, 128)) },               // Тёмно-бирюзовый
        { "Транзит", new SolidColorBrush(Color.FromRgb(255, 20, 147)) },            // Ярко-розовый
        { "В пути", new SolidColorBrush(Color.FromRgb(0, 191, 255)) },              // Ярко-синий
        { "На складе", new SolidColorBrush(Color.FromRgb(173, 216, 230)) },         // Светло-голубой
        { "В резерве", new SolidColorBrush(Color.FromRgb(124, 252, 0)) }            // Ярко-салатовый
    };

        public static SolidColorBrush GetColor(string status)
        {
            if (StatusColors.TryGetValue(status, out var color))
            {
                return color;
            }
            return new SolidColorBrush(Colors.Gray); // Цвет по умолчанию, если статус не найден
        }
    }
}
