namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип врага.
    /// </summary>
    public enum EnemyObjectType
    {
        /// <summary>
        /// Шипы.
        /// </summary>
        [Custom("resources/sprites/enemies/spikes")]
        Spikes = 1,
        /// <summary>
        /// Слизень.
        /// </summary>
        [Custom("resources/sprites/enemies/slime")]
        Slime,
        /// <summary>
        /// Летучая мышь.
        /// </summary>
        [Custom("resources/sprites/enemies/bat")]
        Bat,
        /// <summary>
        /// Призрак.
        /// </summary>
        [Custom("resources/sprites/enemies/ghost")]
        Ghost
    }
}
