namespace AlienFight.Model
{
    public class PlayerObject : GameObject
    {
        public float SizeYstandart { get; set; }
        public float SizeYsmall { get; set; }
        public PlayerObjectType Type { get; set; }
        public int Health { get; set; }
        public int HealthMax { get; set; }
        public double Energy { get; set; }
        public double EnergyMax { get; set; }
    }
}
