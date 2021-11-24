// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using System;

#pragma warning disable CS8618 // See bug #29235.

namespace Caravela.Samples.Dirty
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine( "Instantiating an object." );
            SocialAnimal d = new( "Dog", 5 );
            Console.WriteLine( $"State={d.DirtyState}" );
            Console.WriteLine( "Start tracking changes." );
            d.Track();
            Console.WriteLine( $"State={d.DirtyState}" );
            Console.WriteLine( "Doing a change." );
            d.Name = "Changed";
            Console.WriteLine( $"State={d.DirtyState}" );
        }
    }

    [Dirty]
    public partial class Animal
    {
        public Animal( string name )
        {
            this.Name = name;
        }

        public void Track()
        {
            this.DirtyState = DirtyState.Clean;
        }

        public string Name { get; set; }

        public string? Color { get; set; }
    }

    [Dirty]
    public class SocialAnimal : Animal
    {
        public SocialAnimal( string name, int rank ) : base( name )
        {
            this.Rank = rank;
        }

        public int Rank { get; set; }
    }

    public interface IDirty
    {
        DirtyState DirtyState { get; }
    }

    public enum DirtyState
    {
        NotTracking,
        Clean,
        Dirty
    }
}