using System.ComponentModel;

namespace Metalama.Samples.Dirty;

public interface ISwitchableChangeTracking : IChangeTracking
{
    /// <summary>
    /// Gets or sets a value indicating whether the current object
    /// is tracking its changes.
    /// </summary>
    bool IsTrackingChanges { get; set; }
}