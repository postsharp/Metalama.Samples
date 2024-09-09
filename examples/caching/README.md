---
uid: sample-cache
summary: "This chapters explains how to leverage aspect-oriented programming to cache method outputs and improve performance for computationally expensive tasks."
---

# Example: caching

In optimizing performance, it is important to store method outputs in memory to improve overall application efficiency, especially for time-consuming or computationally expensive tasks. By storing the results in memory, subsequent calls can quickly retrieve the data without having to re-execute the entire operation, reducing latency.

It is important to note that caching method outputs should only be applied to methods without side effects. Side effects are any changes made to the state of the system or external components resulting from method execution. Caching methods with side effects can cause unintended consequences that bypass necessary updates, modifications, or validations, resulting in stale or incorrect data, which can impact application stability and correctness. To ensure the application's stability and correctness, only cache the results of pure, deterministic functions that always produce the same output given the same input, without altering any external state.

Traditionally, caching the output of a method requires a lot of boilerplate code. However, this process can be significantly simplified by encapsulating it in an aspect.

One of the significant challenges of caching is generating a unique cache key reliably. In this article series, we present strategies for creating this key.

| Article | Description |
| ------- | ----------- |
| [Getting started](caching-1/README.md) | Presents a first working version of the caching aspect. |
| [Enforcing eligibility](caching-2/README.md) | Makes the aspect report an error for `void` methods or methods with `out` or `ref` parameters. |
| [Building the cache key](caching-3/README.md) | Explains how to add a `[CacheKeyMember]` aspect to make the specified field or property part of the cache key for that type. |
| [Supporting external types](caching-4/README.md) | Shows how to build a caching key that includes external types. |

