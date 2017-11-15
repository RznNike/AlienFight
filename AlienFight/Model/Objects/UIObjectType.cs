namespace AlienFight.Model
{
    public enum UIObjectType
    {
        New_game = 1,
        Load_game,
        Choose_level,
        Records,
        Exit,
        
        Resume,
        Back_to_menu,
        Repeat,
        Next,
        
        OK,
        Cancel,
        
        [Custom("resources/sprites/ui/health")]
        Health,
        [Custom("resources/sprites/ui/timer")]
        Timer
    }
}
