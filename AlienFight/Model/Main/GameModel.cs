using System.Collections.Generic;

namespace AlienExplorer.Model
{
    public class GameModel
    {
        public int LevelID { get; set; }
        public GameModelType Type { get; set; }
        public BaseModelLogic ModelLogic { get; set; }
        public List<LevelObject> ModelObjects { get; set; }
        public List<LevelObject> Doors { get; set; }
        public List<EnemyObject> Enemies { get; set; }
        public PlayerObject Player { get; set; }
        public List<UIObject> UIItems { get; set; }
        public List<ILogic> EnemyLogics { get; set; }
        public PlayerLogic PlayerLogics { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public float CameraX { get; set; }
        public float CameraY { get; set; }
    }
}
