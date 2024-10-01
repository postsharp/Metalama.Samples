using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Samples.Builder3;

[CompileTime]
internal abstract class PropertyMapping : ITemplateProvider
{
    protected PropertyMapping( IProperty sourceProperty, bool isRequired, bool isInherited )
    {
        this.SourceProperty = sourceProperty;
        this.IsRequired = isRequired;
        this.IsInherited = isInherited;
    }

    public IProperty SourceProperty { get; }

    public bool IsRequired { get; }
    public bool IsInherited { get; }

    public abstract IExpression GetBuilderPropertyValue();

    public int? SourceConstructorParameterIndex { get; set; }

    public int? BuilderConstructorParameterIndex { get; set; }

    public abstract void ImplementBuilderArtifacts( IAdviser<INamedType> builderType );

    public abstract bool TryImportBuilderArtifactsFromBaseType( INamedType baseType, ScopedDiagnosticSink diagnosticSink );

    protected bool TryFindBuilderPropertyInBaseType( INamedType baseType, ScopedDiagnosticSink diagnosticSink,[NotNullWhen(true)] out IProperty? baseProperty  )
    {
        baseProperty =
            baseType.AllProperties.OfName( this.SourceProperty.Name )
                .SingleOrDefault();

        if ( baseProperty == null )
        {
            diagnosticSink.Report(
                BuilderDiagnosticDefinitions.BaseBuilderMustContainProperty.WithArguments( (
                    baseType, this.SourceProperty.Name) ) );
            return false;
        }

        return true;


    }
        
    [Template]
    public virtual void SetBuilderPropertyValue( IExpression expression, IExpression builderInstance )
    {
        // Abstract templates are not supported, so we must create a virtual method and override it.
        throw new NotSupportedException();
    }

}