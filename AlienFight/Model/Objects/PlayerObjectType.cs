namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип игрока.
    /// </summary>
    public enum PlayerObjectType
    {
        /// <summary>
        /// Зеленый.
        /// </summary>
        [Custom("resources/sprites/player/green")]
        Green = 1,
        /// <summary>
        /// Розовый.
        /// </summary>
        [Custom("resources/sprites/player/pink")]
        Pink
    }
}
