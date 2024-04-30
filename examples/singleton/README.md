---
uid: sample-singleton
---

# Example: Singleton

[Singleton](https://en.wikipedia.org/wiki/Singleton_pattern) is a design pattern that requires that there is only one instance of a class in the whole application.
The singleton pattern is often used for classes that manage resources, such as a database connection or a logger. It can also be used to represent a global state, such as the configuration of the application.

Though with the advent of dependency injection and increased focus on testability, the singleton pattern has evolved as well.

In this group of examples, we will explore how Metalama can be used to improve the singleton pattern, with or without dependency injection.

## In this series

We will explore the various ways to improve the singleton pattern:

| Article | Description |
|--------|-------------|
| [Classic singleton](singleton-1/README.md) | We'll start with the classic singleton pattern, where the class has a private constructor and a public static property `Instance`. |
| [Bonus: Checking private constructor references](singleton-1b/README.md) | We'll optionally ensure that the constructor is only ever called once. |
| [Dependency injection and testing](singleton-2/README.md) | We'll discover how to improve safety for "singleton" classes that aer used with dependency injection and testing. |