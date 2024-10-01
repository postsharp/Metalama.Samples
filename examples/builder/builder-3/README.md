---
uid: sample-builder-3
summary: ""
keywords: "builder pattern C#, c# builder pattern, builder design pattern C#"
created-date: 2024-09-30
modified-date: 2024-09-30
level: 400
---

# Builder example, step 3: Handling immutable collection properties

[!metalama-project-buttons .]

In the previous articles, we authored an aspect that implements the `Builder` pattern for properties of "plain" type. Properties of collection types require a different handling.

Since the Builder pattern is typically used to build immutable objects, it is a good practice for properties of the immutable class to be themselves of an immutable type, e.g. `ImmutableArray` or `ImmutableDictionary`. However, in the `Builder` class, it is much more convenient if the collections are mutable. For instance, for a source property of type `Immutable<string>`, the builder property could be an `Immutable<string>.Builder`.

In this article, we will update the aspect so that the collection properties of the `Builder` class are of the _builder_ collection type.

In addition, we want the collection properties in the `Builder` type to be _lazy_, i.e. we only want to allocate a collection builder if the collection is evaluated.

Here is an example of a transformation performed by the aspect.

[!metalama-compare ../../tests/Metalama.Samples.Builder3.Tests/DerivedType.cs]

## Step 1. Setting up more abstractions

We will now update the aspect so that it supports two kinds of properties: standard ones, and properties of an immutable collection type. We will only support collection types of the `System.Collection.Immutable` nanmespace, but the same approach can be used to support different types.

Since we have two kinds of properties, we will make the `PropertyMapping` class _abstract_. It will have two implementations: `StandardPropertyMapping` and `ImmutableCollectionPropertyMapping`. Any implementation-specific method must be abstracted in the `PropertyMapping` class and implemented separately in derived classes.

These implementation-specific methods are the following:

* `GetBuilderPropertyValue() : IExpression` returns an expression that contains the value of the `Builder` property. The type of the expression must be the type of the property in the _source_ type, not in the builder type. For standard properties, this will return the builder property itself. For immutable collection properties, this will be the new immutable collection constructed from the immutable collection builder.
* `ImplementBuilderArtifacts()` will be called for non-inherited properties and must add declarations required to implement the property. For standard properties, this is just the public property in the `Builder` type. For immutable collections, this is more complex, and will be discussed later.
* `TryImportBuilderArtifactsFromBaseType` will be called for inherited properties and  ust find the required declarations from the base type.
* `SetBuilderPropertyValue` is the _template_, used from the copy constructor, that stores the initial value of property.

The `BuilderProperty` property of the `PropertyMapping` class now becomes an implementation detail and is removed from the abstract class.

Here is the new `PropertyMapping` class:

[!metalama-file PropertyMapping.cs]

Note that the `PropertyMapping` class now implements the (empty) `ITemplateProvider` interface. This is required because `SetBuilderPropertyValue` is a template. Note also that `SetBuilderPropertyValue` cannot be abstract because of current limitations in Metalama, so we had to make it virtual.

The implementation of `PropertyMapping` for standard properties is directly extracted from the aspect implementation in the previous article.

[!metalama-file StandardPropertyMapping.cs]

The `TryFindBuilderPropertyInBaseType` helper method is defined here:

[!metalama-file PropertyMapping.Helpers.cs member="PropertyMapping.TryFindBuilderPropertyInBaseType"]

## Step 2. Updating the aspect

Both the `BuildAspect` method and the templates must call the abstract methods and templates of `PropertyMapping`.

Let's look for instance at the code that used to create the builder properties in the `Builder` nested type. You can see how the implementation-specific logic was moved to `PropertyMapping.ImplementBuilderArtifacts` and `PropertyMapping.TryImportBuilderArtifactsFromBaseType`:

[!metalama-file GenerateBuilderAttribute.cs marker="CreateBuilderProperties"]

The aspect has been updated at several other locations. For details, please refer to the source code by following the link to GitHub on the top of this article.

## Step 3. Adding the logic specific to immutable collections

At this point, we can run the same unit tests as for the previous article, and they should execute without difference.

Let's now focus on implementing support for properties whose type is an immutable collection.

As always, we should first design a pattern at a conceptual level, and only them switch to its implementation.

To make things more complex with immutable collections, we must address the requirement that collection builders should not be allocated until the user evaluates the public property of the `Builder` type. When this property is required, we must create a collection builder from the initial collection value, which can be either empty or, if the `ToBuilder()` method was used, can be the current value in the source object.

Each property will be implemented by four artefacts:
- A field containing the property _initial value_, which can be either empty or, when initialized from the copy constructor, set to the value in the source object.
- A nullable field containing the _collection builder_.
- The public property representing the collection, which lazily instantiates the collection builder.
- A method returning the immutable collection from the collection builder if it has been defined, or returning the initial value if undefined (which means that there can be no change).

These artefacts are built by the `ImplementBuilderArtifacts` method of the `ImmutableCollectionPropertyMapping` class, and then used in other methods and templates.

[!metalama-file ImmutableCollectionPropertyMapping.cs]

## Conclusion

Handling different kinds of properties invited us to use more abstraction in our aspect. As you can see, meta-programming, just like other programming, requires a strict definition of concepts and the right level of abstraction.

Our aspect now correctly handles not only derived types, but also immutable collections.