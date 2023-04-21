[Cloneable]
internal class Game
{
    public Player Player { get; }

    [Child]
    public GameSettings Settings { get; private set; }
    
}
