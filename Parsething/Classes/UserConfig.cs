using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parsething.Classes
{
    public static class UserConfig
    {
        private static readonly string ConfigDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parsething");
        private static readonly string ConfigFilePath = Path.Combine(ConfigDirectoryPath, "user.config");

        public static void SaveCredentials(string username, string password, string theme)
        {
            // Путь к конфигурационному файлу
            if (!Directory.Exists(ConfigDirectoryPath))
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
            }

            // Загружаем текущую конфигурацию, если файл существует
            XDocument doc = File.Exists(ConfigFilePath) ? XDocument.Load(ConfigFilePath) : new XDocument(new XElement("Configuration"));

            // Находим или создаём нужные элементы
            var usernameElement = doc.Root.Element("Username");
            if (usernameElement == null)
            {
                usernameElement = new XElement("Username");
                doc.Root.Add(usernameElement);
            }

            var passwordElement = doc.Root.Element("Password");
            if (passwordElement == null)
            {
                passwordElement = new XElement("Password");
                doc.Root.Add(passwordElement);
            }

            var themeElement = doc.Root.Element("Theme");
            if (themeElement == null)
            {
                themeElement = new XElement("Theme");
                doc.Root.Add(themeElement);
            }

            // Обновляем значения
            usernameElement.Value = username;
            passwordElement.Value = password;
            themeElement.Value = theme;

            // Сохраняем изменения в файл
            doc.Save(ConfigFilePath);
        }

        public static (string Username, string Password, string Theme) LoadCredentials()
        {
            if (File.Exists(ConfigFilePath))
            {
                var doc = XDocument.Load(ConfigFilePath);
                var username = doc.Root.Element("Username")?.Value ?? string.Empty;
                var password = doc.Root.Element("Password")?.Value ?? string.Empty;
                var theme = doc.Root.Element("Theme")?.Value ?? "Dark";  // Возвращаем "Dark" как дефолт
                return (username, password, theme);
            }

            return (string.Empty, string.Empty, "Dark"); // Если файл не существует, возвращаем дефолтное значение
        }

        // Добавим сохранение темы
        public static void SaveTheme(string theme)
        {
            if (!Directory.Exists(ConfigDirectoryPath))
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
            }

            var doc = new XDocument(
                new XElement("Configuration",
                    new XElement("Theme", theme)
                )
            );

            doc.Save(ConfigFilePath);
        }

        // Добавим загрузку темы
        public static string LoadTheme()
        {
            if (File.Exists(ConfigFilePath))
            {
                var doc = XDocument.Load(ConfigFilePath);
                var theme = doc.Root.Element("Theme")?.Value;
                return theme ?? "Dark"; // Если тема не задана, возвращаем "Dark" как значение по умолчанию
            }

            return "Dark"; // Если файла нет, возвращаем тему по умолчанию
        }
    }
}