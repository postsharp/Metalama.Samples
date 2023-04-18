---
uid: sample-dirty-3
---

# Dirty Flag example, step 3: integrate with INotifyPropertyChanged

[!metalama-project-buttons .]

## Aspect implementation

[!metalama-file TrackChangesAttribute.cs]

The first thing we add to the `TrackChangesAttribute` is two static fields to _define_ the errors:

[!metalama-file TrackChangesAttribute.cs member="TrackChangesAttribute._mustHaveOnChangeMethod"]
[!metalama-file TrackChangesAttribute.cs member="TrackChangesAttribute._onChangeMethodMustBeProtected"]

Metalama requires the <xref:Metalama.Framework.Diagnostics.DiagnosticDefinition> to be defined in a static field or property. To learn more about reporting errors, see <xref:diagnostics>.

Then, we add this code to the `BuildAspect` method:

[!metalama-file TrackChangesAttribute.cs from="BuildAspect:Start" to="BuildAspect:End"]

As in the previous step, the `BuildAspect` method calls <xref:Metalama.Framework.Advising.IAdviceFactory.ImplementInterface*> with the `Ignore`  <xref:Metalama.Framework.Aspects.OverrideStrategy>.  This time, we inspect the outcome of <xref:Metalama.Framework.Advising.IAdviceFactory.ImplementInterface*>. If the outcome is `Ignored`, it means that type, or any base type, already implements the `IChangeTracking` interface. In this case, we check that the type contains a parameterless method named `OnChange` and we verify its accessibility.