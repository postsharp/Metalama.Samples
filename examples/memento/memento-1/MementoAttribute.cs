using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Diagnostics;

namespace Sample;

public class MementoAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var mementoFields = new List<IField>();
        var properties = new List<IProperty>();

        foreach ( var property in builder.Target.Properties )
        {
            var name = $"_{char.ToLowerInvariant(property.Name.First())}{property.Name.Substring(1)}Memento";
            var mementoField = builder.IntroduceField( name, property.Type );

            mementoFields.Add( mementoField.Declaration );
            properties.Add( property );
        }

        builder.IntroduceMethod( nameof( RememberState ), args: new { mementoFields, properties } );
        builder.IntroduceMethod( nameof( RestoreState ), args: new { mementoFields, properties } );
        builder.IntroduceMethod( nameof( DropState ), args: new { mementoFields } );
    }

    [Template]
    public void RememberState( [CompileTime] List<IField> mementoFields, [CompileTime] List<IProperty> properties )
    {
        var index = meta.CompileTime( 0 );

        foreach ( var field in mementoFields )
        {
            field.Value = properties[index].Value;

            index++;
        }
    }

    [Template]
    public void RestoreState( [CompileTime] List<IField> mementoFields, [CompileTime] List<IProperty> properties )
    {
        var index = meta.CompileTime(0);

        foreach ( var field in mementoFields )
        {
            properties[index].Value = field.Value;

            index++;
        }
    }

    [Template]
    public void DropState( [CompileTime] List<IField> mementoFields )
    {
        foreach ( var field in mementoFields )
        {
            field.Value = default;
        }
    }
}