[Cloneable]
internal class Game
{
    public List<Player> Players { get; private set; } = new List<Player>();

    public GameSettings Settings { get; private set; }
}
