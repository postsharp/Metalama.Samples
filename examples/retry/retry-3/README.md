---
uid: sample-retry-3
level: 200
---

# Example: Handling cancellation tokens

[!metalama-project-buttons .]

Now that we properly handle async methods, our next goal is to handle the <xref:System.Threading.CancellationToken> parameters. When we have a cancellation token, we need to pass it to the call to Task.Delay.

[!metalama-compare RemoteCalculator.cs ]


## Implementation

In this example, we modified the `OverrideMethodAspect` template as follows:

[!metalama-file RetryAttribute.cs ]

Let's look at the first lines:

```cs
var cancellationTokenParameter 
    = meta.Target.Parameters.Where( p => p.Type.Is( typeof( CancellationToken ) ) ).LastOrDefault();
```

It assigns a compile-time local variable to the last parameter of type `CancellationToken`, or null if there is none. `meta.Target.Parameters` is a compile-time expression that gives access to the parameters of the method to which the template is applied. The rest of the expression is compile-time by transitivity. Therefore, the local variable is also compile-time.

We use this compile-time variable to choose which overload of `Task.Delay` we have to call. The `if` statement is executed at compile time because the expression, `cancellationTokenParameter != null`, is fully known at compile time. `cancellationTokenParameter.Value` gives the run-time value of the parameter, i.e. it typically translate to `cancellationToken` in the generated code.

```cs
if ( cancellationTokenParameter != null )
{
      await Task.Delay( (int) delay, cancellationTokenParameter.Value );
}
else
{
      await Task.Delay( (int) delay );
}
```



## Limitations

There are still two limitations in our example:

* The logging is too basic and hardcoded to `Console.WriteLine`.


> [!div class="see-also"]
> <xref:template-compile-time>
> <xref:template-dynamic-code>

  
