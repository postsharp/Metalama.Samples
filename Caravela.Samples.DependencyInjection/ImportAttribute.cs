using System;
using System.Data.SqlTypes;
using Caravela.Framework.Aspects;
using Caravela.Framework.Diagnostics;

namespace Caravela.Samples.DependencyInjection
{
    internal class ImportAttribute : OverrideFieldOrPropertyAspect
    {
        private static readonly SuppressionDefinition _suppressCs8618 = new("CS8618");

        public override dynamic? OverrideProperty
        {
            get
            {
                // Get the property value.
                var value = meta.Proceed();

                if (value == null)
                {
                    // Call the service locator.
                    value = meta.Cast(meta.Target.FieldOrProperty.Type,
                        ServiceLocator.ServiceProvider.GetService(meta.Target.FieldOrProperty.Type.ToType()));

                    // Set the field/property to the new value.
                    meta.Target.Property.Value = value
                                                 ?? throw new InvalidOperationException(
                                                     $"Cannot get a service of type {meta.Target.FieldOrProperty.Type}.");
                }

                // Suppress warning CS8618: Non-nullable property '_service' must contain a non-null value when exiting constructor.
                meta.Diagnostics.Suppress(_suppressCs8618);

                return value;
            }
            set => meta.Proceed();
        }
    }
}