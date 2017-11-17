using System.Collections.Generic;

namespace AlienExplorer.Model
{
    public class MenuLogic : BaseModelLogic
    {
        public dCloseApplication CloseApplication { get; set; }

        public MenuLogic(GameModel parModel) : base(parModel)
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

        public override void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                HandleCommand(parCommand);
            }
        }

        public override void HandleCommand(ModelCommand parCommand)
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

        protected override void AcceptAction()
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                switch (_model.UIItems[SelectedMenuItem].Type)
                {
                    case UIObjectType.New_game:
                        LoadAnotherModel?.Invoke(GameModelType.Level);
                        break;
                    case UIObjectType.Exit:
                        CloseApplication?.Invoke();
                        break;
                }
            }
        }

        protected override void CancelAction()
        {
        }
    }
}
