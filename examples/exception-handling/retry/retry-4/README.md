---
uid: sample-retry-4
level: 300
---

# Retry Example: Adding logging

[!metalama-project-buttons .]

Let's now add real-world logging to the retry aspect. We have covered logging extensively in <xref:sample-log>, so we will not go into great detail in this article.

The code transformation is now the following:

[!metalama-compare RemoteCalculator.cs ]

The code now produces the following output:

```
Trying for the 1-th time.
warn: RemoteCalculator[0]
      RemoteCalculator.Add(a = {1}, b = {1}) has failed: Operation is not valid due to the current state of the object. Retrying in 200 ms.
trce: RemoteCalculator[0]
      RemoteCalculator.Add(a = {1}, b = {1}): retrying now.
Trying for the 2-th time.
warn: RemoteCalculator[0]
      RemoteCalculator.Add(a = {1}, b = {1}) has failed: Operation is not valid due to the current state of the object. Retrying in 400 ms.
trce: RemoteCalculator[0]
      RemoteCalculator.Add(a = {1}, b = {1}): retrying now.
Trying for the 3-th time.
warn: RemoteCalculator[0]
      RemoteCalculator.Add(a = {1}, b = {1}) has failed: Operation is not valid due to the current state of the object. Retrying in 800 ms.
trce: RemoteCalculator[0]
      RemoteCalculator.Add(a = {1}, b = {1}): retrying now.
Trying for the 4-th time.
Succeeded
```

> [!WARNING]
> Including sensitive information (e.g., user credentials, personal data, etc.) in logs can pose a security risk. Be cautious when adding parameter values to logs and avoid exposing sensitive data.
> To remove sensitive information from the logs, see <xref:sample-log-7>

## Implementation

The first improvement is that the logging message now includes the method name and the parameter values. The logic is hidden in the `LoggingHelper.BuildInterpolatedString` method. To learn about how we built this method, read <xref:sample-log-2> and <xref:sample-log-3>.

Then, we replaced `Console.WriteLine` by `ILogger` and we are using dependency injection to pull the `ILogger` into the target class. For details, see <xref:sample-log-4>.

The aspect code is now the following:

[!metalama-file RetryAttribute.cs ]


We will address these limitations in the next examples.

> [!div class="see-also"]
> <xref:sample-log-2>
> <xref:sample-log-3>
> <xref:sample-log-4>
> <xref:template-overview>
> <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>
> <xref:template-compile-time>
> <xref:template-dynamic-code>
    
  
