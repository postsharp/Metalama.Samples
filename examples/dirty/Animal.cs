namespace Metalama.Samples.Dirty;

[Dirty]
public partial class Animal
{
    public Animal( string name )
    {
        this.Name = name;
    }

    public void Track() => this.DirtyState = DirtyState.Clean;

    public string Name { get; set; }

    public string? Color { get; set; }
}
