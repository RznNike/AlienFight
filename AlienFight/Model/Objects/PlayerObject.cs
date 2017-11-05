namespace AlienFight.Model
{
    public class PlayerObject : GameObject
    {
        public PlayerObjectType Type { get; set; }
        public bool IsDoubleJumpPossible { get; set; }
        public int Health { get; set; }
        public int HealthMax { get; set; }
        public int Energy { get; set; }
        public int EnergyMax { get; set; }
    }
}
