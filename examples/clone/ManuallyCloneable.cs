#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Metalama.Samples.Clone;

internal class ManuallyCloneable : ICloneable
{
    public int E;

    public object Clone() => new ManuallyCloneable() { E = this.E };
}