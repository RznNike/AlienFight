namespace AlienFight.Model
{
    public class EnemyObject : GameObject
    {
        public EnemyObjectType Type { get; set; }
        public bool IsMoving { get; set; }
        public int LeftWalkingBound { get; set; }
        public int RightWalkingBound { get; set; }
    }
}
