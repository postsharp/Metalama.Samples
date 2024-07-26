namespace Metalama.Samples.Transactional.Aspects;

public partial class TestObject : TransactionalObject
{
    private int _someField;
    private readonly int _someReadOnlyField;
    public int SomeProperty { get; set; }
    public int SomeReadOnlyProperty { get;  }
    public int SomeInitOnlyProperty { get; init; }

}