namespace Metalama.Samples.ToString;

internal class Program
{
    private static void Main()
    {
        var car = new MovingVertex { X = 5, Y = 3, DX = 0.1, DY = 0.3 };

        Console.WriteLine( $"car = {car}" );
    }
}