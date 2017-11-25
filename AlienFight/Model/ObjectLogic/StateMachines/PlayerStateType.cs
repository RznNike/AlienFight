namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип состояния игрока.
    /// </summary>
    public enum PlayerStateType
    {
        /// <summary>
        /// Без движения.
        /// </summary>
        [Custom(new int[ ] { 0 })]
        Stand = 1,
        /// <summary>
        /// Ходьба.
        /// </summary>
        [Custom(new int[ ] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        Walk,
        /// <summary>
        /// Прыжок.
        /// </summary>
        [Custom(new int[ ] { 10 })]
        Jump,
        /// <summary>
        /// Присед.
        /// </summary>
        [Custom(new int[ ] { 11 })]
        Duck,
        /// <summary>
        /// Повреждение.
        /// </summary>
        [Custom(new int[ ] { 12 })]
        Hurt
    }
}
