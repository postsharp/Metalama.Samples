using System;
using System.Text;

class Program
{
    static void Main()
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
    static int Add(int a, int b)
    {
        if ( a == 0 ) throw new ArgumentOutOfRangeException(nameof(a));
        return a + b;
    }
}