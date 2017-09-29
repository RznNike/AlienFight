using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlienFight.Model;

namespace AlienFight.View
{
    public interface IViewable
    {
        void ViewLevel(GameLevel parLevel);
    }
}
