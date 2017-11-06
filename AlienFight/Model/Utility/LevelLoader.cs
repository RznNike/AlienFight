using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AlienFight.Model
{
    public class LevelLoader
    {
        public static GameLevel Load(int parLevelID)
        {
            string path = $"resources/levels/level{parLevelID}.xml";
            GameLevel level = new GameLevel();
            Stream fileStream = File.OpenRead(path);
            XmlReader xmlReader = XmlReader.Create(fileStream);

            ParseLevelProperties(level, xmlReader);
            level.LevelObjects = ParseLevelObjects(xmlReader);
            level.Enemies = ParseEnemies(xmlReader);
            level.Player = ParsePlayer(xmlReader);

            fileStream.Close();
            xmlReader.Close();

            return level;
        }

        private static void ParseLevelProperties(GameLevel level, XmlReader xmlReader)
        {
            xmlReader.ReadToFollowing("settings");
            xmlReader.ReadToDescendant("levelID");
            level.LevelID = xmlReader.ReadElementContentAsInt();
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
                xmlReader.ReadToNextSibling("leftWalkingBound");
                enemy.LeftWalkingBound = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadToNextSibling("rightWalkingBound");
                enemy.RightWalkingBound = xmlReader.ReadElementContentAsFloat();
                xmlReader.ReadEndElement();

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
