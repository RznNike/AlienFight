using System;
using System.Windows.Forms;

using AlienExplorer.Controller;

namespace AlienExplorer
{
    /// <summary>
    /// Инициализатор работы приложения.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GameController controller = new WinFormController();
            Application.Run();
        }
    }
}
