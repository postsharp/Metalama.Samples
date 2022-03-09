// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System;
using System.Threading;

namespace Metalama.Samples.DependencyInjection
{
    internal class InjectAttribute : OverrideFieldOrPropertyAspect
    {
        //  Non-nullable property 'X' must contain a non-null value when exiting constructor.
        private static readonly SuppressionDefinition _suppressCs8618 = new( "CS8618" );

        //  Field 'X' is never assigned to, and will always have its default value null.
        private static readonly SuppressionDefinition _suppressCs0649 = new("CS0649");

        //  Make field read-only.
        private static readonly SuppressionDefinition _suppressIde0044 = new( "IDE0044" );

        public override void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
        {
            base.BuildAspect( builder );

            // Suppress warnings.
            builder.Diagnostics.Suppress( _suppressCs8618 );
            builder.Diagnostics.Suppress( _suppressCs0649 );
            builder.Diagnostics.Suppress( _suppressIde0044 );
        }

        [Introduce( WhenExists =OverrideStrategy.Ignore )]
        private readonly IServiceProvider _serviceProvider = ServiceLocator.Current;

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
                        this._serviceProvider.GetService( meta.Target.FieldOrProperty.Type.ToType() ) );

                    // Set the field/property to the new value.
                    meta.Target.Property.Value = value
                                                 ?? throw new InvalidOperationException( $"Cannot get a service of type {meta.Target.FieldOrProperty.Type}." );
                }

                return value;
            }
            set => throw new NotSupportedException();
        }
    }


    internal class ServiceLocator
    {

        private static readonly AsyncLocal<IServiceProvider?> _current = new();

        public static IServiceProvider Current
        {
            get => _current.Value ?? throw new InvalidOperationException();
            set => _current.Value = value;
        }
    }
}