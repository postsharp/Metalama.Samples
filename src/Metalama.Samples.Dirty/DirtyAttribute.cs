﻿// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System.Linq;

namespace Metalama.Samples.Dirty
{
    public class DirtyAttribute : TypeAspect
    {
        private static readonly DiagnosticDefinition<INamedType> _mustHaveDirtyStateSetter = new(
            "MY001",
            Severity.Error,
            "The 'IDirty' interface is implemented manually on type '{0}', but the property 'DirtyState' does not have a property setter." );

        private static readonly DiagnosticDefinition<IProperty> _dirtyStateSetterMustBeProtected = new(
            "MY002",
            Severity.Error,
            "The setter of the '{0}' property must be have the 'protected' accessibility." );

        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            // Implement the IDirty interface.
            if ( !builder.Target.ImplementedInterfaces.Any( i => i.Is( typeof(IDirty) ) ) )
            {
                builder.Advices.ImplementInterface( builder.Target, typeof(IDirty), OverrideStrategy.Ignore );
            }
            else
            {
                // If the type already implements IDirty, it must have a protected method called OnDirty, otherwise 
                // this is a contract violation, so we report an error.
                var dirtyStateProperty = builder.Target.Properties
                    .SingleOrDefault(
                        m => m.Name == nameof(this.DirtyState) && m.Parameters.Count == 0 &&
                             m.Type.Is( typeof(DirtyState) ) );

                if ( dirtyStateProperty?.SetMethod == null )
                {
                    _mustHaveDirtyStateSetter.WithArguments( builder.Target ).ReportTo( builder.Diagnostics );
                }
                else if ( dirtyStateProperty.SetMethod.Accessibility != Accessibility.Protected )
                {
                    _dirtyStateSetterMustBeProtected.WithArguments( dirtyStateProperty ).ReportTo( builder.Diagnostics );
                }
            }

            // Override all writable fields and automatic properties.
            var fieldsOrProperties = builder.Target.Properties
                .Cast<IFieldOrProperty>()
                .Concat( builder.Target.Fields )
                .Where( f => f.Writeability == Writeability.All );

            foreach ( var fieldOrProperty in fieldsOrProperties )
            {
                builder.Advices.OverrideFieldOrPropertyAccessors( fieldOrProperty, null, nameof(this.OverrideSetter) );
            }

            // TODO: This aspect is not complete. We should normally not set DirtyState to Clean after the object has been initialized,
            // but this is not possible in the current version of Metalama.
        }

        [InterfaceMember]
        public DirtyState DirtyState { get; protected set; }

        [Template]
        private void OverrideSetter()
        {
            meta.Proceed();

            if ( this.DirtyState == DirtyState.Clean )
            {
                this.DirtyState = DirtyState.Dirty;
            }
        }
    }
}