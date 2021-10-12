using System;

internal class Program
{
    private static void Main()
    {
        try
        {
            Add(1, 1);
            Add(0, 1);
        }
        catch
        {
        }
    }

    [Log]
    private static int Add(int a, int b)
    {
        if (a == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(a));
        }

        return a + b;
    }
}