using System.Threading;
using System.Windows.Forms;

using AlienFight.View;

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
            // запустить событие отрисовки по таймеру View.DrawLevel(Level);
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
            PlayerLogics.Command(null);
            // test
            if (e.KeyCode == Keys.Left)
            {
                Level.CameraX -= 0.1f;
            }
            else if (e.KeyCode == Keys.Right)
            {
                Level.CameraX += 0.1f;
            }
            else if (e.KeyCode == Keys.Up)
            {
                Level.CameraY += 0.1f;
            }
            else if (e.KeyCode == Keys.Down)
            {
                Level.CameraY -= 0.1f;
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
            PlayerLogics.Command(null);
        }
    }
}
