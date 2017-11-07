namespace AlienFight.Model
{
    public class PlayerObject : GameObject
    {
        public PlayerObjectType Type { get; set; }
        public int Health { get; set; }
        public int HealthMax { get; set; }
        public double Energy { get; set; }
        public double EnergyMax { get; set; }
    }
}
