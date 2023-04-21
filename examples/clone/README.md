---
uid: sample-clone
---

# Example: deep cloning

The Deep Clone pattern creates a copy of an object that is completely separate from the original object, including its properties and sub-properties. Deep cloning is achieved by recursively cloning all the child objects that make up the original object, creating a new instance of each one, and assembling the parent clone from the child clones.

The Deep Clone pattern requires a concept of an _object tree_, i.e., a concept of a _child property_. Objects typically have _reference_ properties and _child_ properties, and when deep cloning an object, only _child_ properties should be deep cloned.

Implementing deep cloning can be error-prone and time-consuming, requiring much boilerplate code. This series of articles shows how to automate this work using an aspect, making the code more maintainable and efficient.

> [!WARNING]
> Deep cloning can be expensive in terms of performance, so it should be used judiciously.

| Article | Description |
|--|--|
| [1. Getting started](clone-1/README.md) | Provides the first working version of the Deep Clone aspect. |
| [2. Verifying code](clone-2/README.md) | Reports errors when the child properties are not of a cloneable type. |
| [3. Allowing handmade customizations](clone-3/README.md) | Adds extension points to allow for handmade handling of properties. |
| [4. Adding coding guidance](clone-4/README.md) | Adds coding guidance to the refactoring menu to help developers write the customization method.
