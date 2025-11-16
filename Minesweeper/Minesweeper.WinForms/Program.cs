using System;
using System.Windows.Forms;

namespace Minesweeper.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormStart());
        }
    }
}
