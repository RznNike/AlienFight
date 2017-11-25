namespace AlienExplorer.Model
{
    /// <summary>
    /// Фабрика логик врагов.
    /// </summary>
    public static class EnemyLogicFactory
    {
        /// <summary>
        /// Создание логики для врага.
        /// </summary>
        /// <param name="parLevel">Уровень.</param>
        /// <param name="parObject">Целевой объект.</param>
        /// <returns></returns>
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
