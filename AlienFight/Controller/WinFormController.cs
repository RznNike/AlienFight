using System.Threading;
using System.Windows.Forms;

using AlienExplorer.View;
using AlienExplorer.Model;

namespace AlienExplorer.Controller
{
    /// <summary>
    /// Контроллер для вида на Windows Forms.
    /// </summary>
    public class WinFormController : GameController
    {
        /// <summary>
        /// Производит загрузку главного меню игры и вида, запуск цикла отрисовки кадров.
        /// </summary>
        public WinFormController() : base()
        {
            View = new FormMain();
            View.SendCameraSizeDelegateSending(Model.SetCameraSize);
            ((Form)View).Show();
            ((Form)View).KeyDown += KeyDown;
            ((Form)View).KeyUp += KeyUp;
            Thread framesSender = new Thread(SendModelToView);
            framesSender.Start();
        }

        /// <summary>
        /// Обработчик события вида - нажатия кнопки на клавиатуре.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Аргументы события.</param>
        public void KeyDown(object sender, KeyEventArgs e)
        {
            bool beginCommand = true;
            SendCommandToPlayer(e, beginCommand);
        }

        /// <summary>
        /// Обработчик события вида - отпускания кнопки на клавиатуре.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Аргументы события.</param>
        public void KeyUp(object sender, KeyEventArgs e)
        {
            bool beginCommand = false;
            SendCommandToPlayer(e, beginCommand);
        }

        /// <summary>
        /// Универсальная обработка событий клавиш.
        /// </summary>
        /// <param name="parEventArgs">Аргументы события.</param>
        /// <param name="parBeginCommand">Флаг нажатия клавишы (если нажата, то true).</param>
        private void SendCommandToPlayer(KeyEventArgs parEventArgs, bool parBeginCommand)
        {
            if ((parEventArgs.KeyCode == Keys.Left) || (parEventArgs.KeyCode == Keys.A))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Left, parBeginCommand);
            }
            else if ((parEventArgs.KeyCode == Keys.Right) || (parEventArgs.KeyCode == Keys.D))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Right, parBeginCommand);
            }
            else if ((parEventArgs.KeyCode == Keys.Up) || (parEventArgs.KeyCode == Keys.W) || (parEventArgs.KeyCode == Keys.Space))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Up, parBeginCommand);
            }
            else if ((parEventArgs.KeyCode == Keys.Down) || (parEventArgs.KeyCode == Keys.S) || (parEventArgs.KeyCode == Keys.ControlKey))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Down, parBeginCommand);
            }
            else if (parEventArgs.KeyCode == Keys.Enter)
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.OK, parBeginCommand);
            }
            else if ((parEventArgs.KeyCode == Keys.Escape) || (parEventArgs.KeyCode == Keys.Back))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Escape, parBeginCommand);
            }
        }
    }
}
