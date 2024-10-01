---
uid: sample-builder-2
summary: ""
keywords: "builder pattern C#, c# builder pattern, builder design pattern C#"
created-date: 2024-09-30
modified-date: 2024-09-30
level: 300
---

# Builder example, step 2: Handling derived types

[!metalama-project-buttons .]

In the previous article, we assumed that the type hierarchy was flat. Now, we will take type inheritance into account, handling the case where the base type already has a builder.

Our objective is to generate code like in the following example, where the `WebArticle` class derives from `Article`. See how `WebArticle.Builder` derives from `Article.Builder` and how constructors of `WebArticle` call the `base` constructors of `Article`.

[!metalama-compare ../../tests/Metalama.Samples.Builder2.Tests/DerivedType.cs]


## Step 1. Preparing to report errors

A general best practice when implementing patterns using an aspect is to consider the case where the pattern has been implemented _manually_ on the base case, and to report errors when hand-written code does not respect the conventions we have set for the patterns. For instance, the previous articles has set some rules regarding the generation of constructors. In this article, the aspect will have to assume that the base types (both the base source type and the base builder type) define the expected constructors. Otherwise, we will report an error. It's always better for the user than throwing an exception.

Before reporting any error, we must declare a <xref:Metalama.Framework.Diagnostics.DiagnosticDefinition> static field for each type of error.

[!metalama-file BuilderDiagnosticDefinitions.cs]


## Step 2. Finding the base type and its members

We can now inspect the base type and look for artifacts we will need: the constructors, the `Builder` type, and the constructors of the `Builder` type. If we don't find them, we report an error and quit. 

[!metalama-file GenerateBuilderAttribute.cs marker="FindBaseType"]

## Step 3. Creating the Builder type

Now that we found the artifacts in the base type, we can update the rest of the `BuildAspect` method to use them.

In the snippet that creates the `Builder` type, we specify the `Builder` of the base type as the base type of the new `Builder`:

[!metalama-file GenerateBuilderAttribute.cs marker="IntroduceBuilderType"]

Note that we set the `whenExist` parameter to `OverrideStrategy.New`. This means that we will generate `new` class if the base type already contains a `Builder` class.

## Step 4. Mapping properties

To discover properties, we now use the `AllProperties` collection which, unlike `Properties`, includes properties defined by base types. We added an `IsInherited` property into the `PropertyMapping` field.

Here is how we updated the code that discovers properties.

[!metalama-file GenerateBuilderAttribute.cs marker="CreatePropertyMap"]

The code that creates properties must be updated too. We don't have to create builder properties for properties of the base type, since these properties should already be defined in the base builder type. If we don't find such property, we report an error.

[!metalama-file GenerateBuilderAttribute.cs marker="CreateProperties"]

Note that we could do more validation, such as checking the property type and its visibility.

## Step 5. Updating constructors

All constructors must be updated to call the `base` constructor. Let's demonstrate the technique with the public constructor of the `Builder` class.

Here is the updated code:

[!metalama-file GenerateBuilderAttribute.cs marker="CreateBuilderConstructor"]

The first part of the logic is unchanged: we add a parameter for each required property, including inherited ones. Then, when we have a base class, we call the base constructor. First, we set the `InitializerKind` of the new constructor to `Base`. Then, for each parameter of the base constructor, we find the corresponding parameter in the new constructor, and we call the `AddInitializerArgument` method to add an argument to the call to the `base()` constructor. If we don't find this parameter, we report an error.


## Step 6. Other changes

Other parts of the `BuildAspect` method and most templates must be updated to take inherited properties into account. Please refer to the source code of the example on GitHub for details (see the links on the top of this article).

## Conclusion

Handling type inheritance is generally not a trivial task because you have to take into account the possibility that the base type does not define the expected declarations. Reporting errors is always better than failing with an exception, and of course than generating invalid code.

In the next article, we will see how to handle properties whose type is an immutable collection.
