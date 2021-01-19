using System;
using System.Text;

class Program
{
    static void Main()
    {
        try
        {
            Fibonaci(5);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine(e.Data["Context"]);
        }
    }

    [EnrichException]
    static int Fibonaci(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n));
        }
        if (n == 0)
        {
            return 0;
        }
        // Intentionally ommitting these lines to create an error.
        //else if (n == 1)
        //{
        //    return 1
        //}
        else
        {
            return Fibonaci(n - 1) + Fibonaci(n - 2);
        }
    }
}