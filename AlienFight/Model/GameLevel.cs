using System.Collections.Generic;

namespace AlienFight.Model
{
    public class GameLevel
    {
        public int LevelID { get; set; }
        public List<LevelObject> LevelObjects { get; set; }
        public List<EnemyObject> Enemies { get; set; }
        public PlayerObject Player { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public float CameraX { get; set; }
        public float CameraY { get; set; }

        public void MoveGameObject(GameObject parObject, float parDX, float parDY)
        {
            // проверка возможности сдвига объекта (полного или частичного)
            // сдвиг на возможное расстояние в заданном направлении
        }
    }
}
