using Metalama.Framework.Aspects;

[assembly: AspectOrder( typeof(TrackChangesAttribute), typeof(NotifyPropertyChangedAttribute) )]