using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using System;
using System.Linq;
using System.ComponentModel;

namespace Caravela.Samples.NotifyPropertyChanged
{
    class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
    {
        public void BuildAspect( IAspectBuilder<INamedType> builder )
        {
             builder.AdviceFactory.ImplementInterface(builder.TargetDeclaration, typeof(INotifyPropertyChanged));

            foreach (var property in builder.TargetDeclaration.Properties.Where( p => !p.IsAbstract && p.Writeability == Writeability.All ))
            {
                builder.AdviceFactory.OverrideFieldOrPropertyAccessors(property, null, nameof(OverridePropertySetter));
            }
        }

        [InterfaceMember]
        public event PropertyChangedEventHandler PropertyChanged;

        [Introduce( WhenExists = OverrideStrategy.Ignore )]
        protected void OnPropertyChanged(string name)
        {
            meta.This.PropertyChanged?.Invoke( meta.This, new PropertyChangedEventArgs(name));
        }

        [Template]
        dynamic OverridePropertySetter( dynamic value )
        {
            if ( value != meta.Property.Value )
            {
                this.OnPropertyChanged(meta.Property.Name);
                var __ = meta.Proceed();
            }

            return value;
        }
    }
}
