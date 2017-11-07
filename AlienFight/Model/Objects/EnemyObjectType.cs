namespace AlienFight.Model
{
    public enum EnemyObjectType
    {
        [Custom("resources/sprites/enemies/spikes")]
        Spikes = 1,
        [Custom("resources/sprites/enemies/slime")]
        Slime,
        [Custom("resources/sprites/enemies/bat")]
        Bat,
        [Custom("resources/sprites/enemies/ghost")]
        Ghost
    }
}
