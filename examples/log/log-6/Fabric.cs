using Metalama.Framework.Fabrics;
using Metalama.Framework.Code;

internal class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Outbound
            .SelectMany( compilation => compilation.AllTypes )
            .Where( type => type.Accessibility == Accessibility.Public )
            .SelectMany( type => type.Methods )
            .Where( method => method.Accessibility == Accessibility.Public )
            .AddAspectIfEligible<LogAttribute>();
    }
}