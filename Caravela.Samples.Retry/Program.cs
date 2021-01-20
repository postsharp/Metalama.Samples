﻿using System;
using System.Text;

class Program
{
    static int attempts;
    static void Main()
    {
        try
        {
            Foo();
        }
        catch
        {

        }
    }

    [Retry(Attempts = 10)]
    static int Foo()
    {
        attempts++;
        Console.WriteLine($"Just trying for the {attempts}-th time.");
        throw new Exception();
    }
}