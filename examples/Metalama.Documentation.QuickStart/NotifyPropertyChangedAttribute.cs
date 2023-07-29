using global::Metalama.Framework.Aspects;
using global::Metalama.Framework.Code;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel;

namespace Metalama.Documentation.QuickStart
{
  
    [Inheritable]
    public  class NotifyPropertyChangedAttribute : TypeAspect
    {
        public override void BuildAspect(IAspectBuilder<INamedType> builder)
        {
            builder.Advice.ImplementInterface(builder.Target, typeof(INotifyPropertyChanged), OverrideStrategy.Ignore);

            foreach (var property in builder.Target.Properties.Where(p =>
                         !p.IsAbstract && p.Writeability == Writeability.All))
            {
                builder.Advice.OverrideAccessors(property, null, nameof(this.OverridePropertySetter));
            }
        }

        [InterfaceMember] public event PropertyChangedEventHandler? PropertyChanged;

        [Introduce(WhenExists = OverrideStrategy.Ignore)]
        protected void OnPropertyChanged(string name) =>
            this.PropertyChanged?.Invoke(meta.This, new PropertyChangedEventArgs(name));

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
