namespace AlienFight.Model
{
    public abstract class BaseModelLogic
    {
        protected GameModel _model;
        protected UIObjectType _currentMenu;
        public int SelectedMenuItem { get; protected set; }
        public string MenuHeader { get; protected set; }

        public dLoadAnotherModel LoadAnotherModel { get; set; }

        public BaseModelLogic(GameModel parModel)
        {
            _model = parModel;
            SelectedMenuItem = 0;
        }

        public abstract void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand);
        public abstract void HandleCommand(ModelCommand parCommand);

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
                SelectedMenuItem = newSelection;
                SelectMenuItem(SelectedMenuItem);
            }
        }

        protected abstract void AcceptAction();

        protected abstract void CancelAction();
    }
}
