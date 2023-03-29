namespace Metalama.Samples.Dirty;

[Dirty]
public class SocialAnimal : Animal
{
    public SocialAnimal( string name, int rank ) : base( name )
    {
        this.Rank = rank;
    }

    public int Rank { get; set; }
}
