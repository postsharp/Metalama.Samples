using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;

public class CacheAttribute : OverrideMethodAspect
{
    [IntroduceDependency]
    private readonly ICache _cache;

    public override dynamic? OverrideMethod()
    {
        // Builds the caching string.
        var stringBuilder = new InterpolatedStringBuilder();
        stringBuilder.AddText( meta.Target.Type.ToString() );
        stringBuilder.AddText( "." );
        stringBuilder.AddText( meta.Target.Method.Name );
        stringBuilder.AddText( "(" );

        foreach ( var p in meta.Target.Parameters )
        {
            if ( p.Index > 0 )
            {
                stringBuilder.AddText( ", " );
            }

            // We have to add the parameter type to avoid ambiguities
            // between different overloads of the same method.
            stringBuilder.AddText( "(" );
            stringBuilder.AddText( p.Type.ToString() );
            stringBuilder.AddText( ") " );

            stringBuilder.AddText( "{" );

            if ( p.Type.Is( typeof( ICacheKey ) ) || (p.Type is INamedType namedType && namedType.Enhancements().HasAspect<GenerateCacheKeyAttribute>()) )
            {
                if ( p.Type.IsNullable == false )
                {
                    stringBuilder.AddExpression( p.Value!.ToCacheKey()  );
                }
                else
                {
                    stringBuilder.AddExpression( p.Value?.ToCacheKey() ?? "null" );
                }
            }
            else
            {
                if ( p.Type.IsNullable == false )
                {
                    stringBuilder.AddExpression( p.Value );
                }
                else
                {
                    stringBuilder.AddExpression( p.Value?.ToString() ?? "null" );
                }
            }

            stringBuilder.AddText( "}" );

        }

        stringBuilder.AddText( ")" );

        var cacheKey = (string) stringBuilder.ToValue();

        // Cache lookup.
        if ( this._cache.TryGetValue( cacheKey, out var value ) )
        {
            // Cache hit.
            return value;
        }
        else
        {
            // Cache miss. Go and invoke the method.
            var result = meta.Proceed();

            // Add to cache.
            this._cache.TryAdd( cacheKey, result );

            return result;
        }
    }
    
    public override void BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        builder.MustSatisfy( m => !m.ReturnType.Is( SpecialType.Void ), m => $"{m} cannot be void");
        builder.MustSatisfy( m => !m.Parameters.Any( p => p.RefKind is RefKind.Out or RefKind.Ref), m => $"{m} cannot have out or ref parameter");
    }
}
