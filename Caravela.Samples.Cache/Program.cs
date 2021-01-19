using System;
using System.Text;

class Program
{
    static void Main()
    {
        Add(1, 1);
    }

    [Cache]
    static int Add(int a, int b)
    {
        return a + b;
    }
}