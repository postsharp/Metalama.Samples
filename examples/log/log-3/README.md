---
uid: sample-log-3
level: 250
---

# Example: Logging parameters values

[!metalama-project-buttons .]

So far, our logging aspect writes messages that include _constant_ text and _compile-time_ expressions. Let's now add the values of parameters and the method return value, which are not known at compile time. We want our aspect to generate an _interpolated string_ that includes parameter values.

Here is the code transformed by the new aspect:

[!metalama-compare Calculator.cs ]

## Implementation

The aspect at work is the following:

[!metalama-file LogAttribute.cs]

As you can see, the aspect's code is much more difficult.

To create an interpolated string from an aspect, the most straightforward approach is to use the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> class, and to add literal parts, known at compile time and constant at run time, and run-time expressions, unknown at compile time.

The `BuildInterpolatedString` method of the aspect class is responsible for creating the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>. Note that `BuildInterpolatedString` is _not_ a template method. It is a method that is wholly executed at compile time. It has an `includeOutParameters` parameter that determines whether the values of the `out` parameters are available at the point where the interpolated string is used.

* First, `BuildInterpolatedString` appends the name of the current type and method using the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder.AddText*> method. Then, it iterates through the collection of parameters of the current method. This collection is available on the `meta.Target.Parameters` expression. 

* In the `foreach` loop, the `BuildInterpolatedString` method checks if the parameter is `out`. If the `includeOutParameters` parameter is `false`, the method appends a constant text. But if the parameter can be read, the method appends an expression using the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder.AddExpression*> method.

Let's now look at the `OverrideMethod` method. As in previous examples, this method is a template containing both run-time and compile-time code.

* First, `OverrideMethod` calls `BuildInterpolatedString` to get the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>. Note that the interpolated string built by `BuildInterpolatedString` only includes the name and parameters of the method. We still want to append more information to these strings, such as the text `started`, `succeeded`, `returned` or `failed`. 

* <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> is a compile-time object. To generate the interpolated string from it, we call the <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilderExtensions.ToValue*> method. To be precise, the <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilderExtensions.ToValue*> method does not generate an interpolated string but returns a run-time object of `dynamic` type representing the interpolated string.

* Note that `BuildInterpolatedString` writes a different message when the method is `void` than when it returns a value. We implement this choice with the `if` in the method. Because the `if` condition, `meta.Target.Method.ReturnType.Is( typeof(void) )`, is a compile-time expression, Metalama interprets the whole `if` at compile time. Notice that the compile-time has a particular background color.

> [!div class="see-also"]
> <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>
> <xref:template-compile-time>
> <xref:template-dynamic-code>
  