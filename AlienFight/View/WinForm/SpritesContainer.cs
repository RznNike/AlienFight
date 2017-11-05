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

        public SpritesContainer(
            Dictionary<int, List<Image>> parLevelObjectSprites,
            Dictionary<int, List<Image>> parEnemySprites,
            Dictionary<int, List<Image>> parPlayerSprites)
        {
            _levelObjectSprites = parLevelObjectSprites;
            _enemySprites = parEnemySprites;
            _playerSprites = parPlayerSprites;
        }

        public Image GetSprite(GameObject parObject)
        {
            if (typeof(LevelObject).IsInstanceOfType(parObject))
            {
                return _levelObjectSprites[(int)((LevelObject)parObject).Type][parObject.State];
            }
            else if (typeof(EnemyObject).IsInstanceOfType(parObject))
            {
                return _enemySprites[(int)((EnemyObject)parObject).Type][parObject.State];
            }
            else
            {
                return _playerSprites[(int)((PlayerObject)parObject).Type][parObject.State];
            }
        }
    }
}
