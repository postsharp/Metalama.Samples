using System;
using System.ComponentModel;
using System.Linq;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;

namespace Caravela.Samples.NotifyPropertyChanged
{
    internal class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
    {
        public void BuildAspect(IAspectBuilder<INamedType> builder)
        {
            builder.Advices.ImplementInterface(builder.Target, typeof(INotifyPropertyChanged));

            foreach (var property in builder.Target.Properties.Where(
                p => !p.IsAbstract && p.Writeability == Writeability.All))
            {
                builder.Advices.OverrideFieldOrPropertyAccessors(
                    property, null, nameof(OverridePropertySetter));
            }
        }

        [InterfaceMember] 
        public event PropertyChangedEventHandler? PropertyChanged;

        [Introduce(WhenExists = OverrideStrategy.Ignore)]
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(meta.This, new PropertyChangedEventArgs(name));
        }

        [Template]
        private dynamic OverridePropertySetter(dynamic value)
        {
            if (value != meta.Target.Property.Value)
            {
                meta.Proceed();
                this.OnPropertyChanged(meta.Target.Property.Name);
            }

            return value;
        }
    }
}