using System;
using System.Linq;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using Caravela.Framework.Diagnostics;

namespace Caravela.Samples.Dirty
{
    public class DirtyAttribute : Attribute, IAspect<INamedType>
    {
        static readonly DiagnosticDefinition<INamedType> _mustHaveDirtyStateSetter = new
            ("MY001", 
            Severity.Error, 
            "The 'IDirty' interface is implemented manually on type '{0}', but the property 'DirtyState' does not have a property setter.");

        static readonly DiagnosticDefinition<IProperty> _dirtyStateSetterMustBeProtected = new
            ("MY002", 
            Severity.Error, 
            "The setter of the '{0}' property must be have the 'protected' accessibility.");

        public void BuildAspect(IAspectBuilder<INamedType> aspectBuilder)
        {

            if ( !aspectBuilder.TargetDeclaration.ImplementedInterfaces.Any(i => i.Is(typeof(IDirty)) ) )
            {
                aspectBuilder.AdviceFactory.IntroduceInterface(aspectBuilder.TargetDeclaration, typeof(IDirty));
            }
            else
            {
                // If the type already implements IDirty, it must have a protected method called OnDirty, otherwise 
                // this is a contract violation.
                var dirtyStateProperty = aspectBuilder.TargetDeclaration.Properties.Where(m => m.Name == nameof(this.DirtyState) && m.Parameters.Count == 0 && m.Type.Is(typeof(DirtyState))).SingleOrDefault();

                if ( dirtyStateProperty?.Setter == null )
                {
                    aspectBuilder.Diagnostics.Report(_mustHaveDirtyStateSetter, aspectBuilder.TargetDeclaration);
                }
                else if ( dirtyStateProperty.Setter.Accessibility != Accessibility.Protected )
                {
                    aspectBuilder.Diagnostics.Report(_dirtyStateSetterMustBeProtected, dirtyStateProperty );
                }
            }

            var fieldsOrProperties = aspectBuilder.TargetDeclaration.Properties
                .Cast<IFieldOrProperty>()
                .Concat(aspectBuilder.TargetDeclaration.Fields)
                .Where(f => f.Writeability == Writeability.All);

            foreach ( var fieldOrProperty in fieldsOrProperties )
            {
                aspectBuilder.AdviceFactory.OverrideFieldOrPropertyAccessors(fieldOrProperty, null, nameof(OverrideSetter));
            }

            // TODO: This aspect is not complete. We should normally not set DirtyState to Clean after the object has been initialized,
            // but this is not possible in the current version of Caravela.

        }

        [InterfaceMember]
        public DirtyState DirtyState { get; protected set; }

        [Template]
        private void OverrideSetter()
        {
            // TODO: this syntax is ugly and it will be fix.
            var __ = meta.Proceed();

            if (this.DirtyState == DirtyState.Clean)
            {
                this.DirtyState = DirtyState.Dirty;
            }
        }
    }
}
