namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип состояния слизня.
    /// </summary>
    public enum SlimeStateType
    {
        /// <summary>
        /// Без движения.
        /// </summary>
        [Custom(new int[ ] { 0 })]
        Stand = 1,
        /// <summary>
        /// Ходьба.
        /// </summary>
        [Custom(new int[ ] { 0, 1 })]
        Walk
    }
}
