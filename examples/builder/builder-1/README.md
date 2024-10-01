---
uid: sample-builder-1
summary: ""
keywords: "builder pattern C#, c# builder pattern, builder design pattern C#"
created-date: 2024-09-30
modified-date: 2024-09-30
level: 300
---

# Builder example, step 1: getting started

[!metalama-project-buttons .]

In this first article, we aim at coding the first working implementation of the Builder pattern as described in the [parent article](xref:sample-builder), but we will ignore two important features: class inheritance and immutable collections.

Our objective is to write an aspect that will perform the following transformations:

- Introduce a nested class named `Builder` with the following members:
    - A _copy constructor_ initilaizing initializing the `Builder` class from an instance of source class.
    - A _public constructor_ intended to be used by users of our clas, accepting a value for all required properties.
    - A writable property for each property of the source type.
    - A `Build` method instantiating the source type with the values set in the `Builder`, calling the `Validate` method if present.
- Add the following members to the source type:
    - A private constructor called by the `Builder.Build` method.
    - A `ToBuilder` method returning a new `Builder` initialized with the current instance.
    

Here is an illustration of the changes performed by this aspect when applied to a simple class:

[!metalama-compare ../../tests/Metalama.Samples.Builder1.Tests/SimpleExample.cs]

## 1. Setting up the infrastructure

We are about to author a complex aspects, introducing members that will have relationships with each other. Before writing any complex aspect, a bit of planning and infrastructure is required.

It's a good practice to keep the T# template logic simple and to do all the hard work in the `BuildAspect` method. A question then is how we pass data from `BuildAspect` to T# templates. As explained in <xref:sharing-state-with-advice>, a convenient approach is to use _tags_. Since we will have many tags, it will be even more convenient to use strongly-typed tags containing all data that `BuildAspect` needs to pass to templates. This is the meaning of the `Tags` record.

A critical member of the `Tags` record is a collection of `PropertyMapping` objects. The `PropertyMapping` class maps a _source_ property to a `Builder` property property, as well as to the corresponding parameter in different constructors.

Here is the source code of the `Tags` and `PropertyMapping` types.

[!metalama-file GenerateBuilderAttribute.PropertyMapping.cs]

The first thing we do in the `BuildAspect` method is to build a list of properties. When we initialize this list, we don't know yet all data items because we haven't created them yet.

[!metalama-file GenerateBuilderAttribute.cs marker="InitializeMapping"]

As you can see, we are using the <xref:System.ComponentModel.DataAnnotations.RequiredAttribute> custom attribute to determine if a property is required or optional. We chose not to use the `required` keyword because `required` properties cannot be initialized from the constructor, making the code generation more cumbersome for a first example.

Spoiler alert: here is how we share the `Tags` class with advice at the end of the `BuildAspect` method:

[!metalama-file GenerateBuilderAttribute.cs marker="SetTags"]


## 2. Creating the Builder type and the properties

Let's now create a nested type.

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceBuilder"]

Introducing properties is straightforward:

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceProperties"]

Note that we copy the `InitializerExpression` (i.e. the expression right of the `=` sign on an automatic property) from the source property to the builder property. This ensures that these properties will have the proper default value even in the `Builder` class.

The created property is then stored in the `BuilderProperty` property of the `PropertyMapping` object, so we can refer to it later.

## 3. Creating the Builder public constructor

Our next task is to create the public constructor of the `Builder` nested type, which should have parameters for all required properties.

[!metalama-file GenerateBuilderAttribute.cs marker="IntroducePublicConstructor"]

We use the `AddParameter` method to dynamically create a parameter for each required property. We save the ordinal of this parameter in the `BuilderConstructorParameterIndex` property of the `PropertyMapping` object for later reference in the constructor implementation.

Here is `BuilderConstructorTemplate`, the template for this constructor. You can now see how we use the `Tags` and `PropertyMapping` objects. This code iterates through required properties and assign a property of the `Builder` type to the value of the corresponding constructor parameter.

[!metalama-file GenerateBuilderAttribute.cs member="GenerateBuilderAttribute.BuilderConstructorTemplate"]

## 4. Implementing the Build method

The `Build` method of the `Builder` type is responsible for creating an instance of the source (immutable) type from the values of the `Builder`.

It requires a new constructor in the source type accepting a parameter for all properties. Here is how to create it:

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceSourceConstructor"]

We store the resulting constructor in a local variable, so we can pass it to the `Tags` object later on in the `BuildAspect` method.

The template for this constructor is `SourceConstructorTemplate`. It simply assigns the properties based on constructor parameters.

[!metalama-file GenerateBuilderAttribute.cs member="GenerateBuilderAttribute.SourceConstructorTemplate"]

Equipped with this constructor, we can now introduce the `Build` method:

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceBuildMethod"]

The T# template for the `Build` method first invokes the newly introduced constructor, then tries to find and call the optional `Validate` method before returning the new instance of the source type.

[!metalama-file GenerateBuilderAttribute.cs member="GenerateBuilderAttribute.BuildMethodTemplate"]

## 5. Implementing the ToBuilder method

Our last task is to add a `ToBuilder` to the source type, which must create an instance of the `Builder` type initialized with the values of the current instance.

First, we will need a new constructor in the `Builder` type, called the _copy constructor_. In theory, we could reuse the public constructor for this purpose, but the next articles of this series will be simpler if we use a copy constructor here.

Let's add this code to `BuildAspect`:

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceCopyConstructor"]

Unsurprisingly, the template of this constructor just goes through the list of `PropertyMapping` and assign the `Builder` properties from the corresponding source properties:

[!metalama-file GenerateBuilderAttribute.cs member="GenerateBuilderAttribute.BuilderCopyConstructorTemplate"]

We can finally introduce the `ToBuilder` method:

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceToBuilderMethod"]

Here is the template for the constructor body. It only invokes the constructor.

[!metalama-file GenerateBuilderAttribute.cs member="GenerateBuilderAttribute.ToBuilderMethodTemplate"]


## Conclusion

As you can see, automating the `Builder` aspect with Metalama can seem complex of the beginning, but the process can be split into individual simple tasks. It's crucial to start the work with proper analysis and planning. You should first know what you want, _how_ exactly you want to transform the code. Once this is clear in your head, the implementation is (quite) straightforward.

In the next article, we will see how to take type inheritance into account.
