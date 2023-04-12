---
uid: sample-cache
---

# Example: caching

Caching method outputs in memory is important for optimizing performance, especially when dealing with computationally expensive or time-consuming operations. By storing the results of such methods in memory, subsequent calls can quickly retrieve the cached data instead of re-executing the entire operation. This approach reduces latency and system resource usage, enhancing overall application efficiency. Employing in-memory caching is particularly important when accessing external resources like databases, web services, or complex calculations that may strain system performance or impose significant delays.

Note that caching method outputs should be applied exclusively to methods with no side effects. Side effects refer to any change in the state of the system or external components resulting from the method's execution. Caching methods with side effects can lead to unintended consequences, as it may cause the application to bypass necessary updates, modifications, or validations, and instead return stale or incorrect data. To ensure the application's correctness and stability, only cache the results of pure, deterministic functions, which always produce the same output given the same input and do not alter any external state.

Caching the output of a method traditionally requires a lot of boilerplate code. Fortunately, this can be encapsulated in an aspect.

One of the major challenges of caching is to reliably generate the unique cache key. In this series of articles, we will present strategies to generate this key.

| Article | Description |
| [Getting started](caching-1/README.md) | A first working version of the caching aspect. |
| [Enforcing eligibility](caching-2/README.md) | Makes the aspect report an error when used on `void` method or a method with `out` or `ref` parameters. |
| [Building the cache key](caching-3/README.md) | Adds an aspect `[CacheKeyMember]` that makes the field or property part of the cache key for that type. |
| [Supporting external types](caching-4/README.md) | Makes it possible to build a caching key that include external types |