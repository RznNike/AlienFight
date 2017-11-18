using System.Linq;

namespace AlienExplorer.Model
{
    public class MenuLogic : BaseModelLogic
    {
        public dCloseApplication CloseApplication { get; set; }

        public MenuLogic(GameModel parModel) : base(parModel)
        {
            _stateMachine = new MainMenuStateMachine(parModel);
            MenuHeader = _stateMachine.MenuHeader;
        }

        public override void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                HandleCommand(parCommand);
            }
        }

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
