namespace AlienFight.Model
{
    public abstract class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float SizeX { get; set; }
        public float SizeY { get; set; }
        public int State { get; set; }
        public bool FlippedY { get; set; }
    }
}
