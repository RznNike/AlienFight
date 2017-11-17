using System;

namespace AlienExplorer.Model
{
    public class SaveFile
    {
        private static SaveFile _instance;

        private SaveFile() { }

        public static SaveFile GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SaveFile();
            }
            return _instance;
        }
    }
}
