using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Model
{
    public class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public Image[] Sprites { get; set; }
        public int ActiveSprite { get; set; }
        public bool Flipped { get; set; }
    }
}
