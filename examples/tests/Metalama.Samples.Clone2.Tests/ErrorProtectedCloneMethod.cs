namespace ErrorProtectedCloneMethod;

[Cloneable]
internal class Game
{
    [Child]
    public Player Player { get; private set; }
}

internal class Player 
{
    public string Name { get; init; }

    protected Player Clone()
        => new Player() { Name = this.Name };
}