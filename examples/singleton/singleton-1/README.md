---
uid: sample-singleton-1
---

# Singleton example, version 1: Classic singleton

[!metalama-project-buttons .]

The "classic" version of the singleton pattern in C# is a class with a private constructor, to ensure that the class cannot be instantiated from the outside, and a public static property `Instance` that returns the single instance of the class. The class may or may not implement an interface, depending on the use case.

Configuration manager is a good use case for the singleton pattern, because it should be consistent across the whole application and it can be expensive to load. A configuration manager singleton can look like this:

```c#
public sealed class ConfigurationManager : IConfigurationManager
{
    private FrozenDictionary<string, string> dictionary;

    private ConfigurationManager()
    {
        dictionary = LoadConfiguration();
    }

    public static ConfigurationManager Instance { get; } = new();

    public string GetValue(string key) => dictionary[key];

    private static FrozenDictionary<string, string> LoadConfiguration()
    {
        // load the configuration from somewhere
    }
}
```

While this version of singleton guarantees that there is never more than one instance of the class in the whole application, it is not testable and its use with dependency injection is limited: we can register the singleton in the DI container using code like `services.AddSingleton(ConfigurationManager.Instance)` or `services.AddSingleton<IConfigurationManager>(ConfigurationManager.Instance)`, but the singleton can't depend on other services. See <xref:sample-singleton-2> for a version of the singleton pattern that is more suitable for dependency injection and testing.

If you use the singleton pattern regularly, you might start noticing several issues with this code:

1. Clarity. It is not immeditelly clear that the type is a singleton.
2. Consistency. The singleton pattern can be implemented in slightly different ways (for example, the `Instance` property could have a different name or it could be a method instead), making the code harder to understand.
3. Boilerplate. Each singleton class uses the same code. This is tedious and could lead to bugs due to inattention.
4. Safety. There is nothing preventing someone from making the constructor public and then creating multiple instances of the class.

## Step 1: Clarity

To address the lack of clarity, we could create a very simple Metalama aspect to mark singleton classes:

```c#
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

class SingletonAttribute : TypeAspect
{
    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        base.BuildAspect(builder);
    }
}

[Singleton]
public sealed class ConfigurationManager
{
    // the rest of the code
}
```

An aspect is an attribute that can be applied to some part of the code (in this case, a type, because the aspect inherits from <xref:Metalama.Framework.Aspects.TypeAspect>) to modify it in some way. We do that by adding code to the `BuildAspect` method (or by adding some special members to the aspect class). Since we haven't done that yet, this aspect doesn't actually do anything. That changes with the following steps.

## Step 2: Consistency and boilerplate

To ensure consistency and avoid boilerplate code, we'll add code to `SingletonAttribute`, which will implement the repetitive part of the singleton pattern for us by introducing the `Instance` property (see <xref:introducing-members>), with an initializer that invokes the constructor.

To do this, we first add a template property to the aspect class, which outlines the shape of the `Instance` property (we can't reference the type of the singleton here, so we use `object` as the property type instead and replace it later):

[!metalama-file SingletonAttribute.cs marker="InstanceTemplate"]

Then we add code to the `BuildAspect` method to actually introduce the `Instance` property:

[!metalama-file SingletonAttribute.cs marker="IntroduceInstanceProperty"]

Here, we call <xref:Metalama.Framework.Advising.IAdviceFactory.IntroduceProperty*>, specifyig the type into which the property should be introduced, the name of the template and a lambda that is used to customize the property further. Inside the lambda, we replace the `object` type with the actual type of the singleton class and set the initializer to invoke the constructor. We use <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilder> to build the expression that calls the constructor, including the <xref:Metalama.Framework.Code.SyntaxBuilders.SyntaxBuilder.AppendTypeName(Metalama.Framework.Code.IType)> method, which ensures that the type name is correctly formatted.

The resulting singleton class is a bit simpler and doing this automatically ensures that all singletons in the codebase are implemented in the same way:

[!metalama-compare MySingleton.cs]

## Step 3: Safety

To ensure safety, we can verify that the constructor is private and produce a warning (see <xref:diagnostics>) if it isn't. To do this, we first add a definition of the warning as a `static` field to the aspect class:

[!metalama-file SingletonAttribute.cs marker="PrivateConstructorDiagnostic"]

The type of the field is <xref:Metalama.Framework.Diagnostics.DiagnosticDefinition`1>, where the type argument specifies types of parameters used in the diagnostic message as a tuple. The message uses the same format string syntax as the <xref:System.String.Format*?displayProperty=nameWithType> method.

We then add code to the `BuildAspect` method to check if the constructor is private and produce a warning if it isn't:

[!metalama-file SingletonAttribute.cs marker="PrivateConstructorReport"]

To do this, we iterate over all constructors of the type (in case there are multiple, though usually there is only one), check the <xref:Metalama.Framework.Code.IMemberOrNamedType.Accessibility> for each of them, and then report the warning specified above if the accessibility is not <xref:Metalama.Framework.Code.Accessibility.Private>. We specify the formatting arguments for the diagnostic message as a tuple using the <xref:Metalama.Framework.Diagnostics.DiagnosticDefinition`1.WithArguments(`0)> method. We also set the location of the diagnostic to the constructor, otherwise the warning would be reported at the type level, because we're reporting it through the <xref:Metalama.Framework.Aspects.IAspectBuilder`1> for the singleton type.

[!metalama-test ../../tests/Metalama.Samples.Singleton1.Tests/Singleton.cs tabs="target"]

## Aspect implementation

The full aspect implementation is below:

[!metalama-file SingletonAttribute.cs]