using System.Collections.Generic;

namespace AlienExplorer.Model
{
    public class MenuLoader
    {
        public static GameModel Load()
        {
            GameModel level = new GameModel();
            level.Type = GameModelType.Menu;
            level.ModelObjects = new List<LevelObject>();
            level.Enemies = new List<EnemyObject>();
            level.UIItems = new List<UIObject>();
            level.ModelLogic = new MenuLogic(level);

            return level;
        }
    }
}
