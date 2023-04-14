#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Metalama.Samples.Clone;

internal partial class Derived : AutomaticallyCloneable
{
    public ManuallyCloneable G { get; private set; }
}