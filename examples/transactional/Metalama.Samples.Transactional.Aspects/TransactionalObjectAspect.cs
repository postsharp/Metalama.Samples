using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Samples.Transactional.Aspects;

internal class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender ) =>
        amender.SelectTypesDerivedFrom( typeof(TransactionalObject) )
            .AddAspect<TransactionalObjectAspect>();
}

internal class TransactionalObjectAspect : IAspect<INamedType>
{
    public void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        if ( builder.Target.BaseType?.Is( typeof(TransactionalObject) ) != true )
        {
            builder.Diagnostics.Report(
                DiagnosticDefinitions.TypeMustBeTransactionalObject.WithArguments(
                    builder.Target ) );
            return;
        }


        if ( !this.TryBuildStateNestedType( builder, builder.Target.BaseType.Definition,
                out var stateType, out var stateTypeConstructor ) )
        {
            return;
        }

        var introducedObjConstructor =
        builder.IntroduceConstructor( nameof(this.RestoreObjectConstructorTemplate),
            whenExists:OverrideStrategy.Ignore,
            buildConstructor:
            constructor =>
            {
                constructor.InitializerKind = ConstructorInitializerKind.Base;
                constructor.AddInitializerArgument( constructor.Parameters[0] );
                constructor.AddInitializerArgument( constructor.Parameters[1] );
            } );


        if ( !this.TryBuildFactoryNestedType( builder, introducedObjConstructor.Declaration,
                stateTypeConstructor, out var factoryInstanceProperty ) )
        {
            return;
        }

        builder.IntroduceProperty( nameof(this.TransactionalObjectFactory),
            whenExists:OverrideStrategy.Override,
            tags: new { factoryInstanceProperty } );

