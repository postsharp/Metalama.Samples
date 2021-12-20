// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using System;

namespace Metalama.Samples.ToString
{
    [ToString]
    internal class MovingVertex
    {
        public double X;

        public double Y;

        public double DX;

        public double DY { get; set; }

        public double Velocity => Math.Sqrt( (this.DX * this.DX) + (this.DY * this.DY) );
    }

    internal class Program
    {
        private static void Main()
        {
            var car = new MovingVertex { X = 5, Y = 3, DX = 0.1, DY = 0.3 };

            Console.WriteLine( $"car = {car}" );
        }
    }
}