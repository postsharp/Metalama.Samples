using DefaultNamespace;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

[Inheritable] /*<ClassDefinition>*/
public sealed class MementoAttribute : TypeAspect /*</ClassDefinition>*/
{
    [CompileTime]
    private record BuildAspectInfo(
        // The newly introduced Memento type.
        INamedType MementoType, 
        // Mapping from fields or properties in the Originator to the corresponding property
        // in the Memento type.
        Dictionary<IFieldOrProperty, IProperty> PropertyMap,
        // The Originator property in the new Memento type.
        IProperty? OriginatorProperty );

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        /*<GetBaseType>*/
        var isBaseMementotable = builder.Target.BaseType?.Is( typeof(IMementoable) ) == true;

        INamedType? baseMementoType;
        IConstructor? baseMementoConstructor;
        if ( isBaseMementotable )
        {
            var baseTypeDefinition = builder.Target.BaseType!.Definition;
            baseMementoType = baseTypeDefinition.Types.OfName( "Memento" )
                .SingleOrDefault();

            if ( baseMementoType == null )
            {
                builder.Diagnostics.Report(
                    DiagnosticDefinitions.BaseTypeHasNoMementoType.WithArguments( baseTypeDefinition ) );
                builder.SkipAspect();
                return;
            }

            if ( baseMementoType.Accessibility !=
                 Metalama.Framework.Code.Accessibility.Protected )
            {
                builder.Diagnostics.Report( 
                    DiagnosticDefinitions.MementoTypeMustBeProtected.WithArguments( baseMementoType ) );
                builder.SkipAspect();
                return;
            }

            if ( baseMementoType.IsSealed )
            {
                builder.Diagnostics.Report( 
                    DiagnosticDefinitions.MementoTypeMustNotBeSealed.WithArguments( baseMementoType ) );
                builder.SkipAspect();
                return;
            }

            baseMementoConstructor = baseMementoType.Constructors
                .FirstOrDefault( c => c.Parameters.Count == 1 &&
                                      c.Parameters[0].Type.Is( baseTypeDefinition ) );

            if ( baseMementoConstructor == null )
            {
                builder.Diagnostics.Report( 
                    DiagnosticDefinitions.MementoTypeMustHaveConstructor
                        .WithArguments( (baseMementoType,baseTypeDefinition ) ) );
                builder.SkipAspect();
                return;
            }

            if ( baseMementoConstructor.Accessibility is not (Metalama.Framework.Code.Accessibility
                    .Protected or Metalama.Framework.Code.Accessibility.Public) )
            {
                builder.Diagnostics.Report( 
                    DiagnosticDefinitions.MementoConstructorMustBePublicOrProtected
                        .WithArguments( baseMementoConstructor ) );
                builder.SkipAspect();
                return;
            }

        }
        else
        {
            baseMementoType = null;
            baseMementoConstructor = null;
        }
        /*</GetBaseType>*/
        
        /*<IntroduceType>*/
        // Introduce a new private nested class called Memento.
        var mementoType =  
            builder.IntroduceClass(
                "Memento",
                whenExists: OverrideStrategy.New,
                buildType: b =>
                {
                    b.Accessibility = Metalama.Framework.Code.Accessibility.Protected;
                    b.BaseType = baseMementoType;
                } ); /*</IntroduceType>*/ 
        
        var originatorFieldsAndProperties = builder.Target.FieldsAndProperties /* <SelectFields> */
            .Where( p => p is
            {
                IsStatic: false,
                IsAutoPropertyOrField: true, 
                IsImplicitlyDeclared: false,
                Writeability: Writeability.All
            } )
            .Where( p => !p.Attributes.OfAttributeType( typeof(MementoIgnoreAttribute) ).Any() ); /* </SelectFields> */

        // Introduce data properties to the Memento class for each field of the target class.
        var propertyMap = new Dictionary<IFieldOrProperty, IProperty>();  /* <IntroduceProperties> */

