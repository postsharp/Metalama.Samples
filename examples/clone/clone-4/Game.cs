[Cloneable]
internal class Game
{
    public List<Player> Players { get; private set; } = new();

    public GameSettings Settings { get; private set; }
}