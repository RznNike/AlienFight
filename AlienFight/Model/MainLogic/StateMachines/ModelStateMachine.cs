namespace AlienExplorer.Model
{
    /// <summary>
    /// Абстрактный базовый класс автомата состояний модели.
    /// </summary>
    public abstract class ModelStateMachine
    {
        /// <summary>
        /// Текущее состояние автомата.
        /// </summary>
        protected UIObjectType _machineState;
        /// <summary>
        /// Модель.
        /// </summary>
        protected GameModel _model;
        /// <summary>
        /// Текущее меню.
        /// </summary>
        protected UIObjectType _currentMenu;
        /// <summary>
        /// Текущая команда логике модели.
        /// </summary>
        public ModelStateMachineCommand CurrentCommand { get; protected set; }
        /// <summary>
        /// Номер выбранного элемента меню.
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
        /// Инициализирует автомат состояний начальными значениями.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        /// <param name="parSelectedMenuItem">(Необязательно) выбранный пункт меню.</param>
        public ModelStateMachine(GameModel parModel, int parSelectedMenuItem = 1)
        {
            _model = parModel;
            CurrentCommand = ModelStateMachineCommand.None;
            SelectedMenuItem = parSelectedMenuItem;
            ShadowLevel = false;
        }

        /// <summary>
        /// Изменение состояния согласно команде извне.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        public abstract void ChangeState(ModelCommand parCommand);

        /// <summary>
        /// Выбор указанного элемента меню.
        /// </summary>
        /// <param name="parItemNumber">Номер элемента для выбора.</param>
        protected void SelectMenuItem(int parItemNumber)
        {
            foreach (UIObject elItem in _model.UIItems)
            {
                elItem.State = 0;
            }
            _model.UIItems[parItemNumber].State = 1;
        }

        /// <summary>
        /// Выбор предыдущего элемента меню.
        /// </summary>
        protected void SelectPrevMenuItem()
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                int newSelection = SelectedMenuItem - 1;
                if (newSelection < 0)
                {
                    newSelection = _model.UIItems.Count - 1;
                }
                if ((_model.UIItems[newSelection].Type == UIObjectType.Text)
                    && (_model.UIItems[newSelection].ID == -1))
                {
                    newSelection--;
                    if (newSelection < 0)
                    {
                        newSelection = _model.UIItems.Count - 1;
                    }
                }
                SelectedMenuItem = newSelection;
                SelectMenuItem(SelectedMenuItem);
            }
        }

        /// <summary>
        /// Выбор следующего элемента меню.
        /// </summary>
        protected void SelectNextMenuItem()
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                int newSelection = SelectedMenuItem + 1;
                if (newSelection >= _model.UIItems.Count)
                {
                    newSelection = 0;
                }
                if ((_model.UIItems[newSelection].Type == UIObjectType.Text)
                    && (_model.UIItems[newSelection].ID == -1))
                {
                    newSelection++;
                }
                SelectedMenuItem = newSelection;
                SelectMenuItem(SelectedMenuItem);
            }
        }

        /// <summary>
        /// Обработка действия подтверждения (обычно нажатие клавишы Enter).
        /// </summary>
        protected abstract void AcceptAction();

        /// <summary>
        /// Обработка действия отмены (обычно нажатие клавишы Escape).
        /// </summary>
        protected abstract void CancelAction();
    }
}
