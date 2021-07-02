using System;

namespace Caravela.Samples.Dirty
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
            SocialAnimal d = new("Dog", 5);
            d.Track();

            Console.WriteLine($"State={d.DirtyState}");
            d.Name = "Changed";
            Console.WriteLine($"State={d.DirtyState}");

            Other o = null;
            Console.WriteLine($"State={o.DirtyState}");


        }
    }

    [Dirty]
    public partial class Animal
    {
        public Animal(string name)
        {
            this.Name = name;
        }

        public void Track()
        {
            this.DirtyState = DirtyState.Clean;
        }

        public string Name { get; set; }

        public string Color { get; set; }
    }

    [Dirty]
    public class SocialAnimal : Animal
    {
        public SocialAnimal(string name, int rank) : base(name)
        {
            this.Rank = rank;
        }

        public int Rank { get; set; }
    }

    [Dirty]
    public partial class Other 
    {
        
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
