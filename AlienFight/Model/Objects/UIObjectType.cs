namespace AlienExplorer.Model
{
    /// <summary>
    /// Тип объекта интерфейса.
    /// </summary>
    public enum UIObjectType
    {
        /// <summary>
        /// Новая игра.
        /// </summary>
        New_game = 1,
        /// <summary>
        /// Загрузить игру.
        /// </summary>
        Load_game,
        /// <summary>
        /// Выбор уровня.
        /// </summary>
        Choose_level,
        /// <summary>
        /// Рекорды.
        /// </summary>
        Records,
        /// <summary>
        /// Выход.
        /// </summary>
        Exit,
        
        /// <summary>
        /// Продолжить.
        /// </summary>
        Resume,
        /// <summary>
        /// Вернуться в главное меню.
        /// </summary>
        Back_to_main_menu,
        /// <summary>
        /// Перезапуск уровня.
        /// </summary>
        Restart,
        /// <summary>
        /// Следующий уровень.
        /// </summary>
        Next_level,
        
        /// <summary>
        /// Подтвержение.
        /// </summary>
        OK,
        /// <summary>
        /// Отмена.
        /// </summary>
        Cancel,
        
        /// <summary>
        /// Очки жизни игрока.
        /// </summary>
        [Custom("resources/sprites/ui/health")]
        Health,
        /// <summary>
        /// Таймер прохождения уровня.
        /// </summary>
        [Custom("resources/sprites/ui/timer")]
        Timer,

        /// <summary>
        /// Текст.
        /// </summary>
        Text
    }
}
