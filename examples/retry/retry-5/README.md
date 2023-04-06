---
uid: sample-retry-5
level: 300
---

# Retry Example: Using Polly

If you had the feeling that we were reinventing the wheel in the previous Retry examples, you were partially right. Indeed, libraries like [Polly](https://github.com/App-vNext/Polly) have far more advanced and configurable retry policies. However, even Polly requires some boilerplate because the whole method body must be wrapped in a delegate call. Adding proper logging with parameter values requires even more boilerplate, which we can completely avoid with a Metalama aspect.

## Infrastructure code

Before jumping into the implementation, let's think a minute about the architecture and the infrastructure code.

We want the Polly policy to be centrally configurable. In a production environment, part of this configuration may be read from an XML or JSON file. The user of this aspect will only need to specify which _kind_ of policy is required for the target method by specifying a `PolicyKind`, a new `enum` we will define. Then the target method will obtain the policy from our a `IPolicyFactory`, which will map the `PolicyKind` to a Polly `Policy` object. The `PolicyFactory` implementation is supplied by the dependency injection framework. Its implementation would typically differ in a testing or production environment.

### PolicyKind

This `enum` represents the kinds of policies that are available to business code. You can extend it at will.

[!metalama-file PolicyKind.cs]

We can imagine having several aspect custom attributes, one for each `PolicyKind`. You can add parameters to policies, as long as these parameters can be implemented as properties of a custom attribute.

### IPolicyFactory

This interface is responsible for returning an instance of the `Policy` class that corresponds to the given `PolicyKind`.

[!metalama-file IPolicyFactory.cs]

### PolicyFactory

Here is a minimalistic implementation of the `IPolicyFactory` class. You can make it as complex as necessary, but this goes beyond the scope of this example. This class must be added to the dependency collection.

[!metalama-file PolicyFactory.cs]

## Business code

Let's now compare the source business code and the transformed business code.

[!metalama-compare RemoteCalculator.cs]

## Aspect implementation

Here is the source code of the `Retry` aspect:

[!metalama-file RetryAttribute.cs]

We pull two dependencies into this aspect: `ILogger` and `IPolicyFactory`. The <xref:Metalama.Extensions.DependencyInjection.IntroduceDependencyAttribute?text=[IntroduceDependency]> on the top of the fields do the magic of introducing the fields and pulling them from the constructor.

Polly, by design, requires the logic to be wrapped in a delegate. In a Metalama template, we can achieve this only with a local function because anonymous methods or lambdas cannot contain calls to `meta.Proceed`. We name these local functions `ExecuteCore` or `ExecuteCoreAsync`. In these local functions, we have an exception handler that prints a warning with the method name and all method arguments when the method fails.

We now have the best of both worlds: a fully-featured resilience framework with Polly, and boilerplate reduction with Metalana.