using System;

namespace Caravela.Samples.Clone
{
    [DeepClone]
    partial class AutomaticallyCloneable
    {
        public int A;

        public ManuallyCloneable B;

        public AutomaticallyCloneable C;

        public NotCloneable D;
    }

    class ManuallyCloneable : ICloneable
    {
        public int E;

        public object Clone()
        {
            return new ManuallyCloneable() { E = this.E };
        }
    }

    class NotCloneable
    {
        public int F;
    }

    class Derived : AutomaticallyCloneable
    {
        public ManuallyCloneable G { get; private set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var original = new AutomaticallyCloneable
            {
                A = 1,
                B = new ManuallyCloneable {E = 2 },
                C = new Derived { A = 3 },
                D = new NotCloneable {  F = 4 }
            };

            Print(original, "original");

            var clone = original.Clone();

            Print(clone, "   clone");

            void Print( AutomaticallyCloneable o, string name )
            {
                Console.WriteLine($"{name} = {{ A={o.A}, B.D={o.B.E}, C.A={o.C.A}, D.F={o.D.F} }}");
            }
        }
    }
}
