using AlienFight.Model;
using AlienFight.View;

namespace AlienFight.Controller
{
    public abstract class GameController
    {
        protected IViewable View { get; set; }
        protected GameModel Model { get; set; }
        protected SaveFile Save { get; set; }

        public GameController()
        {
        }

        protected void LoadModel(int parModelID)
        {
            Model = LevelLoader.Load(parModelID);
            Model.ModelLogic.Start();
        }

        protected void SendViewCommand()
        {
            while (true)
            {
                if ((View != null) && (Model != null))
                {
                    View.ViewModel(Model);
                }
            }
        }
    }
}
