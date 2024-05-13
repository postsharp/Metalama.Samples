using Metalama.Framework.Code;
using Metalama.Framework.Project;

public partial class CachingOptions : ProjectExtension
{
    private readonly Dictionary<IType, IType> _externalCacheBuilderTypes = new();
    private readonly HashSet<IType> _toStringTypes = new();


    public void UseToString( Type type ) => this._toStringTypes.Add( TypeFactory.GetType( type ) );

    public void UseCacheKeyBuilder( Type type, Type builderType ) =>
        this._externalCacheBuilderTypes[TypeFactory.GetType( type )] = TypeFactory.GetType( builderType );
}