using System;
using Caravela.Framework.Aspects;

namespace Caravela.Samples.DependencyInjection
{
    internal class ImportAttribute : OverrideFieldOrPropertyAspect
    {
        public override dynamic OverrideProperty 
        { 
            get
            {
                // Get the property value.
                var value = meta.Proceed();

                if ( value == null )
                {
                    // Call the service locator.
                    value = meta.Cast( meta.FieldOrProperty.Type, ServiceLocator.ServiceProvider.GetService(meta.Property.Type.ToType() ) );

                    // Set the field/property to the new value.
                    meta.Property.Value = value
                        ?? throw new InvalidOperationException($"Cannot get a service of type {meta.Property.Type}.");
                }

                return value;
            }

            set => meta.Proceed();
        }
    }
}
