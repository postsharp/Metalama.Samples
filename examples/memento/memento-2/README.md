---
uid: sample-memento-1
level: 400
---

# Memento example, step 1: Restoring state using Memento pattern

[!metalama-project-buttons .]

In UI application development, it is common to have a functionality that allows users to undo or redo their actions.
However, implementing this functionality can be challenging, especially when done by hand.

The Memento pattern is a behavioral design pattern that allows you to capture the internal state of an object as a memento 
object without violating encapsulation, store the memento object, and restore the original state later when needed. 
Using Metalama you can automatically create the Memento types and implement the common interfaces used by the program to 
retrieve and restore the state stored in mementos.

In this example, we implement a simple WPF application, that tracks fish in a home aquarium. It allows the user to add, 
edit and remove fish in evidence. 

Initially, the application will use Memento pattern for the undo functionality, that will work adding and removing fish 
and for editing them, including cancellations of editing.

This gives us a starting point to implement the undo/redo functionality in the next stage of the sample.

## Implementation

Let's now look at the aspect code.

[!metalama-file MementoAttribute.cs]

We derive our `MementoAttribute` from <xref:Metalama.Framework.Aspects.TypeAspect> abstract class, which is a base class
for aspects that target types. This aspect does not provide any specific behavior, we have provide all advices ourselves.

In <xref:Metalama.Framework.Aspects.AdviceFactory.BuildAspect*> method we do the following to create the `Memento` type:
- Call <xref:Metalama.Framework.Advising.AdviserExtensions.IntroduceClass*> to introduce the nested class.
- Add `Originator` property that stores the reference to the target.
- Add fields that store the captured state of the target instance.
- Add instance constructor that initializes the fields with the current state of the target instance.
- Implement the `IMemento` interface.

Then, we finish changes to the target class:
- Add `Restore` method that restores the state of the target instance from the memento.
- Add `Capture` method that creates a new memento with the current state of the target instance.
- Implement the `IOriginator` interface.

> [!NOTE]
> This example is using <xref:Metalama.Framework.Advising.IAdviser`1> extension methods, which were added in Metalama 2024.2.
> Corresponding API is also available in <xref:Metalama.Framework.Aspects.IAdviceFactory>.

> [!div class="see-also"]
> <xref:dependency-injection>