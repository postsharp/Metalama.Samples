---
uid: sample-retry-5
level: 300
summary: "This document explains how to use Polly for advanced retry policies and how to integrate it with Metalama to reduce boilerplate code."
keywords: "Polly, retry policies, boilerplate code, .NET, PolicyKind, IPolicyFactory, resilience framework"
---

# Retry example, step 5: Using Polly

If you had the feeling that we were reinventing the wheel in the previous Retry examples, you were partially right.
Libraries like [Polly](https://github.com/App-vNext/Polly) offer more advanced and configurable retry policies, but even
Polly requires some boilerplate code. Wrapping the whole method body in a delegate call and adding proper logging with
parameter values entails boilerplate. With a Metalama aspect, we can completely avoid this boilerplate.

## Infrastructure code

Before jumping into the implementation, let's consider the architecture and infrastructure code.

We want the Polly policy to be centrally configurable. In a production environment, part of this configuration may be
read from an XML or JSON file. The user of this aspect will only need to specify which _kind_ of policy is required for
the target method by specifying a `PolicyKind`, a new `enum` we will define. Then the target method will obtain the
policy from our a `IPolicyFactory`, which will map the `PolicyKind` to a Polly `Policy` object. The `PolicyFactory`
implementation is supplied by the dependency injection framework. Its implementation would typically differ in a testing
or production environment.

### PolicyKind

This `enum` represents the kinds of policies that are available to business code. You can extend it at will.

[!metalama-file PolicyKind.cs]

We can imagine having several aspect custom attributes for each `PolicyKind`. You can add parameters to policies, as
long as these parameters can be implemented as properties of a custom attribute.

### IPolicyFactory

This interface is responsible for returning an instance of the `Policy` class that corresponds to the
given `PolicyKind`.

[!metalama-file IPolicyFactory.cs]

### PolicyFactory

Here is a minimalistic implementation of the `IPolicyFactory` class. You can make it as complex as necessary, but this
goes beyond the scope of this example. This class must be added to the dependency collection.

[!metalama-file PolicyFactory.cs]

## Business code

Let's compare the source business code and the transformed business code.

[!metalama-compare RemoteCalculator.cs]

## Aspect implementation

Here is the source code of the `Retry` aspect:

[!metalama-file RetryAttribute.cs]

This aspect pulls in two dependencies, `ILogger` and `IPolicyFactory`.
The <xref:Metalama.Extensions.DependencyInjection.IntroduceDependencyAttribute?text=[IntroduceDependency]> custom
attribute on top of the fields introduces the fields and pulls them from the constructor.

Polly, by design, requires the logic to be wrapped in a delegate. In a Metalama template, we achieve this only with a
local function because anonymous methods or lambdas can't contain calls to `meta.Proceed`. These local functions are
named `ExecuteCore` or `ExecuteCoreAsync`. They have an exception handler that prints a warning with the method name and
all method arguments when the method fails.

We now have the best of both worlds: a fully-featured resilience framework with Polly, and boilerplate reduction with
Metalama.


