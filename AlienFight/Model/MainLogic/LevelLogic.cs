namespace AlienFight.Model
{
    public class LevelLogic : BaseModelLogic
    {
        public LevelLogic(GameModel parModel) : base(parModel)
        {
            MenuHeader = "";
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

        public override void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                HandleCommand(parCommand);
            }
            PlayerLogic logic = _model.PlayerLogics;
            if ((logic != null)
                && ((int)parCommand <= 3))
            {
                logic.ReceiveCommand(parCommand, parBeginCommand);
            }
        }

        public override void HandleCommand(ModelCommand parCommand)
        {

        }
    }
}
