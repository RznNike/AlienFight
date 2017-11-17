using System;
using System.Threading;
using AlienExplorer.Model;
using AlienExplorer.View;

namespace AlienExplorer.Controller
{
    public abstract class GameController
    {
        protected IViewable View { get; set; }
        protected GameModel Model { get; set; }
        protected SaveFile Save { get; set; }

        public GameController()
        {
            LoadMenu();
        }

        protected void LoadLevel(int parModelID)
        {
            Model = LevelLoader.Load(parModelID);
            ((LevelLogic)Model.ModelLogic).Start();
            Model.ModelLogic.LoadAnotherModel += LoadAnotherModel;
        }

        protected void LoadMenu()
        {
            Model = MenuLoader.Load();
            Model.ModelLogic.LoadAnotherModel += LoadAnotherModel;
            ((MenuLogic)Model.ModelLogic).CloseApplication += CloseApplication;
        }

        private void LoadAnotherModel(GameModelType parModelType, int parLevelID = 1)
        {
            if (parModelType == GameModelType.Menu)
            {
                LoadMenu();
            }
            else
            {
                LoadLevel(parLevelID);
            }

            Thread delayedGC = new Thread(GCcollectWithDelay);
            delayedGC.Start(500);
        }

        private void GCcollectWithDelay(object parData)
        {
            Thread.Sleep((int)parData);
            GC.Collect();
        }

        protected void SendModelToView()
        {
            while (true)
            {
                if ((View != null) && (Model != null))
                {
                    View.ViewModel(Model);
                }
            }
        }

        private void CloseApplication()
        {
            Environment.Exit(0);
        }
    }
}
