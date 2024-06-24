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

        public static void SaveCredentials(string username, string password)
        {
            if (!Directory.Exists(ConfigDirectoryPath))
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
            }

            var doc = new XDocument(
                new XElement("Configuration",
                    new XElement("Username", username),
                    new XElement("Password", password)
                )
            );

            doc.Save(ConfigFilePath);
        }

        public static (string Username, string Password) LoadCredentials()
        {
            if (File.Exists(ConfigFilePath))
            {
                var doc = XDocument.Load(ConfigFilePath);
                var username = doc.Root.Element("Username")?.Value;
                var password = doc.Root.Element("Password")?.Value;
                return (username, password);
            }

            return (string.Empty, string.Empty);
        }
    }
}
