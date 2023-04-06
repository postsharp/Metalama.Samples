---
uid: sample-retry
---

# Sample: retry

Retrying failed methods is crucial for ensuring reliability and efficiency in systems like database transactions or online services. Temporary issues such as network congestion, hardware hiccups, or software glitches can cause initial failures. By implementing retries with exponential backoff, we can increase the chances of success, minimize data loss, and provide a seamless user experience despite transient obstacles. Ultimately, retry mechanisms contribute to resilient and dependable systems.

In this series of articles, we describe how to build an aspect that automatically retries the execution of a method when it fails.

For instance, it may perform the following transformation to a method:

[!metalama-compare retry-4/RemoteCalculator.cs ]

## In this series

We start from the most trivial implementation and progressively add features:

| Description | Article |
|-----------|-----------|
| <xref:sample-retry-1> | This is the most basic retry aspect. |
| <xref:sample-retry-2> | In this example, we add support for `async` methods and call `await Task.Delay` instead of `Thread.Sleep`. |
| <xref:sample-retry-3> | Here, we add support for `CancellationToken` parameters, and pass it to `Task.Delay` when we have some. |
| <xref:sample-retry-4> | We now add proper logging using `ILogger` and dependency injection. |
| <xref:sample-retry-5> | Finally, show how to use Polly instead of our custom and naive implemention of the retry logic. |