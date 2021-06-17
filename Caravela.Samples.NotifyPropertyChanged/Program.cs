using System;

namespace Caravela.Samples.NotifyPropertyChanged
{

    [NotifyPropertyChanged]
    partial class Car
    {
        public string Make { get; set; }
        public double Power { get; set; }

    }


    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var car = new Car();
            car.PropertyChanged += (_, _) => { };
        }
    }
}
