namespace AlienExplorer.Model
{
    public class EnemyObject : GameObject
    {
        public EnemyObjectType Type { get; set; }
        public bool IsMoving { get; set; }
        public float LeftWalkingBoundX { get; set; }
        public float LeftWalkingBoundY { get; set; }
        public float RightWalkingBoundX { get; set; }
        public float RightWalkingBoundY { get; set; }
        public int Damage { get; set; }
    }
}
