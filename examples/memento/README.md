---
uid: sample-memento
level: 300
---

# Example: Memento Pattern

The Memento pattern is a classic behavioral design pattern that allows you to capture the internal state of an object as a _memento_ object without violating encapsulation. You can then restore this memento state later.

In UI application development, the Memento pattern is commonly used to implement an undo/redo feature.

Implementing the Memento pattern typically requires a lot of boilerplate code.

## In this series

In this series, we will see how to automatically implement this pattern using Metalama.

We start with the most trivial implementation and progressively add new features:

| Article | Description |
|---------|-------------|
| [Basic aspect](memento-1/README.md) | This is the simplest possible implementation of the Memento pattern. |
| [Supporting type inheritance](memento-2/README.md) | We allow memento-able objects to inherit from each other. |
