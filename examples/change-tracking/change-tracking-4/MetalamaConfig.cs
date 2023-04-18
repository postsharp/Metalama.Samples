using Metalama.Framework.Aspects;
using Metalama.Samples.Dirty;
using Metalama.Samples.NotifyPropertyChanged;

[assembly: AspectOrder( typeof(TrackChangesAttribute), typeof(NotifyPropertyChangedAttribute))]