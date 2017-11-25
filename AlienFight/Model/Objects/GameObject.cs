namespace AlienExplorer.Model
{
    /// <summary>
    /// Абстрактный базовый класс объектов модели.
    /// </summary>
    public abstract class GameObject
    {
        /// <summary>
        /// Координата X.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// Координата Y.
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// Размер по горизонтали.
        /// </summary>
        public float SizeX { get; set; }
        /// <summary>
        /// Размер по вертикали.
        /// </summary>
        public float SizeY { get; set; }
        /// <summary>
        /// Состояние.
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// Флаг отражения объекта по вертикали.
        /// </summary>
        public bool FlippedY { get; set; }
    }
}
