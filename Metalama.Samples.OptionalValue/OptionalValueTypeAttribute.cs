// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System.Linq;

namespace Metalama.Samples.OptionalValue
{
    internal class OptionalValueTypeAttribute : TypeAspect
    {
        private static readonly DiagnosticDefinition<INamedType> _missingNestedTypeError = new(
            "OPT001",
            Severity.Error,
            "The [OptionalValueType] aspect requires '{0}' to contain a nested type named 'Optional'" );

        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            var nestedType = builder.Target.NestedTypes.OfName( "Optional" ).FirstOrDefault();

            if ( nestedType == null )
            {
                builder.Diagnostics.Report( _missingNestedTypeError.WithArguments(builder.Target), builder.Target );
                return;
            }

            var optionalValuesProperty = builder.Advices.IntroduceProperty( builder.Target, nameof( this.OptionalValues ) );
            optionalValuesProperty.Type = nestedType;
            optionalValuesProperty.InitializerExpression = meta.ParseExpression( $"new {nestedType.Name}()" );

            var optionalType = (INamedType) builder.Target.Compilation.TypeFactory.GetTypeByReflectionType( typeof( OptionalValue<> ) );

            foreach ( var property in builder.Target.Properties.Where( p => p.IsAutoPropertyOrField ) )
            {
                var propertyBuilder = builder.Advices.IntroduceProperty( nestedType, nameof( this.OptionalPropertyTemplate ) );
                propertyBuilder.Name = property.Name;
                propertyBuilder.Type = optionalType.ConstructGenericInstance( property.Type );

                builder.Advices.OverrideFieldOrProperty( property, nameof(this.OverridePropertyTemplate ) );
            }
        }

        [Template]
        public dynamic OptionalValues { get;private set; }

        [Template]
        public dynamic? OptionalPropertyTemplate { get; private set; }

        [Template]
        public dynamic? OverridePropertyTemplate
        {
            get
            {
                var optionalProperty = (IProperty) meta.Tags["optionalProperty"]!;
                return optionalProperty.Invokers.Base!.GetValue( meta.This.OptionalValues );
            }

            set
            {
                var optionalProperty = (IProperty) meta.Tags["optionalProperty"]!;
                optionalProperty.Invokers.Base!.SetValue( meta.This.OptionalValues, value );
            }
        }


    }


    public struct OptionalValue<T>
    {
        private T _value;

        public bool IsSpecified { get; private set; }

        public T Value
        {
            get => this._value;
            private set
            {
                this.IsSpecified = true;
                this._value = value;
            }
        }

    }



}
