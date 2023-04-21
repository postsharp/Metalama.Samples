[Cloneable]
internal class Game
{
    public List<Player> Players { get; private set; } = new List<Player>();

    [Child]
    public GameSettings Settings { get; private set; }

    private void CloneMembers( Game clone )
    {
        clone.Players = new List<Player>( this.Players );
    }
    
}
