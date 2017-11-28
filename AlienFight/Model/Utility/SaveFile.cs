using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Класс по работе с файлом сохранения.
    /// </summary>
    [Serializable]
    public class SaveFile
    {
        /// <summary>
        /// Имя файла сохранения.
        /// </summary>
        private static readonly string SAVEFILE_NAME = "SaveFile.sf";

        /// <summary>
        /// Экземпляр объекта данного класса.
        /// </summary>
        private static SaveFile _instance;
        /// <summary>
        /// ID уровня для загрузки через меню продолжения игры.
        /// </summary>
        private int _levelToLoad;
        /// <summary>
        /// ID последнего открытого уровня.
        /// </summary>
        private int _openedLevel;
        /// <summary>
        /// Словарь: ID уровня - рекорд.
        /// </summary>
        private Dictionary<int, TimeSpan> _records;
        /// <summary>
        /// Словарь: ID уровня - MD5 сумма его файла.
        /// </summary>
        private Dictionary<int, string> _levelsMD5;

        /// <summary>
        /// ID уровня для загрузки через меню продолжения игры.
        /// </summary>
        public int LevelToLoad
        {
            get
            {
                FilesVerificationRequired?.Invoke(null, null);
                return _levelToLoad;
            }

            set
            {
                _levelToLoad = value;
                SaveInfoChanged?.Invoke(null, null);
            }
        }
        /// <summary>
        /// ID последнего открытого уровня.
        /// </summary>
        public int OpenedLevel
        {
            get
            {
                FilesVerificationRequired?.Invoke(null, null);
                return _openedLevel;
            }

            set
            {
                _openedLevel = value;
                SaveInfoChanged?.Invoke(null, null);
            }
        }
        /// <summary>
        /// Словарь: ID уровня - рекорд.
        /// </summary>
        public Dictionary<int, TimeSpan> Records
        {
            get
            {
                FilesVerificationRequired?.Invoke(null, null);
                return _records;
            }
            private set
            {
                _records = value;
            }
        }

        /// <summary>
        /// Событие возникновения необходимости проверки файлов уровней на диске.
        /// </summary>
        public event EventHandler FilesVerificationRequired;
        /// <summary>
        /// Событие изменения данных для сохранения.
        /// </summary>
        public event EventHandler SaveInfoChanged;

        /// <summary>
        /// Инициализирует новый объект для файла сохранения.
        /// </summary>
        private SaveFile()
        {
            _levelsMD5 = new Dictionary<int, string>();
            int firstLevel = LevelLoader.CheckAvailableLevels().Min();
            _levelToLoad = firstLevel;
            _openedLevel = firstLevel;
            this.FilesVerificationRequired += FilesVerificationRequiredHandler;
            this.SaveInfoChanged += SaveInfoChangedHandler;
        }

        /// <summary>
        /// Получение синглтонного экземпляра объекта для файла сохранения.
        /// </summary>
        /// <returns>Объект для файла сохранения.</returns>
        public static SaveFile GetInstance()
        {
            if (_instance == null)
            {
                _instance = LoadFile();
            }
            return _instance;
        }

        /// <summary>
        /// Проверка и установка (при необходимости) рекорда.
        /// </summary>
        /// <param name="parLevelNumber">ID уровня.</param>
        /// <param name="parRecord">Рекорд.</param>
        public void CheckAndSetRecord(int parLevelNumber, TimeSpan parRecord)
        {
            if (!_records.Keys.Contains(parLevelNumber))
            {
                _records.Add(parLevelNumber, parRecord);
                SaveInfoChanged?.Invoke(null, null);
            }
            else if (_records[parLevelNumber] > parRecord)
            {
                _records[parLevelNumber] = parRecord;
                SaveInfoChanged?.Invoke(null, null);
            }
        }

        /// <summary>
        /// Загрузка файла сохранения.
        /// </summary>
        /// <returns>Объект по работе с файлом сохранения.</returns>
        private static SaveFile LoadFile()
        {
            SaveFile result = null;
            try
            {
                Stream file = new FileStream(SAVEFILE_NAME, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                result = (SaveFile)formatter.Deserialize(file);
                file.Close();
            }
            catch
            {
                result = new SaveFile();
            }

            return result;
        }

        /// <summary>
        /// Обработчик события возникновения необходимости проверки файлов уровней на диске.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Аргументы события.</param>
        private void FilesVerificationRequiredHandler(object sender, EventArgs e)
        {
            string[ ] fileNames = Directory.GetFiles(LevelLoader.LEVELS_FOLDER);
            if (fileNames.Length == 0)
            {
                throw new FileNotFoundException();
            }
            Dictionary<int, string> newLevelsMD5 = new Dictionary<int, string>();
            foreach (string elFileName in fileNames)
            {
                List<string> numbersInFileName = Regex.Split(elFileName, "[^0-9]+").Where(x => !x.Equals("")).ToList();
                if (numbersInFileName.Count > 0)
                {
                    int id = int.Parse(numbersInFileName.Last());
                    string md5 = CalculateMD5(elFileName);
                    newLevelsMD5.Add(id, md5);
                }
            }

            Dictionary<int, TimeSpan> newRecords = new Dictionary<int, TimeSpan>();
            foreach (int elKey in newLevelsMD5.Keys)
            {
                if ((_levelsMD5.ContainsKey(elKey))
                    && (_levelsMD5[elKey].Equals(newLevelsMD5[elKey])))
                {
                    newRecords.Add(elKey, _records[elKey]);
                }
                else
                {
                    newRecords.Add(elKey, new TimeSpan(99, 59, 59));
                    if (_levelToLoad > elKey)
                    {
                        _levelToLoad = elKey;
                    }
                }
            }

            _records = newRecords;
            _levelsMD5 = newLevelsMD5;
            GC.Collect();
        }

        /// <summary>
        /// Обработчик события изменения данных для сохранения.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Аргументы события.</param>
        private void SaveInfoChangedHandler(object sender, EventArgs e)
        {
            Stream file = new FileStream(SAVEFILE_NAME, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, _instance);
            file.Close();
        }

        /// <summary>
        /// Вычисление MD5 суммы для файла.
        /// </summary>
        /// <param name="parFile">Путь к файлу.</param>
        /// <returns>Строковое представление MD5 суммы.</returns>
        private static string CalculateMD5(string parFile)
        {
            MD5 md5 = MD5.Create();
            Stream stream = File.OpenRead(parFile);
            byte[ ] hash = md5.ComputeHash(stream);
            md5.Dispose();
            stream.Close();

            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
