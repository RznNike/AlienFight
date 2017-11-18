namespace AlienExplorer.Model
{
    public abstract class ModelStateMachine
    {
        protected UIObjectType _machineState;
        protected GameModel _model;
        protected UIObjectType _currentMenu;
        public ModelStateMachineCommand CurrentCommand { get; protected set; }
        public int SelectedMenuItem { get; protected set; }
        public string MenuHeader { get; protected set; }

        public ModelStateMachine(GameModel parModel, int parSelectedMenuItem = 1)
        {
            _model = parModel;
            CurrentCommand = ModelStateMachineCommand.None;
            SelectedMenuItem = parSelectedMenuItem;
        }

        public abstract void ChangeState(ModelCommand parCommand);

        protected void SelectMenuItem(int parItemNumber)
        {
            foreach (UIObject elItem in _model.UIItems)
            {
                elItem.State = 0;
            }
            _model.UIItems[parItemNumber].State = 1;
        }

        protected void SelectPrevMenuItem()
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                int newSelection = SelectedMenuItem - 1;
                if (newSelection < 0)
                {
                    newSelection = _model.UIItems.Count - 1;
                }
                if ((_model.UIItems[newSelection].Type == UIObjectType.Text)
                    && (_model.UIItems[newSelection].ID == -1))
                {
                    newSelection--;
                    if (newSelection < 0)
                    {
                        newSelection = _model.UIItems.Count - 1;
                    }
                }
                SelectedMenuItem = newSelection;
                SelectMenuItem(SelectedMenuItem);
            }
        }

        protected void SelectNextMenuItem()
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                int newSelection = SelectedMenuItem + 1;
                if (newSelection >= _model.UIItems.Count)
                {
                    newSelection = 0;
                }
                if ((_model.UIItems[newSelection].Type == UIObjectType.Text)
                    && (_model.UIItems[newSelection].ID == -1))
                {
                    newSelection++;
                }
                SelectedMenuItem = newSelection;
                SelectMenuItem(SelectedMenuItem);
            }
        }

        protected abstract void AcceptAction();

        protected abstract void CancelAction();
    }
}
