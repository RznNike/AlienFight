using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlienFight.Controller
{
    public class GameController
    {
        private FormMain _view;

        public FormMain View { get => _view; set => _view = value; }

        public void SetViewForm(FormMain parView)
        {
            View = parView;
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
