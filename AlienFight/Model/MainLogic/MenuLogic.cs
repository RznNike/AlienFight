using System.Collections.Generic;

namespace AlienFight.Model
{
    public class MenuLogic : BaseModelLogic
    {
        public MenuLogic(GameModel parModel) : base(parModel)
        {
            _model.UIItems = new List<UIObject>();
            for (UIObjectType i = UIObjectType.New_game; i <= UIObjectType.Exit; i++)
            {
                UIObject item = new UIObject()
                {
                    X = (int)i,
                    Type = i,
                    State = 0
                };
                _model.UIItems.Add(item);
            }
            _model.UIItems[0].State = 1;
            MenuHeader = "AlienFight";
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

        }
    }
}
