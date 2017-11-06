using System.Collections.Generic;

namespace AlienFight.Model
{
    public class LevelLoader
    {
        public static GameLevel Load(int parLevelID)
        {
            GameLevel level = new GameLevel();
            level.LevelID = parLevelID;
            List<LevelObject> levelObjects = new List<LevelObject>();
            LevelObject stone1 = new LevelObject
            {
                X = 0,
                Y = 0,
                SizeX = 70,
                SizeY = 70,
                FlippedY = false,
                Type = LevelObjectType.Stone,
                State = 5
            };
            levelObjects.Add(stone1);
            LevelObject stone = new LevelObject
            {
                X = 70,
                Y = 20,
                SizeX = 50,
                SizeY = 50,
                FlippedY = false,
                Type = LevelObjectType.Stone,
                State = 0
            };
            levelObjects.Add(stone);

            level.LevelObjects = levelObjects;

            List<EnemyObject> enemies = new List<EnemyObject>();

            level.Enemies = enemies;

            level.Player = new PlayerObject();
            level.SizeX = 500;
            level.SizeY = 500;
            level.CameraX = 0;
            level.CameraY = 0;

            return level;
        }
    }
}
