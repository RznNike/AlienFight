using System.Collections.Generic;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Загрузчик главного меню игры.
    /// </summary>
    public class MenuLoader
    {
        /// <summary>
        /// Загрузка меню.
        /// </summary>
        /// <returns>Меню.</returns>
        public static GameModel Load()
        {
            GameModel level = new GameModel
            {
                Type = GameModelType.Menu,
                ModelObjects = new List<LevelObject>(),
                Doors = new List<LevelObject>(),
                Enemies = new List<EnemyObject>(),
                UIItems = new List<UIObject>()
            };
            level.ModelLogic = new MenuLogic(level);

            return level;
        }
    }
}
