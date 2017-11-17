namespace AlienExplorer.Model
{
    public static class EnemyLogicFactory
    {
        public static ILogic CreateLogic(GameModel parLevel, EnemyObject parObject)
        {
            ILogic logic = null;
            switch (parObject.Type)
            {
                case EnemyObjectType.Slime:
                    logic = new SlimeLogic(parLevel, parObject);
                    break;
                case EnemyObjectType.Bat:
                    logic = new BatLogic(parLevel, parObject);
                    break;
                case EnemyObjectType.Ghost:
                    logic = new GhostLogic(parLevel, parObject);
                    break;
            }
            return logic;
        }
    }
}
