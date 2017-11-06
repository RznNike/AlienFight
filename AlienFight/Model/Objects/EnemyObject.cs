namespace AlienFight.Model
{
    public class EnemyObject : GameObject
    {
        public EnemyObjectType Type { get; set; }
        public bool IsMoving { get; set; }
        public float LeftWalkingBound { get; set; }
        public float RightWalkingBound { get; set; }
    }
}
