using System.Linq;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Логика меню.
    /// </summary>
    public class MenuLogic : BaseModelLogic
    {
        /// <summary>
        /// Ссылка на метод закрытия приложения.
        /// </summary>
        public dCloseApplication CloseApplication { get; set; }

        /// <summary>
        /// Инициализирует логику модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        public MenuLogic(GameModel parModel) : base(parModel)
        {
            _stateMachine = new MainMenuStateMachine(parModel);
            MenuHeader = _stateMachine.MenuHeader;
        }

        /// <summary>
        /// Получение команды от контроллера.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        /// <param name="parBeginCommand">Флаг начала команды (true, если начата).</param>
        public override void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                HandleCommand(parCommand);
            }
        }

        /// <summary>
        /// Обработка команды контроллера внутри логики модели.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        protected override void HandleCommand(ModelCommand parCommand)
        {
            _stateMachine.ChangeState(parCommand);
            SelectedMenuItem = _stateMachine.SelectedMenuItem;
            MenuHeader = _stateMachine.MenuHeader;
            switch (_stateMachine.CurrentCommand)
            {
                case ModelStateMachineCommand.LoadMenu:
                    LoadAnotherModel?.Invoke(GameModelType.Menu);
                    break;
                case ModelStateMachineCommand.LoadLevel:
                    LoadAnotherModel?.Invoke(GameModelType.Level, _stateMachine.SelectedMenuItem);
                    break;
                case ModelStateMachineCommand.LoadFirstLevel:
                    int firstLevelID = LevelLoader.CheckAvailableLevels().Min();
                    LoadAnotherModel?.Invoke(GameModelType.Level, firstLevelID);
                    break;
                case ModelStateMachineCommand.LoadLastLevel:
                    int lastLevelID = SaveFile.GetInstance().LevelToLoad;
                    LoadAnotherModel?.Invoke(GameModelType.Level, lastLevelID);
                    break;
                case ModelStateMachineCommand.Exit:
                    CloseApplication?.Invoke();
                    break;
            }
        }
    }
}
