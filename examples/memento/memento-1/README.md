---
uid: sample-log-4
level: 300
---

# Memento example, step 1: Restoring state using Memento pattern

[!metalama-project-buttons .]

In UI application development, it is common to have a functionality that allows users to undo or redo their actions.
However, implementing this functionality can be challenging, especially when done by hand.

The Memento pattern is a behavioral design pattern that allows you to capture the internal state of an object without
violating encapsulation, remember it, and restore it later when needed. Using Metalama you can automatically create
the Memento types and implement the common interfaces used by the program to retrieve and restore the state store in
mementos.

In this example, we implement a simple WPF application, that tracks fish in a home aquarium. It allows the user to add, 
edit and remove fish in evidence. However, the application was initially missing any ability to undo or redo 
changes.

In this step we will implement the simplest for of the Memento pattern and use it to restore the state of the `Fish`
class when the user cancels editing of a fish.

This gives us a starting point to implement the undo/redo functionality later in this sample.

## Implementation

Let's now look at the aspect code.

[!metalama-file MementoAttribute.cs]

We derive our `MementoAttribute` from `TypeAspect`, which is a base type for aspects that are added to types. 


> [!div class="see-also"]
> <xref:dependency-injection>
