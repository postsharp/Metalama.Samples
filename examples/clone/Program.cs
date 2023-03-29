#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Metalama.Samples.Clone;

internal class Program
{
    private static void Main()
    {
        var original = new AutomaticallyCloneable
        {
            A = 1, B = new ManuallyCloneable { E = 2 }, C = new Derived { A = 3 }, D = new NotCloneable { F = 4 }
        };

        Print( original, "original" );

        var clone = original.Clone();

        Print( clone, "   clone" );

        static void Print( AutomaticallyCloneable o, string name )
        {
            Console.WriteLine( $"{name} = {{ A={o.A}, B.D={o.B.E}, C.A={o.C.A}, D.F={o.D.F} }}" );
        }
    }
}