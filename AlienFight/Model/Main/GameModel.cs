using System.Collections.Generic;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Базовый контейнер данных модели.
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// ID уровня (актуально только для игровых уровней).
        /// </summary>
        public int LevelID { get; set; }
        /// <summary>
        /// Тип модели.
        /// </summary>
        public GameModelType Type { get; set; }
        /// <summary>
        /// Логика модели.
        /// </summary>
        public BaseModelLogic ModelLogic { get; set; }
        /// <summary>
        /// Основные объекты модели (элементы уровня).
        /// </summary>
        public List<LevelObject> ModelObjects { get; set; }
        /// <summary>
        /// Начальная и конечные точки уровня.
        /// </summary>
        public List<LevelObject> Doors { get; set; }
        /// <summary>
        /// Враги.
        /// </summary>
        public List<EnemyObject> Enemies { get; set; }
        /// <summary>
        /// Игрок.
        /// </summary>
        public PlayerObject Player { get; set; }
        /// <summary>
        /// Элементы интерфейса.
        /// </summary>
        public List<UIObject> UIItems { get; set; }
        /// <summary>
        /// Логики врагов.
        /// </summary>
        public List<ILogic> EnemyLogics { get; set; }
        /// <summary>
        /// Логика игрока.
        /// </summary>
        public PlayerLogic PlayerLogics { get; set; }
        /// <summary>
        /// Размер модели по горизонтали (в клетках).
        /// </summary>
        public int SizeX { get; set; }
        /// <summary>
        /// Размер модели по вертикали (в клетках).
        /// </summary>
        public int SizeY { get; set; }
        /// <summary>
        /// Позиция камеры по горизонтали (в клетках).
        /// </summary>
        public float CameraX { get; set; }
        /// <summary>
        /// Позиция камеры по вертикали (в клетках).
        /// </summary>
        public float CameraY { get; set; }
        /// <summary>
        /// Размер камеры по горизонтали (в клетках).
        /// </summary>
        public float CameraSizeX { get; private set; }
        /// <summary>
        /// Размер камеры по вертикали (в клетках).
        /// </summary>
        public float CameraSizeY { get; private set; }

        /// <summary>
        /// Изменение размеров камеры модели. Вызывается видом через делегат.
        /// </summary>
        /// <param name="parSizeX">Размер камеры по горизонтали (в клетках).</param>
        /// <param name="parSizeY">Размер камеры по вертикали (в клетках).</param>
        public void SetCameraSize(float parSizeX, float parSizeY)
        {
            CameraSizeX = parSizeX;
            CameraSizeY = parSizeY;
        }
    }
}
