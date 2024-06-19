using Parsething.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    internal class AutoClosingMessageBox
    {
        public static void ShowAutoClosingMessageBox(string message, string title, int timeout)
        {
            AutoCloseMessageBox autoCloseMessageBox = new AutoCloseMessageBox(message, title, timeout);
            autoCloseMessageBox.ShowDialog();
        }
    }
}
