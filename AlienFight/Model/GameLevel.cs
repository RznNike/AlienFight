using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienFight.Model
{
    public class GameLevel
    {
        private int _levelID;
        private GameObject[ ] _levelObjects;
        private EnemyObject[ ] _enemies;
        private GameObject _player;
        private int _sizeX;
        private int _sizeY;
        private float _cameraX;
        private float _cameraY;

        public int LevelID
        {
            get
            {
                return _levelID;
            }

            set
            {
                _levelID = value;
            }
        }

        public GameObject[] LevelObjects
        {
            get
            {
                return _levelObjects;
            }

            set
            {
                _levelObjects = value;
            }
        }

        public EnemyObject[] Enemies
        {
            get
            {
                return _enemies;
            }

            set
            {
                _enemies = value;
            }
        }

        public GameObject Player
        {
            get
            {
                return _player;
            }

            set
            {
                _player = value;
            }
        }

        public int SizeX
        {
            get
            {
                return _sizeX;
            }

            set
            {
                _sizeX = value;
            }
        }

        public int SizeY
        {
            get
            {
                return _sizeY;
            }

            set
            {
                _sizeY = value;
            }
        }

        public float CameraX
        {
            get
            {
                return _cameraX;
            }

            set
            {
                _cameraX = value;
            }
        }

        public float CameraY
        {
            get
            {
                return _cameraY;
            }

            set
            {
                _cameraY = value;
            }
        }
    }
}
