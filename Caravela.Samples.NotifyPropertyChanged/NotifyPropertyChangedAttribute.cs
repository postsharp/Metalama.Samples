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
             builder.AdviceFactory.ImplementInterface(builder.Target, typeof(INotifyPropertyChanged));

            foreach (var property in builder.Target.Properties.Where(
                p => !p.IsAbstract && p.Writeability == Writeability.All ))
            {
                builder.AdviceFactory.OverrideFieldOrPropertyAccessors(
                    property, null, nameof(OverridePropertySetter));
            }
        }

        [InterfaceMember]
        public event PropertyChangedEventHandler PropertyChanged;

        [Introduce( WhenExists = OverrideStrategy.Ignore )]
        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke( meta.This, new PropertyChangedEventArgs(name));
        }

        [Template]
        dynamic OverridePropertySetter( dynamic value )
        {
            if ( value != meta.Target.Property.Value )
            {
                meta.Proceed();
                this.OnPropertyChanged(meta.Target.Property.Name);
            }

            return value;
        }
    }
}
