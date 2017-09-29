using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlienFight.Model
{
    public class PlayerLogic
    {
        public GameLevel Level { get; set; }

        public PlayerLogic(GameLevel parLevel)
        {
            Level = parLevel;
        }

        public void KeyDown(KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void KeyUp(KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
