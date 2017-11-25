namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип команды от автомата состояний модели к логике модели.
    /// </summary>
    public enum ModelStateMachineCommand
    {
        /// <summary>
        /// Нет команды.
        /// </summary>
        None,
        /// <summary>
        /// Пауза.
        /// </summary>
        Pause,
        /// <summary>
        /// Продолжить.
        /// </summary>
        Resume,
        /// <summary>
        /// Загрузить меню.
        /// </summary>
        LoadMenu,
        /// <summary>
        /// Загрузить игровой уровень.
        /// </summary>
        LoadLevel,
        /// <summary>
        /// Загрузить первый уровень.
        /// </summary>
        LoadFirstLevel,
        /// <summary>
        /// Загрузить следующий уровень.
        /// </summary>
        LoadNextLevel,
        /// <summary>
        /// Загрузить последний открытый уровень.
        /// </summary>
        LoadLastLevel,
        /// <summary>
        /// Выход.
        /// </summary>
        Exit
    }
}
