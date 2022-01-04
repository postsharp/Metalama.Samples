// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using System;

namespace Metalama.Samples.NotifyPropertyChanged
{
    [NotifyPropertyChanged]
    internal partial class MovingVertex
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double DX { get; set; }

        public double DY { get; set; }

        public double Velocity => Math.Sqrt( (this.DX * this.DX) + (this.DY * this.DY) );

        public void ApplyTime( double time )
        {
            this.X += this.DX * time;
            this.Y += this.DY * time;
        }
    }

    internal class Program
    {
        private static void Main()
        {
            var car = new MovingVertex { X = 5, Y = 3, DX = 0.1, DY = 0.3 };
            car.PropertyChanged += ( _, args ) => Console.WriteLine( $"{args.PropertyName} has changed" );

            car.ApplyTime( 1.2 );
        }
    }
}