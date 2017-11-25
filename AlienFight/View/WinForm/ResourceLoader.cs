using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System;
using System.Linq;
using System.Drawing.Text;
using AlienExplorer.Model;

namespace AlienExplorer.View
{
    /// <summary>
    /// Загрузчик ресурсов.
    /// </summary>
    public static class ResourceLoader
    {
        /// <summary>
        /// Загрузка спрайтов.
        /// </summary>
        /// <returns>Хранилище спрайтов.</returns>
        public static SpritesContainer LoadSprites()
        {
            Dictionary<int, Image> backgrounds = LoadBackgrounds("resources/sprites/levels/backgrounds");
            Dictionary<int, List<Image>> levelObjectSprites = LoadSpritesForEnum(typeof(LevelObjectType));
            Dictionary<int, List<Image>> enemySprites = LoadSpritesForEnum(typeof(EnemyObjectType));
            Dictionary<int, List<Image>> playerSprites = LoadSpritesForEnum(typeof(PlayerObjectType));
            Dictionary<int, Image> UISprites = LoadUISprites();

            return new SpritesContainer(backgrounds, levelObjectSprites, enemySprites, playerSprites, UISprites);
        }

        /// <summary>
        /// Загрузка коллекции шрифтов.
        /// </summary>
        /// <returns>Коллекция шрифтов.</returns>
        public static PrivateFontCollection LoadFontCollection()
        {
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("resources/fonts/04b_30.otf");

            return fontCollection;
        }

        /// <summary>
        /// Загрузка фонов.
        /// </summary>
        /// <param name="parPath">Путь к папке со спрайтами фонов.</param>
        /// <returns>Словарь спрайтов фонов.</returns>
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

        /// <summary>
        /// Загрузка спрайтов для перечисления.
        /// </summary>
        /// <param name="parEnumType">Тип перечисления.</param>
        /// <returns>Словарь спрайтов.</returns>
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

        /// <summary>
        /// Загрузка спрайтов из папки.
        /// </summary>
        /// <param name="parPath">Путь к папке.</param>
        /// <returns>Список спрайтов.</returns>
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

        /// <summary>
        /// Создание зеркальных копий спрайтов.
        /// </summary>
        /// <param name="parOriginalSprites">Оригинальные спрайты.</param>
        /// <returns>Список зеркальных копий спрайтов.</returns>
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

        /// <summary>
        /// Загрузка спрайтов элементов интерфейса.
        /// </summary>
        /// <returns>Словарь спрайтов элементов интерфейса.</returns>
        private static Dictionary<int, Image> LoadUISprites()
        {
            Dictionary<int, Image> sprites = new Dictionary<int, Image>();

            string path = (string)CustomAttribute.GetValue(typeof(UIObjectType), UIObjectType.Health.ToString());
            Image sprite = LoadSpritesFromFolder(path)[0];
            sprites.Add((int)UIObjectType.Health, sprite);

            path = (string)CustomAttribute.GetValue(typeof(UIObjectType), UIObjectType.Timer.ToString());
            sprite = LoadSpritesFromFolder(path)[0];
            sprites.Add((int)UIObjectType.Timer, sprite);

            return sprites;
        }
    }
}
