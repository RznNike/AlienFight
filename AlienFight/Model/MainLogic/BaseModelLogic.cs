namespace AlienFight.Model
{
    public abstract class BaseModelLogic
    {
        protected GameModel _model;
        public string MenuHeader { get; protected set; }

        public BaseModelLogic(GameModel parModel)
        {
            _model = parModel;
        }

        public abstract void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand);
        public abstract void HandleCommand(ModelCommand parCommand);
    }
}
