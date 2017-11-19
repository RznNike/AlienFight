using System;
using System.Collections.Generic;
using System.Linq;

namespace AlienExplorer.Model
{
    public class MainMenuStateMachine : ModelStateMachine
    {
        public MainMenuStateMachine(GameModel parModel, int parSelectedMenuItem = 0) : base(parModel, parSelectedMenuItem)
        {
            InitializeMainMenu();
        }

        public override void ChangeState(ModelCommand parCommand)
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
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
                    case ModelCommand.Escape:
                        CancelAction();
                        break;
                }
            }
        }

        protected override void AcceptAction()
        {
            UIObjectType selectedItem = _model.UIItems[SelectedMenuItem].Type;
            switch (_currentMenu)
            {
                case UIObjectType.OK:
                    if (selectedItem != UIObjectType.New_game)
                    {
                        EnterToMenu(selectedItem);
                    }
                    else
                    {
                        int currentProgress = SaveFile.GetInstance().LevelToLoad;
                        int firstLevel = LevelLoader.CheckAvailableLevels().OrderBy(x => x).First();
                        if (firstLevel != currentProgress)
                        {
                            EnterToMenu(selectedItem);
                        }
                        else
                        {
                            CurrentCommand = ModelStateMachineCommand.LoadFirstLevel;
                        }
                    }
                    break;
                case UIObjectType.New_game:
                    if (_model.UIItems[SelectedMenuItem].Type == UIObjectType.OK)
                    {
                        CurrentCommand = ModelStateMachineCommand.LoadFirstLevel;
                    }
                    else
                    {
                        EnterToMenu(UIObjectType.OK);
                    }
                    break;
                case UIObjectType.Load_game:
                    if (_model.UIItems[SelectedMenuItem].Type == UIObjectType.OK)
                    {
                        CurrentCommand = ModelStateMachineCommand.LoadLastLevel;
                    }
                    else
                    {
                        EnterToMenu(UIObjectType.OK);
                    }
                    break;
                case UIObjectType.Choose_level:
                    CurrentCommand = ModelStateMachineCommand.LoadLevel;
                    SelectedMenuItem = _model.UIItems[SelectedMenuItem].ID;
                    break;
                case UIObjectType.Exit:
                    if (_model.UIItems[SelectedMenuItem].Type == UIObjectType.OK)
                    {
                        CurrentCommand = ModelStateMachineCommand.Exit;
                    }
                    else
                    {
                        EnterToMenu(UIObjectType.OK);
                    }
                    break;
            }
        }

        protected override void CancelAction()
        {
            switch (_currentMenu)
            {
                case UIObjectType.OK:
                    EnterToMenu(UIObjectType.Exit);
                    break;
                default:
                    EnterToMenu(UIObjectType.OK);
                    break;
            }
        }

        private void EnterToMenu(UIObjectType parType)
        {
            switch (parType)
            {
                case UIObjectType.OK:
                    InitializeMainMenu();
                    break;
                case UIObjectType.New_game:
                case UIObjectType.Load_game:
                case UIObjectType.Exit:
                    InitializeConfirmationMenu(parType);
                    break;
                case UIObjectType.Choose_level:
                    InitializeChooseLevelMenu();
                    break;
                case UIObjectType.Records:
                    InitializeRecordsMenu();
                    break;
            }
        }

        private void InitializeMainMenu()
        {
            _model.UIItems = new List<UIObject>();
            for (UIObjectType i = UIObjectType.New_game; i <= UIObjectType.Exit; i++)
            {
                UIObject item = new UIObject()
                {
                    Type = i,
                    State = 0
                };
                _model.UIItems.Add(item);
            }
            _model.UIItems[0].State = 1;
            SelectedMenuItem = 0;
            MenuHeader = "Alien Explorer";
            _currentMenu = UIObjectType.OK;
            CurrentCommand = ModelStateMachineCommand.None;
        }

        private void InitializeConfirmationMenu(UIObjectType parType)
        {
            string caption = "";
            _model.UIItems = new List<UIObject>();
            switch (parType)
            {
                case UIObjectType.New_game:
                    caption = "Progress will be lost. Continue?";
                    MenuHeader = "New game";
                    break;
                case UIObjectType.Load_game:
                    caption = "Load last played level?";
                    MenuHeader = "Load game";
                    break;
                case UIObjectType.Exit:
                    caption = "Close game?";
                    MenuHeader = "Exit";
                    break;
            }
            _model.UIItems.Add(new UIObject() { Type = UIObjectType.Text, State = 0, Text = caption, ID = -1 });
            for (UIObjectType i = UIObjectType.OK; i <= UIObjectType.Cancel; i++)
            {
                UIObject item = new UIObject()
                {
                    Type = i,
                    State = 0
                };
                _model.UIItems.Add(item);
            }
            _model.UIItems[2].State = 1;
            _currentMenu = parType;
            SelectedMenuItem = 2;
            CurrentCommand = ModelStateMachineCommand.None;
        }

        private void InitializeChooseLevelMenu()
        {
            int maxLevel = SaveFile.GetInstance().OpenedLevel;
            List<int> levelIDs = LevelLoader.CheckAvailableLevels().OrderBy(x => x).TakeWhile(x => x <= maxLevel).ToList();

            _model.UIItems = new List<UIObject>();
            foreach (int elLevelID in levelIDs)
            {
                UIObject item = new UIObject()
                {
                    Type = UIObjectType.Text,
                    State = 0,
                    Text = $"Level {elLevelID}",
                    ID = elLevelID
                };
                _model.UIItems.Add(item);
            }
            _model.UIItems[0].State = 1;
            SelectedMenuItem = 0;
            MenuHeader = "Choose level";
            _currentMenu = UIObjectType.Choose_level;
            CurrentCommand = ModelStateMachineCommand.None;
        }

        private void InitializeRecordsMenu()
        {
            Dictionary<int, TimeSpan> records = SaveFile.GetInstance().Records;

            _model.UIItems = new List<UIObject>();
            foreach (int elKey in records.Keys)
            {
                TimeSpan time = records[elKey];
                UIObject item = new UIObject()
                {
                    Type = UIObjectType.Text,
                    State = 0,
                    Text = $"Level {elKey}: {time.Minutes:D2}:{time.Seconds:D2}.{(time.Milliseconds / 100):D1}"
                };
                _model.UIItems.Add(item);
            }
            _model.UIItems[0].State = 1;
            SelectedMenuItem = 0;
            MenuHeader = "Records";
            _currentMenu = UIObjectType.Records;
            CurrentCommand = ModelStateMachineCommand.None;
        }
    }
}
