---
uid: sample-singleton
---

# Example: Singleton

The Singleton pattern is beneficial in scenarios where a class in a software system should have only one instance available to all components.

Singleton can be instrumental in maintaining consistency across the system -- whether it involves synchronized access to a shared resource (e.g., through locking) or ensuring data consistency. In these cases, it is crucial to have only a single instance of the object. Another scenario where the Singleton pattern can be beneficial is when creating multiple instances of a complex object might be resource-intensive. If a single instance suffices for the application’s needs, a Singleton can reduce overhead in terms of both memory and processing.

The classic implementation of the Singleton pattern involves having a private constructor in the class, forbidding it from being instantiated from outside, and exposing a single instance on a static member.

With the advent of dependency injection and unit testing, the Singleton pattern has evolved. To allow several unit tests to be executed concurrently but in isolation from each other, we provide a new instance of each singleton to each unit test.

In this group of examples, we will explore how Metalama can be used to improve the Singleton pattern, with or without dependency injection.

## In this series

We will explore the various ways to improve the Singleton pattern:

| Article                                                                  | Description                                                                                                                        |
|--------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| [Classic singleton](singleton-1/README.md)                               | We'll start with the classic Singleton pattern, where the class has a private constructor and a public static property `Instance`. |
| [Modern singleton](singleton-2/README.md)                | We'll discover how to improve safety for "singleton" classes that are used with dependency injection and testing.                  |
