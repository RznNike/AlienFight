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
            View = new FormMain(this);
            ((Form)View).Show();
            LoadLevel(1);
            Thread framesSender = new Thread(SendViewCommand);
            framesSender.Start();
            //Save = SaveFile.GetInstance();
        }

        private void SendViewCommand()
        {
            while (true)
            {
                if ((View != null) && (Level != null))
                {
                    View.ViewLevel(Level);
                }
            }
        }

        public void KeyDown(KeyEventArgs e)
        {
            bool beginCommand = true;
            SendCommandToPlayer(e, beginCommand);
        }

        public void KeyUp(KeyEventArgs e)
        {
            bool beginCommand = false;
            SendCommandToPlayer(e, beginCommand);
        }

        private void SendCommandToPlayer(KeyEventArgs e, bool parBeginCommand)
        {
            if (e.KeyCode == Keys.Left)
            {
                Level.PlayerLogics.ReceiveCommand(PlayerCommand.Left, parBeginCommand);
            }
            else if (e.KeyCode == Keys.Right)
            {
                Level.PlayerLogics.ReceiveCommand(PlayerCommand.Right, parBeginCommand);
            }
            else if (e.KeyCode == Keys.Up)
            {
                Level.PlayerLogics.ReceiveCommand(PlayerCommand.Up, parBeginCommand);
            }
            else if (e.KeyCode == Keys.Down)
            {
                Level.PlayerLogics.ReceiveCommand(PlayerCommand.Down, parBeginCommand);
            }
        }
    }
}
