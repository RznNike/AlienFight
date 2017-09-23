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
        private float _x;
        private float _y;
        private int _sizeX;
        private int _sizeY;
        private Image[] _sprites;
        private int _activeSprite;
        private bool _flipped;

        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }
        public int SizeX { get => _sizeX; set => _sizeX = value; }
        public int SizeY { get => _sizeY; set => _sizeY = value; }
        public Image[ ] Sprites { get => _sprites; set => _sprites = value; }
        public int ActiveSprite { get => _activeSprite; set => _activeSprite = value; }
        public bool Flipped { get => _flipped; set => _flipped = value; }
    }
}
