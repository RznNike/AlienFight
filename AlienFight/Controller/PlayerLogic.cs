using AlienFight.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlienFight.Controller
{
    public class PlayerLogic
    {
        private GameLevel _level;

        public PlayerLogic(GameLevel parLevel)
        {
            _level = parLevel;
        }

        public void KeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void KeyUp(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
