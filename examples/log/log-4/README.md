---
uid: sample-log-4
level: 300
---

# Logging Example: Using ILogger

[!metalama-project-buttons .]

In the previous steps, we used the `Console.WriteLine` method to write the trace message. We will now write messages to <xref:Microsoft.Extensions.Logging.ILogger> from the `Microsoft.Extensions.Logging` namespace, and use dependency injection to get the <xref:Microsoft.Extensions.Logging.ILogger>.

Utilizing dependency injection to obtain an `ILogger` instance, rather than directly writing to `Console.WriteLine`, offers increased flexibility, maintainability, and testability. This approach allows seamless swapping of logging implementations, promotes a clean separation of concerns, simplifies configuration management, and enables effective unit testing by substituting real loggers with mock objects during testing.

Let's look at how the new aspect transforms code.

[!metalama-compare Calculator.cs ]

As you can see, calls to `Console.WriteLine` have been replaced by calls to `_logger.TraceInformation`. There is a new `ILogger` field, and its value is pulled from the constructor.

## Implementation

Let's now look at the aspect code.

[!metalama-file LogAttribute.cs]

We added some logic to handle the `ILogger` field or property.

The main point of interest in this example is the `_logger` field. The <xref:Metalama.Extensions.DependencyInjection.IntroduceDependencyAttribute?text=[IntroduceDependency]> custom attribute means that the field must be _introduced_ into the target type unless it already exists, and its value should be pulled from the constructor. This custom attribute is implemented in the `Metalama.Extensions.DependencyInjection` package. It supports several dependency injection frameworks. To read more about it, see <xref:dependency-injection>.

There are just a few changes in the `OverrideMethod` method. Instead of `Console.WriteLine`, we use the `ILogger.LogTrace` method. Note that we need to cast the `InterpolatedStringBuilder.ToValue()` expression into a string, which seems redundant because `InterpolatedStringBuilder.ToValue()` returns a `dynamic`. The reason is that `LogTrace` is an extension method, and extension methods cannot have dynamic arguments. Therefore, we must cast the `dynamic` value into a `string`, which helps the C# compiler find the correct extension method.


> [!div class="see-also"]
> <xref:dependency-injection>

  
