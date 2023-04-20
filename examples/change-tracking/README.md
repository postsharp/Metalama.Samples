---
uid: sample-dirty
---

# Example: Change Tracking

The _Dirty Flag_ pattern is a design pattern used to track changes to an object's state by maintaining a boolean flag that is set to true whenever the object's properties or fields are modified. This pattern is commonly used in user interface programming, where it is necessary to quickly determine whether an object's state has changed so that the `Save` button can be dynamically enabled. In .NET, objects that support this feature must implement the <xref:System.ComponentModel.IChangeTracking> interface.

Implementing the _Dirty Flag_ pattern typically requires adding a lot of boilerplate code to each class that needs to track changes to its state. However, this repetitive and error-prone task can be automated by using aspect-oriented programming (AOP) techniques. In this example, we will see how to create an aspect that automates this pattern, reducing the amount of boilerplate code you need to write and maintain, while also improving code quality and consistency.

| Article | Description |
| --- | --- |
| [1. Getting started](change-tracking-1/README.md) | Gives a minimal implementation of the change tracking pattern. |
| [2. Verifying code](change-tracking-2/README.md) | Reports errors if the manual implementation of the pattern does not respect the conventions of the pattern. |
| [3. Integrating with INotifyPropertyChanged](change-tracking-3/README.md) | Raises `PropertyChange` notifications when the `IsChanged` property has changed. |
| [4. Reverting changes](change-tracking-4/README.md) | Adds support for the `RevertChanges` method. |
| [Limitations](limitations.md) | This article discusses the limitations of the above aspects. |
