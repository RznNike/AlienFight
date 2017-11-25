namespace AlienExplorer.Model
{
    /// <summary>
    /// Абстрактный базовый класс логики модели.
    /// </summary>
    public abstract class BaseModelLogic
    {
        /// <summary>
        /// Автомат состояний модели.
        /// </summary>
        protected ModelStateMachine _stateMachine;
        /// <summary>
        /// Номер выбранного пункта меню.
        /// </summary>
        public int SelectedMenuItem { get; protected set; }
        /// <summary>
        /// Заголовок меню.
        /// </summary>
        public string MenuHeader { get; protected set; }
        /// <summary>
        /// Флаг необходимости затенения слоя объектов уровня (под слоем интерфейса).
        /// </summary>
        public bool ShadowLevel { get; protected set; }

        /// <summary>
        /// Ссылка на метод загрузки другой модели.
        /// </summary>
        public dLoadAnotherModel LoadAnotherModel { get; set; }

        /// <summary>
        /// Инициализирует логику модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        public BaseModelLogic(GameModel parModel)
        {
        }

        /// <summary>
        /// Получение команды от контроллера.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        /// <param name="parBeginCommand">Флаг начала команды (true, если начата).</param>
        public abstract void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand);

        /// <summary>
        /// Обработка команды контроллера внутри логики модели.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        protected abstract void HandleCommand(ModelCommand parCommand);
    }
}
