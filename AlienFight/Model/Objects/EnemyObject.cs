namespace AlienExplorer.Model
{
    /// <summary>
    /// Враг.
    /// </summary>
    public class EnemyObject : GameObject
    {
        /// <summary>
        /// Тип врага.
        /// </summary>
        public EnemyObjectType Type { get; set; }
        /// <summary>
        /// Флаг наличия движения.
        /// </summary>
        public bool IsMoving { get; set; }
        /// <summary>
        /// Левая граница зоны передвижения (координата X).
        /// </summary>
        public float LeftWalkingBoundX { get; set; }
        /// <summary>
        /// Левая граница зоны передвижения (координата Y).
        /// </summary>
        public float LeftWalkingBoundY { get; set; }
        /// <summary>
        /// Правая граница зоны передвижения (координата X).
        /// </summary>
        public float RightWalkingBoundX { get; set; }
        /// <summary>
        /// Правая граница зоны передвижения (координата Y).
        /// </summary>
        public float RightWalkingBoundY { get; set; }
        /// <summary>
        /// Величина урона игроку.
        /// </summary>
        public int Damage { get; set; }
    }
}
