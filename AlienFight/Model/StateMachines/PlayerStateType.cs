namespace AlienExplorer.Model
{
    public enum PlayerStateType
    {
        [Custom(new int[ ] { 0 })]
        Stand = 1,
        [Custom(new int[ ] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        Walk,
        [Custom(new int[ ] { 10 })]
        Jump,
        [Custom(new int[ ] { 11 })]
        Duck,
        [Custom(new int[ ] { 12 })]
        Hurt
    }
}
