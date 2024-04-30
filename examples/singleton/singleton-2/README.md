---
uid: sample-singleton-2
---

# Singleton example, version 2: Singleton for dependency injection and testing

[!metalama-project-buttons .]

In many cases, the regular singleton pattern is not suitable, either because we want to inject some dependency into the singleton, or because we want to allow multiple instances of the singleton for testing purposes. In these cases, a public static property `Instance` is not appropriate, and the constructor cannot be private.

A configuration manager "singleton" can look like this:

```c#
public sealed class ConfigurationManager : IConfigurationManager
{
    private FrozenDictionary<string, string> dictionary;

    public ConfigurationManager(IConfigurationSource configurationSource)
    {
        dictionary = configurationSource.LoadConfiguration();
    }

    public string GetValue(string key) => dictionary[key];
}
```

When using Microsoft.Extensions.DependencyInjection, this class would registered as a singleton: `services.AddSingleton<IConfigurationManager, ConfigurationManager>()`. In production code, the `IConfigurationSource` could read the configuration for example from a file. For testing, we would use a mock `IConfigurationSource` that returns a predefined configuration, which comes from a hard-coded dictionary or string.

In contrast with the classic singleton pattern, code like the one above does not have issues with boilerplate code or consistency. But it does have a big problem with safety: there is nothing preventing someone from creating multiple instances of the class in production code, by directly calling the constructor.

To prevent that, we can use Metalama architecture validation (see <xref:validating-usage>) and allow the constructor to be called only from test code by programmaticaly adding the <xref:Metalama.Extensions.Architecture.Aspects.CanOnlyBeUsedFromAttribute> aspect.

[!metalama-test ../../tests/Metalama.Samples.Singleton2.Tests/Singleton.cs]

We do this by creating a Metalama <xref:Metalama.Framework.Aspects.TypeAspect>. In its `BuildAspect` method, we first access the <xref:Metalama.Framework.Aspects.IAspectBuilder`1.Outbound?text=builder.Outbound> property, which allows adding other aspects from an aspect, among other things. We then select constructors of the type using LINQ-like syntax. Finally, we add the <xref:Metalama.Extensions.Architecture.Aspects.CanOnlyBeUsedFromAttribute> aspect to the constructors, with the `Namespaces` property set to a pattern `**.Tests`, which matches all namespaces that end in `.Tests`.