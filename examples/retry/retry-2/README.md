---
uid: sample-retry-2
level: 200
---

# Retry Example: Handling async methods

[!metalama-project-buttons .]

In the previous example, `async` methods were handled with the same template as plain methods. As a result, we were using a synchronous `Thread.Sleep` instead of an asynchronous `await Task.Delay`, essentially defeating the `async` nature of the original method.

This new aspect address this problem. Async methods now have their template.

[!metalama-compare RemoteCalculator.cs ]



## Implementation

The aspect provides a second template, <xref:Metalama.Framework.Aspects.OverrideMethodAspect.OverrideAsyncMethod>, which will provide the `async` implementation of the method.

[!metalama-file RetryAttribute.cs ]

The `async` template uses `await meta.ProceedAsync()` instead of `meta.Proceed()`, and `await Task.Delay` instead of `Thread.Sleep`.


## Limitations

There are still two limitations in our example:

* The aspect does not correctly handle `CancellationToken`.
* The logging is too basic and hardcoded to `Console.WriteLine`.


> [!div class="see-also"]
> <xref:simple-override-method>
> <xref:quickstart-adding-aspects>
  

  
