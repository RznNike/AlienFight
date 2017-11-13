using AlienFight.Model;
using AlienFight.View;

namespace AlienFight.Controller
{
    public abstract class GameController
    {
        protected IViewable View { get; set; }
        protected GameLevel Level { get; set; }
        protected SaveFile Save { get; set; }

        public GameController()
        {
        }

        protected void LoadLevel(int parLevelID)
        {
            Level = LevelLoader.Load(parLevelID);
            Level.PlayerLogics.Start();
            foreach (ILogic elLogic in Level.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Start();
                }
            }
        }

        protected void EndLevel(bool parWin, bool parExit)
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

        protected void SendViewCommand()
        {
            while (true)
            {
                if ((View != null) && (Level != null))
                {
                    View.ViewLevel(Level);
                }
            }
        }
    }
}
