namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип состояния призрака.
    /// </summary>
    public enum GhostStateType
    {
        /// <summary>
        /// Без движения.
        /// </summary>
        [Custom(new int[ ] { 0 })]
        Stand = 1,
        /// <summary>
        /// Атака.
        /// </summary>
        [Custom(new int[ ] { 1 })]
        Attack
    }
}
