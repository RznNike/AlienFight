using System;
using System.Threading;
using AlienExplorer.Model;
using AlienExplorer.View;

namespace AlienExplorer.Controller
{
    /// <summary>
    /// Абстрактный базовый класс контроллеров.
    /// </summary>
    public abstract class GameController
    {
        /// <summary>
        /// Вид
        /// </summary>
        protected IViewable View { get; set; }
        /// <summary>
        /// Модель
        /// </summary>
        protected GameModel Model { get; set; }

        /// <summary>
        /// Произвоит загрузку главного меню игры.
        /// </summary>
        public GameController()
        {
            LoadMenu();
        }

        /// <summary>
        /// Загрузка игрового уровня.
        /// </summary>
        /// <param name="parLevelID">ID уровня.</param>
        protected void LoadLevel(int parLevelID)
        {
            try
            {
                Model = LevelLoader.Load(parLevelID);
                if (View != null)
                {
                    View.SendCameraSizeDelegateSending(Model.SetCameraSize);
                    View.SendCameraSizeToModel();
                }
                ((LevelLogic)Model.ModelLogic).Start();
                Model.ModelLogic.LoadAnotherModel += LoadAnotherModel;
            }
            catch
            {
                LoadMenu();
            }
        }

        /// <summary>
        /// Загрузка главного меню игры.
        /// </summary>
        protected void LoadMenu()
        {
            Model = MenuLoader.Load();
            if (View != null)
            {
                View.SendCameraSizeDelegateSending(Model.SetCameraSize);
                View.SendCameraSizeToModel();
            }
            Model.ModelLogic.LoadAnotherModel += LoadAnotherModel;
            ((MenuLogic)Model.ModelLogic).CloseApplication += CloseApplication;
        }

        /// <summary>
        /// Команда контроллеру загрузить другую модель. Вызывается логикой старой модели через делегат.
        /// </summary>
        /// <param name="parModelType">Тип модели для загрузки.</param>
        /// <param name="parLevelID">(Необязательно) ID уровня.</param>
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

        /// <summary>
        /// Отсроченная сборка мусора.
        /// </summary>
        /// <param name="parData">Задержка перед сборкой в миллисекундах.</param>
        private void GCcollectWithDelay(object parData)
        {
            Thread.Sleep((int)parData);
            GC.Collect();
        }

        /// <summary>
        /// Бесконечный цикл показа кадров модели в виде.
        /// </summary>
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

        /// <summary>
        /// Закрытие приложения. Вызывается при закрытии вида или из логики модели через делегат.
        /// </summary>
        private void CloseApplication()
        {
            Environment.Exit(0);
        }
    }
}
