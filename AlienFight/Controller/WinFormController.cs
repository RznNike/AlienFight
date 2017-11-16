using System.Threading;
using System.Windows.Forms;

using AlienFight.View;
using AlienFight.Model;

namespace AlienFight.Controller
{
    public class WinFormController : GameController
    {
        public WinFormController()
        {
            View = new FormMain();
            ((Form)View).Show();
            ((Form)View).KeyDown += KeyDown;
            ((Form)View).KeyUp += KeyUp;
            Thread framesSender = new Thread(SendModelToView);
            framesSender.Start();
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            bool beginCommand = true;
            SendCommandToPlayer(e, beginCommand);
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            bool beginCommand = false;
            SendCommandToPlayer(e, beginCommand);
        }

        private void SendCommandToPlayer(KeyEventArgs e, bool parBeginCommand)
        {
            if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.A))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Left, parBeginCommand);
            }
            else if ((e.KeyCode == Keys.Right) || (e.KeyCode == Keys.D))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Right, parBeginCommand);
            }
            else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.W) || (e.KeyCode == Keys.Space))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Up, parBeginCommand);
            }
            else if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.S) || (e.KeyCode == Keys.ControlKey))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Down, parBeginCommand);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.OK, parBeginCommand);
            }
            else if ((e.KeyCode == Keys.Escape) || (e.KeyCode == Keys.Back))
            {
                Model.ModelLogic.ReceiveCommand(ModelCommand.Escape, parBeginCommand);
            }
        }
    }
}
