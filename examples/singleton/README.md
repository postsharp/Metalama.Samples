---
uid: sample-singleton
---

# Example: Singleton

[Singleton](https://en.wikipedia.org/wiki/Singleton_pattern) is a design pattern that requires that there is only one instance of a class in the whole application. A singleton class also usually provides a global point of access to that instance.

The singleton pattern is often used for classes that manage resources, such as a database connection or a logger. It can also be used to represent a global state, such as the configuration of the application.

Configuration manager is a good case for the singleton pattern, because it should be consistent across the whole application and it can be expensive to load. A configuration manager singleton can look like this:

```c#
public sealed class ConfigurationManager
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

Notice that we have a private constructor, to ensure that the class cannot be instantiated from the outside, and a public static property `Instance` that returns the single instance of the class. (The instance is created when the class is first used.)

If you use the singleton pattern regularly, you might start noticing several issues with this code:

1. Clarity. It is not immeditelly clear that the type is a singleton.
2. Consistency. The singleton pattern can be implemented in slightly different ways (for example, the `Instance` property could have a different name or it could be a method instead), making the code harder to understand.
3. Boilerplate. Each singleton class uses the same code. This is tedious and could lead to bugs due to inattention.
4. Safety. There is nothing preventing someone from making the constructor public and then creating multiple instances of the class. It might even be necessary to make the constructor public for testing purposes.

## Clarity

To address the lack of clarity, we could create a very simple attribute to mark singleton classes:

```c#
[AttributeUsage(AttributeTargets.Class)]
public sealed class SingletonAttribute : Attribute;

[Singleton]
public sealed class MySingleton
{
    // the rest of the code
}
```

This attribute on its own doesn't achieve much, so it wouldn't be worth adding it to your codebase. That changes with the following steps.

## In this series

We will explore the various ways to improve the singleton pattern:

| Article | Description |
|--------|-------------|
| [Ensuring consistency and avoiding boilerplate](singleton-1/README.md) | We'll turn the `SingletonAttribute` into a Metalama aspect, which will implement the singleton pattern for us by introducing the `Instance` property. |
| [Simple safety](singleton-2/README.md) | We'll ensure that the constructor is private by emitting a warning. |
| [Detour: Checking constructor references](singleton-2b/README.md) | We'll optionally ensure that the constructor is only ever called once by using Metalama infrastructure for architecture validation. |
| [Advanced safety](singleton-3/README.md) | We'll allow multiple instances of the singleton class for testing purposes, while ensuring that the singleton pattern is still enforced in production code. |