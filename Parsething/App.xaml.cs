using Parsething.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Parsething
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Регистрация конвертера программно
            var converter = new ComponentStatusToColorConverter();
            Resources.Add("ComponentStatusToColorConverter", converter);
        }
    }
}
