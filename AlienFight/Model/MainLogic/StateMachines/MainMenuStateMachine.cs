using System.Collections.Generic;

namespace AlienExplorer.Model
{
    public class MainMenuStateMachine : ModelStateMachine
    {
        public MainMenuStateMachine(GameModel parModel, int parSelectedMenuItem = 1) : base(parModel, parSelectedMenuItem)
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
            MenuHeader = "Alien Explorer";
            _currentMenu = UIObjectType.OK;
        }

        public override void ChangeState(ModelCommand parCommand)
        {
        }
    }
}
