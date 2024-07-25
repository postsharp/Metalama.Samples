---
uid: samples
---

# Commented examples

This chapter shows a few examples that demonstrate how to use Metalama. Each of them is designed as a tutorial, progressively introducing features of growing complexity. You can use these examples to support your learning process. 

| Example                               | Description                                                                                                     |
|---------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| [Logging](log/README.md)              | Shows several logging aspects, adding complexity at every step.                                                |
| [Caching](caching/README.md)          | Caches the method return value as a function of its parameters.                                                 |
| [Exception Handling](exception-handling/README.md) | Demonstrates several exception-handling strategies including retry, [Poly](https://github.com/App-vNext/Polly), and adding parameter values for richer reports. |
| [INotifyPropertyChanged](notifypropertychanged/README.md) | Automatically implements the <xref:System.ComponentModel.INotifyPropertyChanged> interface and instruments properties. |
| [Change Tracking](change-tracking/README.md)              | Automatically implements the <xref:System.ComponentModel.IChangeTracking> or <xref:System.ComponentModel.IRevertibleChangeTracking> interface  interface and instruments fields and properties accordingly.                      |
| [Clone](clone/README.md)              | Implements the _Deep Clone_ pattern.                                                                             |
| [Memento](memento/README.md)          | Implements the classic _Memento_ pattern.
| [Enum View-Model](enum-viewmodel/README.md) | Creates a view-model class to wrap an enum value.                                                                |
| [Shared Fabric](fabrics/shared/README.md) | Demonstrates a fabric that targets several projects. Contributed by Whit Waldo. |
<!--
| [ToString](tostring/README.md)        | Implements the <xref:System.Object.ToString*> method.                                                           |

| [Optional Value Type](optional-value/README.md) | Transforms automatic properties of a type to make them store and represent a flag indicating whether they have been set or they are still at their default value. |
-->

If you have developed an original aspect with Metalama, consider [contributing](contributing.md) to this list.
