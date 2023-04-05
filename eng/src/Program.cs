// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Microsoft.Build.Globbing;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Publishers;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

var product = new Product( Dependencies.MetalamaSamples )
{
    Solutions = new Solution[] { new DotNetSolution( "Metalama.Samples.sln" ) { CanFormatCode = true, BuildMethod = BuildMethod.Build } },
    Dependencies = new[] { Dependencies.PostSharpEngineering, Dependencies.MetalamaExtensions },
    Configurations = Product.DefaultConfigurations
        .WithValue(
        BuildConfiguration.Public, new BuildConfigurationInfo(
            MSBuildName: "Release",
            PublicPublishers: new Publisher[] {
                new MergePublisher()
            } )
        ),
    TestOnBuild = true
};

product.TestCompleted += OnTestCompleted ;

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );


void OnTestCompleted( BuildCompletedEventArgs args )
{
    var targetDirectory = Path.Combine( args.PrivateArtifactsDirectory, "html" );
    var sourceDirectory = Path.Combine( args.Context.RepoDirectory, "examples" );
    Directory.CreateDirectory( targetDirectory );
    
    var matcher = new Matcher();
    matcher.AddInclude("**/*.html");
    var matches = matcher.Execute(new DirectoryInfoWrapper( new DirectoryInfo( sourceDirectory ) ));

// Copy each matched file to the destination directory
    foreach (var match in matches.Files)
    {
        var sourceFile = Path.Combine( sourceDirectory, match.Path );
        var targetFile = Path.Combine( targetDirectory, match.Path );
        var targetSubdirectory = Path.GetDirectoryName( targetFile );
        Directory.CreateDirectory( targetSubdirectory );
        
        
        File.Copy(sourceFile, targetFile, true);
    }
}