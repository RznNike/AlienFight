using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Загрузчик уровней.
    /// </summary>
    public class LevelLoader
    {
        /// <summary>
        /// Относительный путь к папке, содержащей файлы уровней.
        /// </summary>
        public static readonly string LEVELS_FOLDER = "resources/levels";

        /// <summary>
        /// Загрузка указанного уровня.
        /// </summary>
        /// <param name="parLevelID">ID уровня</param>
        /// <returns>Уровень.</returns>
        public static GameModel Load(int parLevelID)
        {
            string path = $"{LEVELS_FOLDER}/level{parLevelID}.xml";
            GameModel level = new GameModel
            {
                LevelID = parLevelID,
                Type = GameModelType.Level
            };
            Stream fileStream = File.OpenRead(path);
            XmlReader xmlReader = XmlReader.Create(fileStream);

            ParseLevelProperties(level, xmlReader);
            level.ModelObjects = ParseLevelObjects(xmlReader);
            level.Enemies = ParseEnemies(xmlReader);
            level.Doors = ParseDoors(xmlReader);
            level.Player = ParsePlayer(xmlReader);
            level.PlayerLogics = new PlayerLogic(level);
            level.EnemyLogics = new List<ILogic>();
            foreach (EnemyObject elEnemy in level.Enemies)
            {
                if (elEnemy.Type != EnemyObjectType.Spikes)
                {
                    level.EnemyLogics.Add(EnemyLogicFactory.CreateLogic(level, elEnemy));
                }
            }
            level.UIItems = new List<UIObject>();
            level.ModelLogic = new LevelLogic(level);

            fileStream.Close();
            xmlReader.Close();

            return level;
        }

        /// <summary>
        /// Анализ папки с уровнями и выдача списка ID имеющихся уровней.
        /// </summary>
        /// <returns>Список ID уровней.</returns>
        public static List<int> CheckAvailableLevels()
        {
            List<int> result = new List<int>();
            string[ ] fileNames = Directory.GetFiles(LEVELS_FOLDER);
            if (fileNames.Length == 0)
            {
                throw new FileNotFoundException();
            }
            foreach (string elFileName in fileNames)
            {
                List<string> numbersInFileName = Regex.Split(elFileName, "[^0-9]+").Where(x => !x.Equals("")).ToList();
                if (numbersInFileName.Count > 0)
                {
                    int id = int.Parse(numbersInFileName.Last());
                    result.Add(id);
                }
            }

            return result;
        }

        /// <summary>
        /// Считывание из XML настроек уровня.
        /// </summary>
        /// <param name="parLevel">Уровень для сохранения данных.</param>
        /// <param name="parXmlReader">Парсер XML в нужном состоянии.</param>
        private static void ParseLevelProperties(GameModel parLevel, XmlReader parXmlReader)
        {
            parXmlReader.ReadToFollowing("settings");
            parXmlReader.ReadToDescendant("sizeX");
            parLevel.SizeX = parXmlReader.ReadElementContentAsInt();
            parXmlReader.ReadToNextSibling("sizeY");
            parLevel.SizeY = parXmlReader.ReadElementContentAsInt();
            parXmlReader.ReadToNextSibling("cameraX");
            parLevel.CameraX = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("cameraY");
            parLevel.CameraY = parXmlReader.ReadElementContentAsFloat();
        }

        /// <summary>
        /// Считывание из XML элементов уровня.
        /// </summary>
        /// <param name="parXmlReader">Парсер XML в нужном состоянии.</param>
        /// <returns>Список созданных элементов уровня.</returns>
        private static List<LevelObject> ParseLevelObjects(XmlReader parXmlReader)
        {
            List<LevelObject> levelObjects = new List<LevelObject>();
            parXmlReader.ReadToFollowing("levelObjects");
            parXmlReader.Read();
            while (parXmlReader.ReadToNextSibling("object"))
            {
                LevelObject levelObject = new LevelObject();
                parXmlReader.ReadToDescendant("type");
                levelObject.Type = (LevelObjectType)Enum.Parse(typeof(LevelObjectType), parXmlReader.ReadElementContentAsString());
                parXmlReader.ReadToNextSibling("x");
                levelObject.X = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("y");
                levelObject.Y = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("sizeX");
                levelObject.SizeX = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("sizeY");
                levelObject.SizeY = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("state");
                levelObject.State = parXmlReader.ReadElementContentAsInt();
                parXmlReader.ReadToNextSibling("flippedY");
                levelObject.FlippedY = parXmlReader.ReadElementContentAsBoolean();
                parXmlReader.ReadEndElement();

                levelObjects.Add(levelObject);
            }

            return levelObjects;
        }

        /// <summary>
        /// Считывание из XML целей уровня.
        /// </summary>
        /// <param name="parXmlReader">Парсер XML в нужном состоянии.</param>
        /// <returns>Список созданных целей уровня.</returns>
        private static List<LevelObject> ParseDoors(XmlReader parXmlReader)
        {
            List<LevelObject> doors = new List<LevelObject>();
            parXmlReader.ReadToFollowing("doors");
            parXmlReader.Read();
            while (parXmlReader.ReadToNextSibling("object"))
            {
                LevelObject door = new LevelObject();
                parXmlReader.ReadToDescendant("type");
                door.Type = (LevelObjectType)Enum.Parse(typeof(LevelObjectType), parXmlReader.ReadElementContentAsString());
                parXmlReader.ReadToNextSibling("x");
                door.X = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("y");
                door.Y = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("sizeX");
                door.SizeX = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("sizeY");
                door.SizeY = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("state");
                door.State = parXmlReader.ReadElementContentAsInt();
                parXmlReader.ReadToNextSibling("flippedY");
                door.FlippedY = parXmlReader.ReadElementContentAsBoolean();
                parXmlReader.ReadEndElement();

                doors.Add(door);
            }

            return doors;
        }

        /// <summary>
        /// Считывание из XML врагов.
        /// </summary>
        /// <param name="parXmlReader">Парсер XML в нужном состоянии.</param>
        /// <returns>Список созданных врагов.</returns>
        private static List<EnemyObject> ParseEnemies(XmlReader parXmlReader)
        {
            List<EnemyObject> enemies = new List<EnemyObject>();
            parXmlReader.ReadToFollowing("enemies");
            parXmlReader.Read();
            while (parXmlReader.ReadToNextSibling("enemy"))
            {
                EnemyObject enemy = new EnemyObject();
                parXmlReader.ReadToDescendant("type");
                enemy.Type = (EnemyObjectType)Enum.Parse(typeof(EnemyObjectType), parXmlReader.ReadElementContentAsString());
                parXmlReader.ReadToNextSibling("damage");
                enemy.Damage = parXmlReader.ReadElementContentAsInt();
                parXmlReader.ReadToNextSibling("x");
                enemy.X = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("y");
                enemy.Y = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("sizeX");
                enemy.SizeX = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("sizeY");
                enemy.SizeY = parXmlReader.ReadElementContentAsFloat();
                parXmlReader.ReadToNextSibling("state");
                enemy.State = parXmlReader.ReadElementContentAsInt();
                parXmlReader.ReadToNextSibling("flippedY");
                enemy.FlippedY = parXmlReader.ReadElementContentAsBoolean();
                parXmlReader.ReadToNextSibling("isMoving");
                enemy.IsMoving = parXmlReader.ReadElementContentAsBoolean();
                if (parXmlReader.ReadToNextSibling("leftWalkingBoundX"))
                {
                    enemy.LeftWalkingBoundX = parXmlReader.ReadElementContentAsFloat();
                    parXmlReader.ReadToNextSibling("leftWalkingBoundY");
                    enemy.LeftWalkingBoundY = parXmlReader.ReadElementContentAsFloat();
                    parXmlReader.ReadToNextSibling("rightWalkingBoundX");
                    enemy.RightWalkingBoundX = parXmlReader.ReadElementContentAsFloat();
                    parXmlReader.ReadToNextSibling("rightWalkingBoundY");
                    enemy.RightWalkingBoundY = parXmlReader.ReadElementContentAsFloat();
                    parXmlReader.ReadEndElement();
                }
                else
                {
                    enemy.LeftWalkingBoundX = 0;
                    enemy.LeftWalkingBoundY = 0;
                    enemy.RightWalkingBoundX = 0;
                    enemy.RightWalkingBoundY = 0;
                    parXmlReader.ReadEndElement();
                }

                enemies.Add(enemy);
            }

            return enemies;
        }

        /// <summary>
        /// Считывание из XML игрока.
        /// </summary>
        /// <param name="parXmlReader">Парсер XML в нужном состоянии.</param>
        /// <returns>Созданный объект игрока.</returns>
        private static PlayerObject ParsePlayer(XmlReader parXmlReader)
        {
            PlayerObject player = new PlayerObject();
            parXmlReader.ReadToFollowing("player");

            parXmlReader.ReadToDescendant("type");
            player.Type = (PlayerObjectType)Enum.Parse(typeof(PlayerObjectType), parXmlReader.ReadElementContentAsString());
            parXmlReader.ReadToNextSibling("x");
            player.X = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("y");
            player.Y = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("sizeX");
            player.SizeX = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("sizeY");
            player.SizeY = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("sizeYstandart");
            player.SizeYstandart = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("sizeYsmall");
            player.SizeYsmall = parXmlReader.ReadElementContentAsFloat();
            parXmlReader.ReadToNextSibling("state");
            player.State = parXmlReader.ReadElementContentAsInt();
            parXmlReader.ReadToNextSibling("flippedY");
            player.FlippedY = parXmlReader.ReadElementContentAsBoolean();
            parXmlReader.ReadToNextSibling("health");
            player.Health = parXmlReader.ReadElementContentAsInt();
            parXmlReader.ReadToNextSibling("healthMax");
            player.HealthMax = parXmlReader.ReadElementContentAsInt();
            parXmlReader.ReadToNextSibling("energyMax");
            player.EnergyMax = parXmlReader.ReadElementContentAsInt();

            return player;
        }
    }
}
