namespace AlienExplorer.Model
{
    public abstract class BaseModelLogic
    {
        protected ModelStateMachine _stateMachine;
        public int SelectedMenuItem { get; protected set; }
        public string MenuHeader { get; protected set; }
        public bool ShadowLevel { get; protected set; }

        public dLoadAnotherModel LoadAnotherModel { get; set; }

        public BaseModelLogic(GameModel parModel)
        {
        }

        public abstract void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand);
        protected abstract void HandleCommand(ModelCommand parCommand);
    }
}
