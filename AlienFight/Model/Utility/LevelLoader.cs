using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AlienFight.Model
{
    public class LevelLoader
    {
        public static GameModel Load(int parLevelID)
        {
            string path = $"resources/levels/level{parLevelID}.xml";
            GameModel level = new GameModel();
            Stream fileStream = File.OpenRead(path);
            XmlReader xmlReader = XmlReader.Create(fileStream);

            ParseLevelProperties(level, xmlReader);
            level.ModelObjects = ParseLevelObjects(xmlReader);
            level.Enemies = ParseEnemies(xmlReader);
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

            fileStream.Close();
            xmlReader.Close();

            return level;
        }

        private static void ParseLevelProperties(GameModel level, XmlReader xmlReader)
        {
            xmlReader.ReadToFollowing("settings");
            xmlReader.ReadToDescendant("levelID");
            level.ModelID = xmlReader.ReadElementContentAsInt();
            xmlReader.ReadToNextSibling("sizeX");
            level.SizeX = xmlReader.ReadElementContentAsInt();
            xmlReader.ReadToNextSibling("sizeY");
            level.SizeY = xmlReader.ReadElementContentAsInt();
            xmlReader.ReadToNextSibling("cameraX");
            level.CameraX = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("cameraY");
            level.CameraY = xmlReader.ReadElementContentAsFloat();
        }

        private static List<LevelObject> ParseLevelObjects(XmlReader xmlReader)
        {
            List<LevelObject> levelObjects = new List<LevelObject>();
            xmlReader.ReadToFollowing("levelObjects");
            xmlReader.Read();
            while (xmlReader.ReadToNextSibling("object"))
            {
                LevelObject levelObject = new LevelObject();
                xmlReader.ReadToDescendant("type");
                levelObject.Type = (LevelObjectType)Enum.Parse(typeof(LevelObjectType), xmlReader.ReadElementContentAsString());
                xmlReader.ReadToNextSibling("x");
                levelObject.X = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("y");
                levelObject.Y = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("sizeX");
                levelObject.SizeX = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("sizeY");
                levelObject.SizeY = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("state");
                levelObject.State = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToNextSibling("flippedY");
                levelObject.FlippedY = xmlReader.ReadElementContentAsBoolean();
                xmlReader.ReadEndElement();

                levelObjects.Add(levelObject);
            }

            return levelObjects;
        }

        private static List<EnemyObject> ParseEnemies(XmlReader xmlReader)
        {
            List<EnemyObject> enemies = new List<EnemyObject>();
            xmlReader.ReadToFollowing("enemies");
            xmlReader.Read();
            while (xmlReader.ReadToNextSibling("enemy"))
            {
                EnemyObject enemy = new EnemyObject();
                xmlReader.ReadToDescendant("type");
                enemy.Type = (EnemyObjectType)Enum.Parse(typeof(EnemyObjectType), xmlReader.ReadElementContentAsString());
                xmlReader.ReadToNextSibling("x");
                enemy.X = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("y");
                enemy.Y = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("sizeX");
                enemy.SizeX = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("sizeY");
                enemy.SizeY = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("state");
                enemy.State = xmlReader.ReadElementContentAsInt();
                xmlReader.ReadToNextSibling("flippedY");
                enemy.FlippedY = xmlReader.ReadElementContentAsBoolean();
                xmlReader.ReadToNextSibling("isMoving");
                enemy.IsMoving = xmlReader.ReadElementContentAsBoolean();
                if (xmlReader.ReadToNextSibling("leftWalkingBoundX"))
                {
                    enemy.LeftWalkingBoundX = xmlReader.ReadElementContentAsFloat();
                    xmlReader.ReadToNextSibling("leftWalkingBoundY");
                    enemy.LeftWalkingBoundY = xmlReader.ReadElementContentAsFloat();
                    xmlReader.ReadToNextSibling("rightWalkingBoundX");
                    enemy.RightWalkingBoundX = xmlReader.ReadElementContentAsFloat();
                    xmlReader.ReadToNextSibling("rightWalkingBoundY");
                    enemy.RightWalkingBoundY = xmlReader.ReadElementContentAsFloat();
                    xmlReader.ReadEndElement();
                }
                else
                {
                    enemy.LeftWalkingBoundX = 0;
                    enemy.LeftWalkingBoundY = 0;
                    enemy.RightWalkingBoundX = 0;
                    enemy.RightWalkingBoundY = 0;
                    xmlReader.ReadEndElement();
                }

                enemies.Add(enemy);
            }

            return enemies;
        }

        private static PlayerObject ParsePlayer(XmlReader xmlReader)
        {
            PlayerObject player = new PlayerObject();
            xmlReader.ReadToFollowing("player");

            xmlReader.ReadToDescendant("type");
            player.Type = (PlayerObjectType)Enum.Parse(typeof(PlayerObjectType), xmlReader.ReadElementContentAsString());
            xmlReader.ReadToNextSibling("x");
            player.X = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("y");
            player.Y = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("sizeX");
            player.SizeX = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("sizeY");
            player.SizeY = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("sizeYstandart");
            player.SizeYstandart = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("sizeYsmall");
            player.SizeYsmall = xmlReader.ReadElementContentAsFloat();
            xmlReader.ReadToNextSibling("state");
            player.State = xmlReader.ReadElementContentAsInt();
            xmlReader.ReadToNextSibling("flippedY");
            player.FlippedY = xmlReader.ReadElementContentAsBoolean();
            xmlReader.ReadToNextSibling("health");
            player.Health = xmlReader.ReadElementContentAsInt();
            xmlReader.ReadToNextSibling("healthMax");
            player.HealthMax = xmlReader.ReadElementContentAsInt();
            xmlReader.ReadToNextSibling("energyMax");
            player.EnergyMax = xmlReader.ReadElementContentAsInt();

            return player;
        }
    }
}
