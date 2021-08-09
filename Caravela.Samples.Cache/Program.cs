using System;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("Calling a first time.");
        Add(1, 1);
        Console.WriteLine("Calling a second time.");
        Add(1, 1);
        Console.WriteLine("Completed.");
    }

    [Cache]
    static int Add(int a, int b)
    {
        Console.WriteLine("Thinking...");
        return a + b;
    }
}