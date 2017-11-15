namespace AlienFight.Model
{
    public abstract class BaseModelLogic
    {
        private GameModel _model;

        public BaseModelLogic(GameModel parModel)
        {
            _model = parModel;
        }

        public void Start()
        {
            _model.PlayerLogics.Start();
            foreach (ILogic elLogic in _model.EnemyLogics)
            {
                if (elLogic != null)
                {
                    elLogic.Start();
                }
            }
        }
    }
}