        foreach ( var fieldOrProperty in originatorFieldsAndProperties )
        {
            var introducedField = mementoType.IntroduceProperty(
                nameof(this.MementoProperty),
                buildProperty: b =>
                {
                    var trimmedName = fieldOrProperty.Name.TrimStart( '_' );

                    b.Name = trimmedName.Substring( 0, 1 ).ToUpperInvariant() +
                             trimmedName.Substring( 1 );
                    b.Type = fieldOrProperty.Type;
                } );

            propertyMap.Add( fieldOrProperty, introducedField.Declaration );
        } /* </IntroduceProperties> */
        
        // Add a constructor to the Memento class that records the state of the originator.
        mementoType.IntroduceConstructor( /*<IntroduceConstructor>*/
            nameof(this.MementoConstructorTemplate),
            buildConstructor: b =>
            {
                var parameter = b.AddParameter( "originator", builder.Target );
                
                if ( baseMementoConstructor != null )
                {
                    b.InitializerKind =  ConstructorInitializerKind.Base;    
                    b.AddInitializerArgument( parameter );
                }
                
            } ); /*</IntroduceConstructor>*/


        // Implement the IMemento interface on the Memento class and add its members.   
        mementoType.ImplementInterface( typeof(IMemento), whenExists: OverrideStrategy.Ignore ); /*<AddMementoInterface>*/
        
        var introducePropertyResult = mementoType.IntroduceProperty( 
            nameof(this.Originator),
            whenExists: OverrideStrategy.Ignore );
        var originatorProperty = introducePropertyResult.Outcome == AdviceOutcome.Default
            ? introducePropertyResult.Declaration
            : null;
        
        
        /*</AddMementoInterface>*/

        // Implement the rest of the IOriginator interface and its members.
        builder.ImplementInterface( typeof(IMementoable), OverrideStrategy.Ignore );

        builder.IntroduceMethod(
            nameof(this.SaveToMemento),
            whenExists: OverrideStrategy.Override,
            buildMethod: m => m.IsVirtual = !builder.Target.IsSealed,
            args: new { mementoType = mementoType.Declaration } );

        builder.IntroduceMethod(
            nameof(this.RestoreMemento),
            buildMethod: m => m.IsVirtual = !builder.Target.IsSealed,
            whenExists: OverrideStrategy.Override );

        // Pass the state to the templates.
        builder.Tags = new BuildAspectInfo( mementoType.Declaration, propertyMap, /* <SetTag> */
            originatorProperty ); /* </SetTag> */
    }

    [Template] public object? MementoProperty { get; } 

    [Template] public IMementoable? Originator { get; }

    [Template]
    public IMemento SaveToMemento()
    {
        var buildAspectInfo = (BuildAspectInfo) meta.Tags.Source!; /* <GetTag> */
        /* </GetTag> */

        // Invoke the constructor of the Memento class and pass this object as the originator.
        return buildAspectInfo.MementoType.Constructors.Single()
            .Invoke( (IExpression) meta.This )!;
    }

    [Template]
    public void RestoreMemento( IMemento memento )
    {
        var buildAspectInfo = (BuildAspectInfo) meta.Tags.Source!;
        
        // Call the base method if any.
        meta.Proceed();

        var typedMemento = meta.Cast( buildAspectInfo.MementoType, memento );

        // Set fields of this instance to the values stored in the Memento.
        foreach ( var pair in buildAspectInfo.PropertyMap )
        {
            pair.Key.Value = pair.Value.With( (IExpression) typedMemento ).Value;
        }
    }

    [Template] /* <ConstructorTemplate> */
    public void MementoConstructorTemplate()
    {
        var buildAspectInfo = (BuildAspectInfo) meta.Tags.Source!;

        // Set the originator property and the data properties of the Memento.
        if ( buildAspectInfo.OriginatorProperty != null )
        {
            buildAspectInfo.OriginatorProperty.Value = meta.Target.Parameters[0];
        }
        else
        {
            // We are in a derived type and there is no need to assign the property.
        }

        foreach ( var pair in buildAspectInfo.PropertyMap )
        {
            pair.Value.Value = pair.Key.With( meta.Target.Parameters[0] ).Value;
        }
    } /* </ConstructorTemplate> */
}