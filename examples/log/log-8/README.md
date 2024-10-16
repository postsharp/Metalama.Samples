---
uid: sample-log-8
level: 300
summary: "Avoid infinite recursion in logging by using `LoggingRecursionGuard` to prevent stack overflow and performance issues."
keywords: "infinite recursion, stack overflow, performance issues, logging"
created-date: 2023-04-06
modified-date: 2024-09-09
---

# Logging example, step 8: Avoiding infinite recursion

[!metalama-project-buttons .]

Infinite recursion can occur when the logging logic of one method calls the logging logic of another method. Infinite
recursion can cause stack overflow exceptions, resulting in crashes or unintended side effects. It consumes system
resources, thus negatively affecting performance and potentially rendering the application, and any other application on
the same device, unresponsive. In addition, excessive log entries generated by recursion make it difficult to locate the
real cause of the problem, complicate log analysis, and increase storage requirements.

Therefore, infinite recursions in logging must be avoided at all costs.

To make this possible, a first step is to avoid logging the `ToString` method. However, sometimes a
non-logged `ToString` method can access logged properties or methods, and indirectly cause an infinite recursion. The
most reliable approach is to add the following code in the logging pattern:

```cs
using ( var guard = LoggingRecursionGuard.Begin() )
{
  if ( guard.CanLog )
  {
    this._logger.LogTrace( message );
  }
}
```

We can update the previous example with this new approach:

[!metalama-compare LoginService.cs]

## Infrastructure code

`LoggingRecursionGuard` uses a thread-static field to indicate whether logging is currently occurring:

[!metalama-file LoggingRecursionGuard.cs]

## Aspect code

The `LogAttribute` code has been updated as follows:

[!metalama-file LogAttribute.cs]



