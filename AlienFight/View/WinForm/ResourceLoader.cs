using System.Collections.Generic;
using System.Drawing;
using AlienFight.Model;
using System.IO;
using System;
using System.Linq;

namespace AlienFight.View
{
    public static class ResourceLoader
    {
        public static SpritesContainer LoadSprites()
        {
            Dictionary<int, List<Image>> levelObjectSprites = LoadSpritesForEnum(typeof(LevelObjectType));
            Dictionary<int, List<Image>> enemySprites = LoadSpritesForEnum(typeof(EnemyObjectType));
            Dictionary<int, List<Image>> playerSprites = LoadSpritesForEnum(typeof(PlayerObjectType));
            Image background = Image.FromFile("resources/sprites/levels/background.png");

            return new SpritesContainer(levelObjectSprites, enemySprites, playerSprites, background);
        }

        private static Dictionary<int, List<Image>> LoadSpritesForEnum(Type parEnumType)
        {
            Dictionary<int, List<Image>> sprites = new Dictionary<int, List<Image>>();

            int min = Enum.GetValues(parEnumType).Cast<int>().Min();
            int max = Enum.GetValues(parEnumType).Cast<int>().Max();
            for (int i = min; i <= max; i++)
            {
                string fieldName = parEnumType.GetFields()[i].Name;
                string path = (string)CustomAttribute.GetValue(parEnumType, fieldName);
                List<Image> list = LoadSpritesFromFolder(path);
                sprites.Add(i, list);
                sprites.Add(-i, FlipSprites(list));
            }

            return sprites;
        }

        private static List<Image> LoadSpritesFromFolder(string parPath)
        {
            List<Image> result = new List<Image>();
            List<string> files = new List<string>(Directory.EnumerateFiles(parPath));
            files.Sort();
            foreach (string file in files)
            {
                result.Add(Image.FromFile(file));
            }

            return result;
        }

        private static List<Image> FlipSprites(List<Image> parOriginalSprites)
        {
            List<Image> sprites = new List<Image>();
            foreach (Image elSprite in parOriginalSprites)
            {
                Image flippedSprite = (Image)elSprite.Clone();
                flippedSprite.RotateFlip(RotateFlipType.RotateNoneFlipX);
                sprites.Add(flippedSprite);
            }

            return sprites;
        }
    }
}
