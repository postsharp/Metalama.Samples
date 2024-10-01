using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Metalama.Samples.NormalizeStrings;

public abstract class StringContractAspect : ContractAspect
{
    public override void BuildEligibility(IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder)
        => builder.Type().MustBe(typeof(string));

    public override void BuildEligibility(IEligibilityBuilder<IParameter> builder)
        => builder.Type().MustBe(typeof(string));

    [CompileTime]
    protected static bool IsAppliedToNullableString()
    {
        var type = meta.Target.Declaration switch
        {
            IFieldOrProperty fieldOrProperty => fieldOrProperty.Type,
            IParameter parameter => parameter.Type,
            _ => throw new InvalidOperationException()
        };

        return type.IsNullable == true;
    }
}