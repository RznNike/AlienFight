namespace AlienExplorer.Model
{
    /// <summary>
    /// Объект игрока.
    /// </summary>
    public class PlayerObject : GameObject
    {
        /// <summary>
        /// Высота стандартная.
        /// </summary>
        public float SizeYstandart { get; set; }
        /// <summary>
        /// Высота в приседе.
        /// </summary>
        public float SizeYsmall { get; set; }
        /// <summary>
        /// Тип игрока.
        /// </summary>
        public PlayerObjectType Type { get; set; }
        /// <summary>
        /// Очки жизни.
        /// </summary>
        public int Health { get; set; }
        /// <summary>
        /// Максимальный уровень очков жизни.
        /// </summary>
        public int HealthMax { get; set; }
        /// <summary>
        /// Очки энергии.
        /// </summary>
        public double Energy { get; set; }
        /// <summary>
        /// Максимальный уровень очков энергии.
        /// </summary>
        public double EnergyMax { get; set; }
    }
}
