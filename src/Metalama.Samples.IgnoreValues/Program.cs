// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using System;

var author = new Author { Name = "Antoine" };
author.Name = "";
Console.WriteLine( $"Author's name: '{author.Name}." );

class Author
{
    [IgnoreValues("\"\"")]
    public string Name { get; set; }
}