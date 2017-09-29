using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Model
{
    public class EnemyObject : GameObject
    {
        public bool IsMoving { get; set; }
        public int LeftWalkingBound { get; set; }
        public int RightWalkingBound { get; set; }
    }
}
