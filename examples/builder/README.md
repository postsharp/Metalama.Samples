---
uid: sample-builder
summary: "This chapters explains how automatically implement the builder pattern in C# thanks to Metalama."
keywords: "builder pattern C#, c# builder pattern, builder design pattern C#"
created-date: 2024-09-30
modified-date: 2024-09-30
---

# Example: Builder

The popularity of _immutable_ objects made the Builder pattern one of the most important ones in C#. Indeed, the most frequent use of builders in C# today is to ease the creation of immutable objects using a mutable objects, the builder, used only to create the immutable one. An example of use of this pattern is the `System.Collections.Immutable` namespace, where each collection type comes with its own -- mutable -- builder.

Without builders, constructing complex immutable objects can be cumbersome because all properties must be set at once. The builder pattern allows to split the object creation process in several steps. 

Typically, a builder type would be almost a copy of the corresponding immutable type, with mutable properties instead of read-only ones. An exception of this rule is for properties of _collection_ type. Typically, the immutable type would have a properties of other immutable types, while the builder will rather have properties of mutable types.

Implementing the Builder pattern by hand is a boring and repetitive task. Fortunately, precisely because it is repetitive, it can be automated using a Metalama aspect.

An alternative to the Builder pattern in C# is the use of `record` types, `init` properties, and the `with` keyword.

In this chapter, we explain, step by step, how to implement the Builder pattern:


| Article | Description |
| ------- | ----------- |
| [Getting started](builder-1/README.md) | Presents a first working version of the GenerateBuilder aspect. |
| [Handling derived classes](builder-2/README.md) | Discusses how to handle non-sealed types. |

