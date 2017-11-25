namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип команды от контроллера к модели.
    /// </summary>
    public enum ModelCommand
    {
        /// <summary>
        /// Влево.
        /// </summary>
        Left,
        /// <summary>
        /// Вправо.
        /// </summary>
        Right,
        /// <summary>
        /// Вверх.
        /// </summary>
        Up,
        /// <summary>
        /// Вниз.
        /// </summary>
        Down,
        /// <summary>
        /// Подтверждение.
        /// </summary>
        OK,
        /// <summary>
        /// Отмена.
        /// </summary>
        Escape
    }
}
