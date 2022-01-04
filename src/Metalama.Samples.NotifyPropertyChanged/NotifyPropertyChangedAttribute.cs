// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel;
using System.Linq;

namespace Metalama.Samples.NotifyPropertyChanged
{
    [Inherited]
    internal class NotifyPropertyChangedAttribute : TypeAspect
    {
        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            builder.Advices.ImplementInterface( builder.Target, typeof(INotifyPropertyChanged) );

            foreach ( var property in builder.Target.Properties.Where( p => !p.IsAbstract && p.Writeability == Writeability.All ) )
            {
                builder.Advices.OverrideFieldOrPropertyAccessors( property, null, nameof(this.OverridePropertySetter) );
            }
        }

        [InterfaceMember]
        public event PropertyChangedEventHandler? PropertyChanged;

        [Introduce( WhenExists = OverrideStrategy.Ignore )]
        protected void OnPropertyChanged( string name )
        {
            this.PropertyChanged?.Invoke( meta.This, new PropertyChangedEventArgs( name ) );
        }

        [Template]
        private dynamic OverridePropertySetter( dynamic value )
        {
            if ( value != meta.Target.Property.Value )
            {
                meta.Proceed();
                this.OnPropertyChanged( meta.Target.Property.Name );
            }

            return value;
        }
    }
}