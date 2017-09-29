using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AlienFight.Model
{
    public class LevelLoader
    {
        public static GameLevel Load(int parLevelID)
        {
            GameLevel level = new GameLevel();
            level.LevelID = parLevelID;
            List<GameObject> levelObjects = new List<GameObject>();
            GameObject grass1 = new GameObject();
            grass1.X = 0;
            grass1.Y = 0;
            grass1.SizeX = 70;
            grass1.SizeY = 70;
            grass1.Flipped = false;
            grass1.Sprites = new Image[1];
            grass1.Sprites[0] = Image.FromFile("resources/levels/grass/grass.png");
            grass1.ActiveSprite = 0;
            levelObjects.Add(grass1);
            GameObject grass = new GameObject();
            grass.X = 70;
            grass.Y = 20;
            grass.SizeX = 50;
            grass.SizeY = 50;
            grass.Flipped = false;
            grass.Sprites = new Image[1];
            grass.Sprites[0] = Image.FromFile("resources/levels/grass/grass.png");
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
