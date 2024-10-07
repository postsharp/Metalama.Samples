﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace))
            .AddSingleton<RemoteCalculator>()
            .BuildServiceProvider();

        var calculator = serviceProvider.GetService<RemoteCalculator>()!;

        try
        {
            calculator.Add(1, 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}