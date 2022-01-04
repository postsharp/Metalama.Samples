// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System;

namespace Metalama.Samples.DependencyInjection
{
    internal class ImportAttribute : OverrideFieldOrPropertyAspect
    {
        private static readonly SuppressionDefinition _suppressCs8618 = new( "CS8618" );
        private static readonly SuppressionDefinition _suppressIde0044 = new( "IDE0044" );

        public override void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
        {
            base.BuildAspect( builder );

            // Suppress warning CS8618: Non-nullable property '_service' must contain a non-null value when exiting constructor.
            builder.Diagnostics.Suppress( builder.Target, _suppressCs8618 );

            // Suppress warning IDE0044: Make field read-only.
            builder.Diagnostics.Suppress( builder.Target, _suppressIde0044 );
        }

        public override dynamic? OverrideProperty
        {
            get
            {
                // Get the property value.
                var value = meta.Proceed();

                if ( value == null )
                {
                    // Call the service locator.
                    value = meta.Cast(
                        meta.Target.FieldOrProperty.Type,
                        ServiceLocator.ServiceProvider.GetService( meta.Target.FieldOrProperty.Type.ToType() ) );

                    // Set the field/property to the new value.
                    meta.Target.Property.Value = value
                                                 ?? throw new InvalidOperationException( $"Cannot get a service of type {meta.Target.FieldOrProperty.Type}." );
                }

                return value;
            }
            set => meta.Proceed();
        }
    }
}