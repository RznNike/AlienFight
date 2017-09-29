using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlienFight.Model;
using AlienFight.View;

namespace AlienFight.Controller
{
    public class WinFormController : GameController
    {
        public WinFormController()
        {
            View = new FormMain(this);
            ((Form)View).Show();
            LoadLevel(0);
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
                Level.CameraX -= 10;
            }
            else if (e.KeyCode == Keys.Right)
            {
                Level.CameraX += 10;
            }
            else if (e.KeyCode == Keys.Up)
            {
                Level.CameraY += 10;
            }
            else if (e.KeyCode == Keys.Down)
            {
                Level.CameraY -= 10;
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
            PlayerLogics.Command(null);
        }
    }
}
