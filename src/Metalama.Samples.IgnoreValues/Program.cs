// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using System;

var author = new Author("Antoine" );
author.Name = "";
Console.WriteLine( $"Author's name: '{author.Name}." );

internal class Author
{
    [IgnoreValues("", null)]
    public string Name { get; set; }

    public Author( string name )
    {
        this.Name = name;
    }
}