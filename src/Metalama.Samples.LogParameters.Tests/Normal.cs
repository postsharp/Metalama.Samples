// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using System;

namespace Metalama.Samples.LogParameters.Tests.Normal
{
    internal class Foo
    {
        [Log]
        private void Bar( int a, string b )
        {
            Console.WriteLine( $"a={a}, b='{b}'" );
        }
    }
}