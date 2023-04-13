---
uid: sample-cache
---

# Example: caching

When optimizing performance, caching method outputs in memory is crucial, especially when dealing with computationally expensive or time-consuming operations. Storing the results of such methods in memory allows subsequent calls to quickly retrieve the cached data instead of re-executing the entire operation, reducing latency and enhancing overall application efficiency. 

Note that caching method outputs should only be applied to methods with no side effects. Side effects refer to any change in the state of the system or external components resulting from the method's execution. Caching methods with side effects can lead to unintended consequences by causing the application to bypass necessary updates, modifications, or validations, and instead return stale or incorrect data. To ensure the application's correctness and stability, only cache the results of pure, deterministic functions that always produce the same output given the same input, without altering any external state.

Caching the output of a method traditionally requires a lot of boilerplate code. However, we can encapsulate it in an aspect. 

One of the major challenges of caching is to generate the unique cache key reliably. In this series of articles, we present strategies to generate this key.

| Article | Description |
| ------- | ----------- |
| [Getting started](caching-1/README.md) | A first working version of the caching aspect. |
| [Enforcing eligibility](caching-2/README.md) | Makes the aspect report an error when used on `void` method or a method with `out` or `ref` parameters. |
| [Building the cache key](caching-3/README.md) | Adds a `[CacheKeyMember]` aspect that makes the specified field or property part of the cache key for that type. |
| [Supporting external types](caching-4/README.md) | Makes it possible to build a caching key that includes external types. |
