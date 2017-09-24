using AlienFight.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Controller
{
    public class EnemyLogic
    {
        private GameLevel _level;
        private EnemyObject _object;

        public GameLevel Level { get => _level; set => _level = value; }
        public EnemyObject Object { get => _object; set => _object = value; }

        public EnemyLogic(GameLevel parLevel, EnemyObject parEnemy)
        {
            Level = parLevel;
            Object = parEnemy;
        }
    }
}
