namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип состояния летучей мыши.
    /// </summary>
    public enum BatStateType
    {
        /// <summary>
        /// Полет.
        /// </summary>
        [Custom(new int[ ] { 0, 1 })]
        Fly = 1
    }
}
