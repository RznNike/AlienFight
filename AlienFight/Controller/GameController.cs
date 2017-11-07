using System.Collections.Generic;

using AlienFight.Model;
using AlienFight.View;

namespace AlienFight.Controller
{
    public abstract class GameController
    {
        public IViewable View { get; set; }
        public GameLevel Level { get; set; }
        public SaveFile Save { get; set; }

        public GameController()
        {
        }

        public void LoadLevel(int parLevelID)
        {
            Level = LevelLoader.Load(parLevelID);
            Level.PlayerLogics.Start();
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
