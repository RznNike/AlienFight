namespace AlienExplorer.Model
{
    public abstract class ModelStateMachine
    {
        protected UIObjectType _machineState;
        protected GameModel _model;
        protected UIObjectType _currentMenu;
        public int SelectedMenuItem { get; protected set; }
        public string MenuHeader { get; protected set; }

        public ModelStateMachine(GameModel parModel, int parSelectedMenuItem = 1)
        {
            _model = parModel;
            SelectedMenuItem = parSelectedMenuItem;
        }

        public abstract void ChangeState(ModelCommand parCommand);
    }
}
