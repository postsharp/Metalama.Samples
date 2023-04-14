namespace Metalama.Samples.Dirty;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine( "Instantiating an object." );
        SocialAnimal d = new("Dog", 5);
        Console.WriteLine( $"State={d.DirtyState}" );
        Console.WriteLine( "Start tracking changes." );
        d.Track();
        Console.WriteLine( $"State={d.DirtyState}" );
        Console.WriteLine( "Doing a change." );
        d.Name = "Changed";
        Console.WriteLine( $"State={d.DirtyState}" );
    }
}