---
uid: samples
---

# Examples

| Example | Description |
|--------|--------------|
| [Logging](log/README.md) | Shows several logging aspects, adding complexity at every step. |
| [Caching](cache/README.md) | Caches the method return value as a function of its parameters. |
| [Retry](retry/README.md) | Automatically retries the execution of the target method when an exception occurs. |
| [Enrich Exception](enrich-exception/README.md) | Adds the parameter values to the <xref:System.Exception> object so that they can be later included in the crash report. |
| [INotifyPropertyChanged](notifypropertychanged/README.md) | Automatically implements the <xref:System.ComponentModel.INotifyPropertyChanged> interface and instrument properties. |
| [Dirty](dirty/README.md) | Adds an `IsDirty` property to the type and sets whenever a field or property is modified. |
| [Clone](clone/README.md) | Implements the _deep clone_ pattern.
| [ToString](tostring/README.md) | Implements the <xref:System.Object.ToString*> method.
| [Enum View-Model](enum-viewmodel/README.md) | Creates a view-model class to wrap an enum value.
| [Optional Value Type](optional-value/README.md) | Transforms automatic properties of a type to make them store and represent a flag indicating whether they have been set, or if they are still at their default value.

If you developed an original aspect, consider [contributing](contributing.md) to this list.