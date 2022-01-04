// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

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