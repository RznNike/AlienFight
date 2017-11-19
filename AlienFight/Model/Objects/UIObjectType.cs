namespace AlienExplorer.Model
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
        Restart,
        Next,
        
        OK,
        Cancel,
        
        [Custom("resources/sprites/ui/health")]
        Health,
        [Custom("resources/sprites/ui/timer")]
        Timer,

        Text
    }
}
