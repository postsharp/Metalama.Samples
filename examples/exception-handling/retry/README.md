---
uid: sample-retry
summary: "The document discusses the importance of retry mechanisms for system reliability and explains a series of articles on implementing automatic retries with features like async support, logging, and using Polly."
---

# Sample: retry

Retrying failed methods is crucial for ensuring reliability and efficiency in systems such as database transactions or online services. As temporary faults such as network congestion, hardware issues, or software glitches can cause initial failures, it is important to implement retry mechanisms with exponential backoff to increase the probability of success, minimize data loss, and provide a seamless user experience in the face of transient obstacles. Retry mechanisms enable systems to be dependable and resilient.

This series of articles describes how to construct an aspect that automatically retries a failed method. This aspect modifies a method in the following way:

[!metalama-compare retry-4/RemoteCalculator.cs]

## In this series

We start with the most basic implementation and add features progressively.

| Description | Article |
|-------------|---------|
| <xref:sample-retry-1> | This is the most basic retry aspect. |
| <xref:sample-retry-2> | In this example, we add support for `async` methods and call `await Task.Delay` instead of `Thread.Sleep`. |
| <xref:sample-retry-3> | Here, we add support for `CancellationToken` parameters, which we pass to `Task.Delay`. |
| <xref:sample-retry-4> | We now add proper logging using `ILogger` and dependency injection. |
| <xref:sample-retry-5> | Finally, we show how to use Polly instead of our custom and naïve implementation of the retry logic. |

