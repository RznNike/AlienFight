using System.Drawing;
using AlienFight.Model;
using System.Collections.Generic;

namespace AlienFight.View
{
    public class SpritesContainer
    {
        private Dictionary<int, List<Image>> _levelObjectSprites;
        private Dictionary<int, List<Image>> _enemySprites;
        private Dictionary<int, List<Image>> _playerSprites;

        public Image Background { get; private set; }

        public SpritesContainer(
            Dictionary<int, List<Image>> parLevelObjectSprites,
            Dictionary<int, List<Image>> parEnemySprites,
            Dictionary<int, List<Image>> parPlayerSprites,
            Image parBackground)
        {
            _levelObjectSprites = parLevelObjectSprites;
            _enemySprites = parEnemySprites;
            _playerSprites = parPlayerSprites;
            Background = parBackground;
        }

        public Image GetSprite(GameObject parObject, bool parFlipped)
        {
            int sign = parFlipped ? -1 : 1;
            if (typeof(LevelObject).IsInstanceOfType(parObject))
            {
                return _levelObjectSprites[(int)((LevelObject)parObject).Type * sign][parObject.State];
            }
            else if (typeof(EnemyObject).IsInstanceOfType(parObject))
            {
                return _enemySprites[(int)((EnemyObject)parObject).Type * sign][parObject.State];
            }
            else
            {
                return _playerSprites[(int)((PlayerObject)parObject).Type * sign][parObject.State];
            }
        }
    }
}
