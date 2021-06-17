using System;

namespace Caravela.Samples.Dirty
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    [Dirty]
    public class Example
    {
        public int Property { get; set; }
    }

    [Dirty]
    public class Derived : Example
    {
        public string OtherProperty { get; set; }
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
