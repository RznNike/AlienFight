using System;
using System.Collections.Generic;
using System.Drawing;
using AlienFight.Model;

namespace AlienFight.View
{
    public static class ResourceLoader
    {
        public static SpritesContainer LoadSprites()
        {
            Dictionary<int, List<Image>> levelObjectSprites = LoadLevelObjectSprites();
            Dictionary<int, List<Image>> enemySprites = LoadEnemySprites();
            Dictionary<int, List<Image>> playerSprites = LoadPlayerSprites();

            return new SpritesContainer(levelObjectSprites, enemySprites, playerSprites);
        }

        private static Dictionary<int, List<Image>> LoadLevelObjectSprites()
        {
            Dictionary<int, List<Image>> sprites = new Dictionary<int, List<Image>>();

            sprites.Add((int)LevelObjectType.Stone, LoadStoneSprites());

            return sprites;
        }

        private static Dictionary<int, List<Image>> LoadEnemySprites()
        {
            Dictionary<int, List<Image>> sprites = new Dictionary<int, List<Image>>();

            return sprites;
        }

        private static Dictionary<int, List<Image>> LoadPlayerSprites()
        {
            Dictionary<int, List<Image>> sprites = new Dictionary<int, List<Image>>();

            return sprites;
        }

        private static List<Image> LoadStoneSprites()
        {
            List<Image> result = new List<Image>
            {
                Image.FromFile("resources/sprites/levels/stone/single.png"),
                Image.FromFile("resources/sprites/levels/stone/left.png"),
                Image.FromFile("resources/sprites/levels/stone/mid.png"),
                Image.FromFile("resources/sprites/levels/stone/right.png"),
                Image.FromFile("resources/sprites/levels/stone/center.png"),
                Image.FromFile("resources/sprites/levels/stone/center_round.png")
            };

            return result;
        }
    }
}
