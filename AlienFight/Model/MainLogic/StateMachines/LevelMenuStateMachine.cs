using System.Collections.Generic;
using System.Linq;

namespace AlienExplorer.Model
{
    public class LevelMenuStateMachine : ModelStateMachine
    {
        private bool _menuDisplayed;
        public LevelMenuStateMachine(GameModel parModel, int parSelectedMenuItem = 0) : base(parModel, parSelectedMenuItem)
        {
            InitializeLevelUI();
            CurrentCommand = ModelStateMachineCommand.None;
            _menuDisplayed = false;
        }

        public override void ChangeState(ModelCommand parCommand)
        {
            CurrentCommand = ModelStateMachineCommand.None;
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                if (_menuDisplayed)
                {
                    switch (parCommand)
                    {
                        case ModelCommand.Up:
                            SelectPrevMenuItem();
                            break;
                        case ModelCommand.Down:
                            SelectNextMenuItem();
                            break;
                        case ModelCommand.OK:
                            AcceptAction();
                            break;
                    }
                }
                if (parCommand == ModelCommand.Escape)
                {
                    CancelAction();
                }
            }
        }

        public void EnterToMenu(UIObjectType parType)
        {
            switch (parType)
            {
                case UIObjectType.OK:
                    InitializeLevelUI();
                    break;
                case UIObjectType.Resume:
                    InitializePauseMenu();
                    break;
                case UIObjectType.Next:
                    InitializeWinMenu();
                    break;
                case UIObjectType.Restart:
                    InitializeLoseMenu();
                    break;
            }
        }

        protected override void AcceptAction()
        {
            UIObjectType selectedItem = _model.UIItems[SelectedMenuItem].Type;
            switch (selectedItem)
            {
                case UIObjectType.Resume:
                    EnterToMenu(UIObjectType.OK);
                    break;
                case UIObjectType.Restart:
                    CurrentCommand = ModelStateMachineCommand.LoadLevel;
                    break;
                case UIObjectType.Next:
                    CurrentCommand = ModelStateMachineCommand.LoadNextLevel;
                    break;
                case UIObjectType.Back_to_menu:
                    CurrentCommand = ModelStateMachineCommand.LoadMenu;
                    break;
            }
        }

        protected override void CancelAction()
        {
            switch (_currentMenu)
            {
                case UIObjectType.OK:
                    EnterToMenu(UIObjectType.Resume);
                    break;
                case UIObjectType.Resume:
                    EnterToMenu(UIObjectType.OK);
                    break;
                default:
                    CurrentCommand = ModelStateMachineCommand.LoadMenu;
                    break;
            }
        }

        private void InitializeLevelUI()
        {
            _model.UIItems = new List<UIObject>();
            for (UIObjectType i = UIObjectType.Health; i <= UIObjectType.Timer; i++)
            {
                UIObject item = new UIObject()
                {
                    Type = i
                };
                _model.UIItems.Add(item);
            }
            MenuHeader = "";
            _currentMenu = UIObjectType.OK;
            CurrentCommand = ModelStateMachineCommand.Resume;
            _menuDisplayed = false;
            ShadowLevel = false;
        }

        private void InitializePauseMenu()
        {
            _model.UIItems = new List<UIObject>
            {
                new UIObject() { Type = UIObjectType.Text, State = 0, Text = "GAME PAUSED", ID = -1 },
                new UIObject() { Type = UIObjectType.Resume, State = 1 },
                new UIObject() { Type = UIObjectType.Restart, State = 0 },
                new UIObject() { Type = UIObjectType.Back_to_menu, State = 0 }
            };
            SelectedMenuItem = 1;
            MenuHeader = $"Level {_model.LevelID}";
            _currentMenu = UIObjectType.Resume;
            CurrentCommand = ModelStateMachineCommand.Pause;
            _menuDisplayed = true;
            ShadowLevel = true;
        }

        private void InitializeWinMenu()
        {
            _model.UIItems = new List<UIObject>
            {
                new UIObject() { Type = UIObjectType.Text, State = 0, Text = "YOU WIN!", ID = -1 }
            };
            int currentProgress = SaveFile.GetInstance().LevelToLoad;
            int lastLevel = LevelLoader.CheckAvailableLevels().OrderBy(x => x).Last();
            if (lastLevel != currentProgress)
            {
                _model.UIItems.Add(new UIObject() { Type = UIObjectType.Next, State = 1 });
                _model.UIItems.Add(new UIObject() { Type = UIObjectType.Restart, State = 0 });
                _model.UIItems.Add(new UIObject() { Type = UIObjectType.Back_to_menu, State = 0 });
                SelectedMenuItem = 1;
            }
            else
            {
                _model.UIItems.Add(new UIObject() { Type = UIObjectType.Restart, State = 0 });
                _model.UIItems.Add(new UIObject() { Type = UIObjectType.Back_to_menu, State = 1 });
                SelectedMenuItem = 2;
            }
            MenuHeader = $"Level {_model.LevelID}";
            _currentMenu = UIObjectType.Next;
            CurrentCommand = ModelStateMachineCommand.Pause;
            _menuDisplayed = true;
            ShadowLevel = true;
        }

        private void InitializeLoseMenu()
        {
            _model.UIItems = new List<UIObject>
            {
                new UIObject() { Type = UIObjectType.Text, State = 0, Text = "YOU LOSE...", ID = -1 },
                new UIObject() { Type = UIObjectType.Restart, State = 1 },
                new UIObject() { Type = UIObjectType.Back_to_menu, State = 0 }
            };
            SelectedMenuItem = 1;
            MenuHeader = $"Level {_model.LevelID}";
            _currentMenu = UIObjectType.Next;
            CurrentCommand = ModelStateMachineCommand.Pause;
            _menuDisplayed = true;
            ShadowLevel = true;
        }
    }
}
