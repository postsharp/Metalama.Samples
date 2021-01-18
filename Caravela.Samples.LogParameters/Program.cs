using System;

class Program
{
    static void Main()
    {
        Add(1, 1);
    }

    [Log]
    static int Add(int a, int b)
    {
        return a + b;
    }
}