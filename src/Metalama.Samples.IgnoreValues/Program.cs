var author = new Author("Antoine" );
author.Name = "";
Console.WriteLine( $"Author's name: '{author.Name}'." );

internal class Author
{
    [IgnoreValues("", null)]
    public string Name { get; set; }

    public Author( string name )
    {
        this.Name = name;
    }
}