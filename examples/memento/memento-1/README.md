---
uid: sample-memento-1
level: 400
---

# Memento example

[!metalama-project-buttons .]

The Memento pattern is a classic behavioral design pattern that allows you to capture the internal state of an object as a _memento_ object without violating encapsulation. You can then restore this memento state later.

In UI application development, the Memento pattern is commonly used to implement an undo/redo feature.

Implementing the Memento pattern typically requires a lot of boilerplate code. In this article, we will see how to automatically implement this pattern using Metalama. We will do this in the context of a simple WPF application that tracks fish in a home aquarium.

At the heart of the Memento pattern, we have the following interface:

[!metalama-file IMementoable.cs]

> [!NOTE]
> This interface is named `IOriginator` in the classic Gang-of-Four book. We continue to refer to this object as the _originator_ in the context of this article.

The _memento_ class is typically a private nested class implementing the following interface:

[!metalama-file IMemento.cs]

Our objective is to generate the code supporting the `SaveToMemento` and `RestoreMemento` methods in the following class:

[!metalama-compare Fish.cs]

> [!NOTE]
> This example also uses the <xref:observability?text=[Observable]> aspect to implement the `INotifyPropertyChanged` interface.

How can we implement this aspect?

## Implementation overview

The first step is to list all the code operations that we need to perform:

1. Add a nested type named `Memento` with the following members:
   * The `IMemento` interface and its `Originator` property.
   * A private field for each field or automatic property of the originator (`IMementoable`) object, copying its name and property.
   * A constructor that accepts the originator types as an argument and copies its fields and properties to the private fields of the `Memento` object.
2. Implement the `IMementoable` interface with the following members:
   * The `SaveToMemento` method that returns an instance of the new `Memento` type (effectively returning a copy of the state of the object).
   * The `RestoreMemento` method that copies the properties of the `Memento` object back to the fields and properties of the originator.



## Passing state between BuildAspect and the templates

As always with non-trivial Metalama aspects, our `BuildAspect` method performs the code analysis and adds or overrides members using the `IAspectBuilder` advising API. Templates implementing the new methods and constructors read this state.

The following state encapsulates the state that is shared between `BuildAspect` and the templates:

[!metalama-file MementoAttribute.cs member="MementoAttribute.BuildAspectInfo"]

The crucial part is the `PropertyMap` dictionary, which maps fields and properties of the originator class to the corresponding property of the Memento type.

We use the `Tags` facility to pass state between `BuildAspect` and the templates. At the end of `BuildAspect`, we set the tag:

[!metalama-file MementoAttribute.cs marker="SetTag"]

Then, in the templates, we read it:

[!metalama-file MementoAttribute.cs marker="GetTag"]

## Step 1. Introducing the Memento type

The first step is to introduce a nested type named `Memento`.

[!metalama-file MementoAttribute.cs marker="IntroduceType"]

We store the result in a local variable named `mementoType`. We will use it to construct the type.

## Step 2. Introducing and mapping the properties

We select the mutable fields and automatic properties, except those that have a `[MementoIgnore]` attribute.

[!metalama-file MementoAttribute.cs marker="SelectFields"]

We iterate through this list and create the corresponding public property in the new `Memento` type. While doing this, we update the `propertyMap` dictionary, mapping the originator type field or property to the `Memento` type property.

[!metalama-file MementoAttribute.cs marker="IntroduceProperties"]

Here is the template for these properties:

[!metalama-file MementoAttribute.cs member="MementoAttribute.MementoProperty"]

## Step 3. Adding the Memento constructor

Now that we have the properties and the mapping, we can generate the constructor of the `Memento` type.

[!metalama-file MementoAttribute.cs marker="IntroduceConstructor"]

Here is the constructor template. We iterate the `PropertyMap` to set the `Memento` properties from the _originator_.

[!metalama-file MementoAttribute.cs member="MementoAttribute.MementoConstructorTemplate"]

## Step 4. Implementing the IMemento interface in the Memento type

Let's now implement the `IMemento` interface in the `Memento` nested type. Here is the `BuildAspect` code:

[!metalama-file MementoAttribute.cs marker="AddMementoInterface"]

This interface has a single member:

[!metalama-file MementoAttribute.cs member="MementoAttribute.Originator"]


## Step 5. Implementing the IMementoable interface in the originator type

We can finally implement the `IMementoable` interface.

[!metalama-file MementoAttribute.cs marker="AddMementoableInterface"]

Here are the interface members:

[!metalama-file MementoAttribute.cs member="MementoAttribute.SaveToMemento"]

[!metalama-file MementoAttribute.cs member="MementoAttribute.RestoreMemento"]

Note again the use of the property mapping in the `RestoreMemento` method.

## Complete aspect

Let's now put all the bits together. Here is the complete source code of our aspect, `MementoAttribute`.

[!metalama-file MementoAttribute.cs]

> [!div class="see-also"]
> <xref:dependency-injection>
