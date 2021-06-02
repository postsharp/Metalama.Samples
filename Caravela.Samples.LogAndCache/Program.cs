using System;
using Caravela.Framework.Aspects;

[assembly: AspectOrder(typeof(CacheAttribute), typeof(LogAttribute))]

class Program
{
    static void Main()
    {
        try
        {
            Add(1, 1);
            Add(1, 1);
        }
        catch
        {
            
        }
    }

    [Log]
    [Cache]
    static int Add(int a, int b)
    {
        Console.WriteLine("Thinking...");
        return a + b;
    }
}