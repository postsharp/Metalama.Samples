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
                    value = meta.Cast( meta.Target.FieldOrProperty.Type, ServiceLocator.ServiceProvider.GetService(meta.Target.FieldOrProperty.Type.ToType() ) );

                    // Set the field/property to the new value.
                    meta.Target.Property.Value = value
                        ?? throw new InvalidOperationException($"Cannot get a service of type {meta.Target.FieldOrProperty.Type}.");
                }

                return value;
            }

            set => meta.Proceed();
        }
    }
}
