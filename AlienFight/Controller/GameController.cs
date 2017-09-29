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
        public FormMain View { get; set; }
        public GameLevel Level { get; set; }
        public SaveFile Save { get; set; }
        public List<EnemyLogic> EnemyLogics { get; set; }
        public PlayerLogic PlayerLogics { get; set; }

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
