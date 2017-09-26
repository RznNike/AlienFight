using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienFight.Model;
using System.Drawing;

namespace AlienFight.Controller
{
    public class LevelLoader
    {
        public static GameLevel Load(int parLevelID)
        {
            GameLevel level = new GameLevel();
            level.LevelID = parLevelID;
            List<GameObject> levelObjects = new List<GameObject>();
            GameObject grass = new GameObject();
            grass.X = 10;
            grass.Y = 50;
            grass.SizeX = 50;
            grass.SizeY = 50;
            grass.Flipped = false;
            grass.Sprites = new Image[1];
            grass.Sprites[0] = Image.FromFile("grass.png");
            grass.ActiveSprite = 0;
            levelObjects.Add(grass);

            level.LevelObjects = levelObjects;

            List<EnemyObject> enemies = new List<EnemyObject>();

            level.Enemies = enemies;

            level.Player = new GameObject();
            level.SizeX = 500;
            level.SizeY = 500;
            level.CameraX = 0;
            level.CameraY = 0;

            return level;
        }
    }
}
