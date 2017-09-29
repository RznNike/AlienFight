using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Model
{
    public class EnemyLogic
    {
        public GameLevel Level { get; set; }
        public EnemyObject Object { get; set; }

        public EnemyLogic(GameLevel parLevel, EnemyObject parEnemy)
        {
            Level = parLevel;
            Object = parEnemy;
        }
    }
}
