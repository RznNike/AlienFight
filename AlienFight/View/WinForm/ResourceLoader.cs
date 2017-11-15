using System.Collections.Generic;
using System.Drawing;
using AlienFight.Model;
using System.IO;
using System;
using System.Linq;
using System.Drawing.Text;

namespace AlienFight.View
{
    public static class ResourceLoader
    {
        public static SpritesContainer LoadSprites()
        {
            Dictionary<int, Image> backgrounds = LoadBackgrounds("resources/sprites/levels/backgrounds");
            Dictionary<int, List<Image>> levelObjectSprites = LoadSpritesForEnum(typeof(LevelObjectType));
            Dictionary<int, List<Image>> enemySprites = LoadSpritesForEnum(typeof(EnemyObjectType));
            Dictionary<int, List<Image>> playerSprites = LoadSpritesForEnum(typeof(PlayerObjectType));
            Dictionary<int, List<Image>> UISprites = LoadUISprites();

            return new SpritesContainer(backgrounds, levelObjectSprites, enemySprites, playerSprites, UISprites);
        }

        public static PrivateFontCollection LoadFontCollection()
        {
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("resources/fonts/04b_30.otf");

            return fontCollection;
        }

        private static Dictionary<int, Image> LoadBackgrounds(string parPath)
        {
            Dictionary<int, Image> result = new Dictionary<int, Image>();
            List<string> files = new List<string>(Directory.EnumerateFiles(parPath));
            files.Sort();
            for (int i = 0; i < files.Count; i++)
            {
                result.Add(i, Image.FromFile(files[i]));
            }

            return result;
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

        private static Dictionary<int, List<Image>> LoadUISprites()
        {
            Dictionary<int, List<Image>> sprites = new Dictionary<int, List<Image>>();

            string path = (string)CustomAttribute.GetValue(typeof(UIObjectType), UIObjectType.Health.ToString());
            List<Image> list = LoadSpritesFromFolder(path);
            sprites.Add(1, list);

            path = (string)CustomAttribute.GetValue(typeof(UIObjectType), UIObjectType.Timer.ToString());
            list = LoadSpritesFromFolder(path);
            sprites.Add(2, list);

            return sprites;
        }
    }
}
