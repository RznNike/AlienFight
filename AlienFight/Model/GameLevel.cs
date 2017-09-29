using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Model
{
    public class GameLevel
    {
        public int LevelID { get; set; }
        public List<GameObject> LevelObjects { get; set; }
        public List<EnemyObject> Enemies { get; set; }
        public GameObject Player { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public float CameraX { get; set; }
        public float CameraY { get; set; }
    }
}
