namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип элемента уровня.
    /// </summary>
    public enum LevelObjectType
    {
        /// <summary>
        /// Камень (платформа).
        /// </summary>
        [Custom("resources/sprites/levels/stone")]
        Stone = 1,
        /// <summary>
        /// Дверь (цель).
        /// </summary>
        [Custom("resources/sprites/levels/door")]
        Door
    }
}
