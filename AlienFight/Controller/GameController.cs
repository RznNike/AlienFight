using AlienFight.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlienFight.Controller
{
    public class GameController
    {
        private FormMain _view;
        private GameLevel _level;
        private SaveFile _save;
        private List<EnemyLogic> _enemyLogics;
        private PlayerLogic _playerLogics;

        public FormMain View
        {
            get
            {
                return _view;
            }

            set
            {
                _view = value;
            }
        }

        public GameLevel Level
        {
            get
            {
                return _level;
            }

            set
            {
                _level = value;
            }
        }

        public SaveFile Save
        {
            get
            {
                return _save;
            }

            set
            {
                _save = value;
            }
        }

        public List<EnemyLogic> EnemyLogics
        {
            get
            {
                return _enemyLogics;
            }

            set
            {
                _enemyLogics = value;
            }
        }

        public PlayerLogic PlayerLogics
        {
            get
            {
                return _playerLogics;
            }

            set
            {
                _playerLogics = value;
            }
        }

        public GameController()
        {
            LoadLevel(0);
            Thread framesSender = new Thread(SendDrawCommand);
            framesSender.Start();
            //Save = SaveFile.GetInstance();
            // запустить событие отрисовки по таймеру View.DrawLevel(Level);
        }

        private void SendDrawCommand()
        {
            while (true)
            {
                Thread.Sleep(10);
                if ((View != null) && (Level != null))
                {
                    View.DrawLevel(Level);
                    Level.CameraX -= 0.5f;
                    Level.CameraY -= 0.5f;
                }
            }
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
            //View.DrawLevel(Level);
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

        public void KeyDown(KeyEventArgs e)
        {
            PlayerLogics.KeyDown(e);
        }

        public void KeyUp(KeyEventArgs e)
        {
            PlayerLogics.KeyUp(e);
        }

        public void MoveGameObject(GameObject parObject, float parDX, float parDY)
        {
            // проверка возможности сдвига объекта (полного или частичного)
            // сдвиг на возможное расстояние в заданном направлении
        }
    }
}
