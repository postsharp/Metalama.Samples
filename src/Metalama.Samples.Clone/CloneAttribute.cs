﻿// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Samples.Clone
{
    [Inherited]
    [LiveTemplate]
    internal class DeepCloneAttribute : TypeAspect
    {
        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            builder.Advice.IntroduceMethod(
                builder.Target,
                nameof(this.CloneImpl),
                whenExists: OverrideStrategy.Override,
                args: new { T = builder.Target },
                buildMethod: m =>
                {
                    m.Name = "Clone";
                    m.ReturnType = builder.Target;
                } );

            builder.Advice.ImplementInterface(
                builder.Target,
                typeof(ICloneable),
                whenExists: OverrideStrategy.Ignore );
        }

        [Template( IsVirtual = true )]
        public T CloneImpl<[CompileTime] T>()
        {
            // This compile-time variable will receive the expression representing the base call.
            // If we have a public Clone method, we will use it (this is the chaining pattern). Otherwise,
            // we will call MemberwiseClone (this is the initialization of the pattern).
            IExpression baseCall;

            if ( meta.Target.Method.IsOverride )
            {
                meta.DefineExpression( meta.Base.Clone(), out baseCall );
            }
            else
            {
                meta.DefineExpression( meta.Base.MemberwiseClone(), out baseCall );
            }

            // Define a local variable of the same type as the target type.
            var clone = (T) baseCall.Value!;

            // Select clonable fields.
            var clonableFields =
                meta.Target.Type.FieldsAndProperties.Where(
                    f => f.IsAutoPropertyOrField &&
                         ((f.Type.Is( typeof(ICloneable) ) && f.Type.SpecialType != SpecialType.String) ||
                          (f.Type is INamedType fieldNamedType && fieldNamedType.Aspects<DeepCloneAttribute>().Any())) );

            foreach ( var field in clonableFields )
            {
                // Check if we have a public method 'Clone()' for the type of the field.
                var fieldType = (INamedType) field.Type;
                var cloneMethod = fieldType.Methods.OfExactSignature( "Clone", Array.Empty<IType>() );

                if ( cloneMethod is { Accessibility: Accessibility.Public } ||
                     fieldType.Aspects<DeepCloneAttribute>().Any() )
                {
                    // If yes, call the method without a cast.
                    field.Invokers.Base!.SetValue(
                        clone,
                        meta.Cast( fieldType, field.ToExpression().Value?.Clone() ) );
                }
                else
                {
                    // If no, use the interface.
                    field.Invokers.Base!.SetValue(
                        clone,
                        meta.Cast( fieldType, ((ICloneable?) field.ToExpression().Value)?.Clone() ) );
                }
            }

            return clone;
        }

        [InterfaceMember( IsExplicit = true )]
        private object Clone() => meta.This.Clone();
    }
}