        builder.IntroduceMethod( nameof(this.GetState), whenExists: OverrideStrategy.New,
            args: new { T = stateType } );

    }

    private bool TryBuildStateNestedType(
        IAspectBuilder<INamedType> builder,
        INamedType baseTypeDefinition,
        [NotNullWhen( true )] out INamedType? stateType,
        [NotNullWhen( true )] out IConstructor? stateTypeConstructor )
    {
        stateType = null;
        stateTypeConstructor = null;

        var baseStateType = baseTypeDefinition.Types
            .OfName( baseTypeDefinition.Name + "State" )
            .SingleOrDefault();

        if ( baseStateType == null )
        {
            builder.Diagnostics.Report(
                DiagnosticDefinitions.BaseTypeHasNoStateType.WithArguments(
                    baseTypeDefinition ) );
            return false;
        }

        if ( baseStateType.Accessibility !=
             Accessibility.Protected )
        {
            builder.Diagnostics.Report(
                DiagnosticDefinitions.StateTypeMustBeProtected.WithArguments(
                    baseStateType ) );
            return false;
        }

        if ( baseStateType.IsSealed )
        {
            builder.Diagnostics.Report(
                DiagnosticDefinitions.StateTypeMustNotBeSealed.WithArguments(
                    baseStateType ) );
            return false;
        }

        var baseStateConstructor = baseStateType.Constructors
            .FirstOrDefault( c => c.Parameters.Count == 1 &&
                                  c.Parameters[0].Type.Is( typeof(TransactionalObjectId) ) );

        if ( baseStateConstructor == null )
        {
            builder.Diagnostics.Report(
                DiagnosticDefinitions.StateTypeMustHaveConstructor
                    .WithArguments( baseStateType ) );
            return false;
        }

        if ( baseStateConstructor.Accessibility is not (Accessibility.Protected
            or Accessibility.Public) )
        {
            builder.Diagnostics.Report(
                DiagnosticDefinitions.StateConstructorMustBePublicOrProtected
                    .WithArguments( baseStateConstructor ) );
            builder.SkipAspect();
            return false;
        }


        var introducedStateType =
            builder.IntroduceClass(
                builder.Target.Name + "State",
                buildType: b =>
                {
                    b.Accessibility = Accessibility.Protected;
                    b.BaseType = baseStateType;
                } ); /*</IntroduceType>*/

        var originatorFieldsAndProperties = builder.Target.FieldsAndProperties /* <SelectFields> */
            .Where( p => p is
            {
                IsStatic: false,
                IsAutoPropertyOrField: true,
                IsImplicitlyDeclared: false
            } )
            .Where( p =>
                !p.Attributes.OfAttributeType( typeof(NotTransactionalAttribute) )
                    .Any() ); /* </SelectFields> */

        // Introduce data properties to the State class for each field of the target class.

        foreach ( var fieldOrProperty in originatorFieldsAndProperties )
        {
            var trimmedPropertyName = fieldOrProperty.Name.TrimStart( '_' );

            var dataFieldName = trimmedPropertyName.Substring( 0, 1 ).ToUpperInvariant() +
                                trimmedPropertyName.Substring( 1 );
                     
            var introducedDataField = introducedStateType.IntroduceField(
                dataFieldName,
                fieldOrProperty.Type,
                buildField: f =>
                {
                    f.Accessibility = Accessibility.Public;
                } );

            builder.With( fieldOrProperty ).OverrideAccessors(
                nameof(this.GetterTemplate),
                 nameof(this.SetterTemplate),
                args: new { stateField = introducedDataField.Declaration } );
        } /* </IntroduceProperties> */

        // Add a constructor to the State class that records the state of the originator.
        var introducedStateConstructor =
            introducedStateType.IntroduceConstructor( /*<IntroduceConstructor>*/
                nameof(this.StateConstructorTemplate),
                buildConstructor: b =>
                {
                    b.InitializerKind = ConstructorInitializerKind.Base;
                    b.AddInitializerArgument( b.Parameters[0] );
                } );


        // Implement the ISnapshot interface on the State class and add its members.   
        introducedStateType.ImplementInterface( typeof(ITransactionalObjectState),
            whenExists: OverrideStrategy.Ignore ); /*<AddStateInterface>*/


        stateType = introducedStateType.Declaration;
        stateTypeConstructor = introducedStateConstructor.Declaration;
        return true;
    }

    private bool TryBuildFactoryNestedType( IAspectBuilder<INamedType> builder, IConstructor objConstructor, IConstructor stateConstructor, [NotNullWhen(true)] out IProperty? instanceProperty )
    {
        var introducedFactoryClass = builder.IntroduceClass( 
            builder.Target.Name + "Factory",
            buildType: type => type.IsSealed = true );

        var introducedFactoryConstructor =
            introducedFactoryClass.IntroduceConstructor( nameof(this.FactoryConstructorTemplate) );
        


        introducedFactoryClass.IntroduceMethod( nameof(this.CreateInitialState),
            args: new { stateConstructor } );
        
        introducedFactoryClass.IntroduceMethod( nameof(this.CreateObject),
            args: new { objConstructor } );

        introducedFactoryClass.IntroduceProperty( nameof(this.ObjectType),
            tags: new { objType = objConstructor.DeclaringType } );
        
        var introducedInstanceProperty = introducedFactoryClass.IntroduceProperty( nameof(Instance),
            tags: new { factoryConstructor = introducedFactoryConstructor.Declaration } );

        instanceProperty = introducedInstanceProperty.Declaration;

        return true;

    }


    #region Templates for the State nested type

    [Template] /* <ConstructorTemplate> */
    public void StateConstructorTemplate( TransactionalObjectId id )
    {
    } /* </ConstructorTemplate> */
    
    [Template]
    public ITransactionalObjectState CreateInitialState( TransactionalObjectId id, IConstructor stateConstructor ) 
        => stateConstructor.Invoke( id )!;
    
    [Template]
    public ITransactionalObject CreateObject( TransactionalObjectId id, IMemoryTransactionAccessor transactionAccessor, IConstructor objConstructor ) 
        => objConstructor.Invoke( transactionAccessor, id )!;

    [Template] public Type ObjectType => ((INamedType) meta.Tags["objType"]!).ToTypeOfExpression().Value!;

    [Template]
    public static dynamic Instance { get; } = ((IConstructor) meta.Tags["factoryConstructor"]!).Invoke()!;

    #endregion

    #region Templates for the Factory nested type

    [Template]
    private void FactoryConstructorTemplate() { }

    #endregion

    #region Templates for the originator type

    [Template]
    private void RestoreObjectConstructorTemplate( IMemoryTransactionAccessor transactionAccessor, TransactionalObjectId id ) { }


    [Template]
    private T GetState<[CompileTime] T>( bool editing ) =>
        (T) ((IMemoryTransactionAccessor) meta.This.TransactionAccessor).GetObjectState( meta.This,
            editing );

    [Template]
    public dynamic? GetterTemplate( IField stateField )
        => stateField.With( (IExpression) meta.This.GetState( false ) ).Value;

    [Template]
    public void SetterTemplate( IField stateField, dynamic? value )
        => stateField.With( (IExpression) meta.This.GetState( true ) ).Value = value;

    [Template]
    protected ITransactionalObjectFactory TransactionalObjectFactory =>
        ((IProperty) meta.Tags["factoryInstanceProperty"]!).Value!;

    #endregion
}