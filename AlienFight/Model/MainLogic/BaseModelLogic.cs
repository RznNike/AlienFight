namespace AlienFight.Model
{
    public abstract class BaseModelLogic
    {
        protected GameModel _model;
        protected int _selectedMenuItem;
        protected UIObjectType _currentMenu;
        public string MenuHeader { get; protected set; }

        public dLoadAnotherModel LoadAnotherModel { get; set; }

        public BaseModelLogic(GameModel parModel)
        {
            _model = parModel;
            _selectedMenuItem = 0;
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
                int newSelection = _selectedMenuItem - 1;
                if (newSelection < 0)
                {
                    newSelection = _model.UIItems.Count - 1;
                }
                _selectedMenuItem = newSelection;
                SelectMenuItem(_selectedMenuItem);
            }
        }

        protected void SelectNextMenuItem()
        {
            if ((_model.UIItems != null) && (_model.UIItems.Count > 0))
            {
                int newSelection = _selectedMenuItem + 1;
                if (newSelection >= _model.UIItems.Count)
                {
                    newSelection = 0;
                }
                _selectedMenuItem = newSelection;
                SelectMenuItem(_selectedMenuItem);
            }
        }

        protected abstract void AcceptAction();

        protected abstract void CancelAction();
    }
}
