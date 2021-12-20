// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using Spectre.Console.Cli;
using System.Collections.Immutable;

namespace Build
{
    internal static class Program
    {
        private static int Main( string[] args )
        {
           
            var product = new Product
            {
                ProductName = "Metalama.Samples",
                Solutions = ImmutableArray.Create<Solution>(
                    new DotNetSolution( "Metalama.Samples.sln" ) { CanFormatCode = true, CanPack = false } ),
                Dependencies = ImmutableArray.Create( new ProductDependency( "Metalama" ) )
            };

            var commandApp = new CommandApp();
            commandApp.AddProductCommands( product );

            return commandApp.Run( args );
        }
    }
}