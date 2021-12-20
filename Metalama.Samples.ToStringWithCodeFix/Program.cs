using System;
using Metalama.Samples.ToString;

namespace Metalama.Samples.ToString
{
    internal class MovingVertex
    {
        public double X;

        public double Y;

        public double DX;

        public double DY { get; set; }
        public double Velocity => Math.Sqrt((this.DX * this.DX) + (this.DY * this.DY));


        public override string ToString()
        {
            return $"{{ MovingVertex X={X}, Y={Y}, DX={DX}, DY={DY} }}";
        }
    }

    internal class Program
    {
        private static void Main()
        {
            var car = new MovingVertex { X = 5, Y = 3, DX = 0.1, DY = 0.3 };

            Console.WriteLine($"car = {car}");
        }
    }
}