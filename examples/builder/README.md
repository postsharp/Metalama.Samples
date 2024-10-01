---
uid: sample-builder
summary: "This chapters explains how automatically implement the builder pattern in C# thanks to Metalama."
keywords: "builder pattern C#, c# builder pattern, builder design pattern C#"
created-date: 2024-09-30
modified-date: 2024-09-30
---

# Implementing the Builder pattern without boilerplate

The popularity of _immutable_ objects made the Builder pattern one of the most important ones in C#. Indeed, the most frequent use of builders in C# today is to ease the creation of immutable objects using a mutable objects, named the _builder_, used only to create the immutable one. An example of use of this pattern is the `System.Collections.Immutable` namespace, where each collection type comes with its own mutable builder.

Without builders, constructing complex immutable objects can be cumbersome because all properties must be set at once. The builder pattern allows to split the object creation process in several steps. 

Typically, a builder type would be almost a copy of the corresponding immutable type, with mutable properties instead of read-only ones. An exception of this rule is for properties of _collection_ type. Typically, the immutable type would have a properties of other immutable types, while the builder will rather have properties of mutable types.

A proper implementation of the `Builder` pattern should exhibit the following features:

* A `Builder` constructor accepting all required properties.
* A writable property in the `Builder` type corresponding to each property of the build type. For properties returning an immutable collection, the property of the `Builder` type should be read-only but return the corresponding _mutable_ collection type.
* A `Builder.Build` method returning the built immutable object.
* The ability to call an optional `Validate` method when an object is built.
* In the source type, a `ToBuilder` method returning a `Builder` initialized with the current values.

To support these features, some implementation details are necessary, such as internal constructors for the source and the `Builder` type.

As you can imagine, implementing the Builder pattern by hand is a boring and repetitive task. Fortunately, precisely because it is repetitive, it can be automated using a Metalama aspect.

## Alternative

An alternative to the Builder pattern in C# is the use of `record` types, `init` properties, and the `with` keyword. Using vanilla c# is _always_ better than using an aspect if it covers your needs. 

However, using records has some limitations:

* If an object must be built in several steps, several instances of a `record` are required (modified in chain with the `with` keyword), while a single `Builder` instance would be necessary in this case.
* Handling of collections is more convenient with the `Builder` pattern.
* It is not possible to validate a `record` before it is made accessible.


## In this series

In this series, we explain, step by step, how to implement the Builder pattern:


| Article | Description |
| ------- | ----------- |
| [Getting started](builder-1/README.md) | Presents a first working version of the GenerateBuilder aspect. |
| [Handling derived classes](builder-2/README.md) | Discusses how to handle non-sealed types. |
| [Handling immutable collections](builder-3/README.md) | Discusses how to generate mutable properties in the Builder class from immutable properties in the source class. |


