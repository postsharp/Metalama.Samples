---
uid: sample-cache
---

# Example: caching

When optimizing performance, storing method outputs in memory using a cache is crucial, particularly for computationally expensive or time-consuming tasks. Storing the results of such methods in memory enables subsequent calls to quickly retrieve the data instead of re-executing the entire operation. This method reduces latency and enhances overall application efficiency.

Note that caching method outputs should only be applied to methods with no side effects. Side effects refer to any change in the state of the system or external components resulting from the method's execution. Caching methods with side effects can lead to unintended consequences by causing the application to bypass necessary updates, modifications, or validations, and instead return stale or incorrect data. To ensure the application's correctness and stability, only cache the results of pure, deterministic functions that always produce the same output given the same input, without altering any external state.

Traditionally, caching the output of a method requires a lot of boilerplate code. However, we can significantly simplify the process by encapsulating it in an aspect.

One of the significant challenges of caching involves generating a unique cache key reliably. In this article series, we present strategies for creating this key.

| Article | Description |
| ------- | ----------- |
| [Getting started](caching-1/README.md) | Presents a first working version of the caching aspect. |
| [Enforcing eligibility](caching-2/README.md) | Makes the aspect report an error for `void` methods or methods with `out` or `ref` parameters. |
| [Building the cache key](caching-3/README.md) | Explains how to add a `[CacheKeyMember]` aspect to make the specified field or property part of the cache key for that type. |
| [Supporting external types](caching-4/README.md) | Shows how to build a caching key that includes external types. |
