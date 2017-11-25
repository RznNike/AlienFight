using System.Drawing;
using System.Collections.Generic;
using AlienExplorer.Model;

namespace AlienExplorer.View
{
    /// <summary>
    /// Хранилище всех спрайтов игры.
    /// </summary>
    public class SpritesContainer
    {
        /// <summary>
        /// Словарь: тип модели - часть фона для заливки.
        /// </summary>
        private Dictionary<int, Image> _backgrounds;
        /// <summary>
        /// Словарь: тип элемента уровня - список спрайтов.
        /// </summary>
        private Dictionary<int, List<Image>> _levelObjectSprites;
        /// <summary>
        /// Словарь: тип врага - список спрайтов.
        /// </summary>
        private Dictionary<int, List<Image>> _enemySprites;
        /// <summary>
        /// Словарь: тип игрока - список спрайтов.
        /// </summary>
        private Dictionary<int, List<Image>> _playerSprites;
        /// <summary>
        /// Словарь: тип элемента интерфейса - спрайт.
        /// </summary>
        private Dictionary<int, Image> _UISprites;

        /// <summary>
        /// Инициализирует хранилище спрайтов.
        /// </summary>
        /// <param name="parBackgrounds">Словарь фонов.</param>
        /// <param name="parLevelObjectSprites">Словарь спрайтов элементов уровня.</param>
        /// <param name="parEnemySprites">Словарь спрайтов врагов.</param>
        /// <param name="parPlayerSprites">Словарь спрайтов игрока.</param>
        /// <param name="parUISprites">Словарь спрайтов элементов интерфейса.</param>
        public SpritesContainer(
            Dictionary<int, Image> parBackgrounds,
            Dictionary<int, List<Image>> parLevelObjectSprites,
            Dictionary<int, List<Image>> parEnemySprites,
            Dictionary<int, List<Image>> parPlayerSprites,
            Dictionary<int, Image> parUISprites)
        {
            _backgrounds = parBackgrounds;
            _levelObjectSprites = parLevelObjectSprites;
            _enemySprites = parEnemySprites;
            _playerSprites = parPlayerSprites;
            _UISprites = parUISprites;
        }

        /// <summary>
        /// Получение спрайта для объекта.
        /// </summary>
        /// <param name="parObject">Объект.</param>
        /// <returns>Спрайт.</returns>
        public Image GetLevelSprite(GameObject parObject)
        {
            int sign = parObject.FlippedY ? -1 : 1;
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

        /// <summary>
        /// Получение спрайта для элемента интерфейса.
        /// </summary>
        /// <param name="parObject">Объект.</param>
        /// <returns>Спрайт.</returns>
        public Image GetUISprite(UIObject parObject)
        {
            return _UISprites[(int)parObject.Type];
        }

        /// <summary>
        /// Получение спрайта для фона.
        /// </summary>
        /// <param name="parType">Тип модели.</param>
        /// <returns>Спрайт.</returns>
        public Image GetBackground(GameModelType parType)
        {
            return _backgrounds[(int)parType];
        }
    }
}
