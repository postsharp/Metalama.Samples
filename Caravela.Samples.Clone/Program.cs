// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using System;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Caravela.Samples.Clone
{
    [DeepClone]
    internal partial class AutomaticallyCloneable
    {
        public int A;

        public ManuallyCloneable B;

        public AutomaticallyCloneable C;

        public NotCloneable D;
    }

    internal class ManuallyCloneable : ICloneable
    {
        public int E;

        public object Clone()
        {
            return new ManuallyCloneable() { E = this.E };
        }
    }

    internal class NotCloneable
    {
        public int F;
    }

    internal class Derived : AutomaticallyCloneable
    {
        public ManuallyCloneable G { get; private set; }
    }

    internal class Program
    {
        private static void Main()
        {
            var original = new AutomaticallyCloneable { A = 1, B = new ManuallyCloneable { E = 2 }, C = new Derived { A = 3 }, D = new NotCloneable { F = 4 } };

            Print( original, "original" );

            var clone = original.Clone();

            Print( clone, "   clone" );

            void Print( AutomaticallyCloneable o, string name )
            {
                Console.WriteLine( $"{name} = {{ A={o.A}, B.D={o.B.E}, C.A={o.C.A}, D.F={o.D.F} }}" );
            }
        }
    }
}