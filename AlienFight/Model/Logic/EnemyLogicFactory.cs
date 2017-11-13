namespace AlienFight.Model
{
    public static class EnemyLogicFactory
    {
        public static ILogic CreateLogic(GameLevel parLevel, EnemyObject parObject)
        {
            ILogic logic = null;
            switch (parObject.Type)
            {
                case EnemyObjectType.Slime:
                    logic = new SlimeLogic(parLevel, parObject);
                    break;
                case EnemyObjectType.Bat:
                    break;
                case EnemyObjectType.Ghost:
                    break;
            }
            return logic;
        }
    }
}
