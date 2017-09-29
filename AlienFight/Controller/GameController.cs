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
    public abstract class GameController
    {
        public IViewable View { get; set; }
        public GameLevel Level { get; set; }
        public SaveFile Save { get; set; }
        public List<EnemyLogic> EnemyLogics { get; set; }
        public PlayerLogic PlayerLogics { get; set; }

        public GameController()
        {
        }

        public void LoadLevel(int parLevelID)
        {
            Level = LevelLoader.Load(parLevelID);
            PlayerLogics = new PlayerLogic(Level);
            EnemyLogics = new List<EnemyLogic>();
            foreach (EnemyObject enemy in Level.Enemies)
            {
                EnemyLogics.Add(new EnemyLogic(Level, enemy));
            }
            // для логик игрока и врагов запустить обработку в отд. потоках
        }

        public void EndLevel(bool parWin, bool parExit)
        {
            if (parWin)
            {
                Save.UpdateLevelsList(Level.LevelID);
                if (!parExit)
                {
                    LoadLevel(Level.LevelID + 1);
                }
            }
            else if (!parExit)
            {
                LoadLevel(Level.LevelID);
            }
            if (parExit)
            {
                LoadLevel(0);
            }
        }
    }
}
