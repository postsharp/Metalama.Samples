---
uid: sample-log-103
---

# Example: Logging parameters values

[!metalama-project-buttons .]

So far, our logging aspect writes messages that include _constant_ text and _compile-time_ expressions. Let's now add the values of parameters and the method return value, which are not known at compile time. We want our aspect to generate an _interpolated string_ that includes parameter values.

Here is the code transformed by the new aspect:

[!metalama-file Program.cs transformed]

The aspect at work is the following:

[!metalama-file LogAttribute.cs]

As you can see, the code of the aspect is much more difficult.

To create an interpolated string from an aspect, the simplest approach is to use the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> class, and to add literal parts, which are known at compile time and constant at run time, and run-time expressions.

The `BuildInterpolatedString` method of the aspect class is responsible for creating the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>. Note that `BuildInterpolatedString` is _not_ a template. It is a method that is completely executed at compile time.

* First, `BuildInterpolatedString` appends the name of the current type and method using the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder.AddText*> method. Then, it iterates through the collection of parameters of the current method. This collection is available on the `meta.Target.Parameters` expression. 

* Then, within the `foreach` loop, the `BuildInterpolatedString` method checks if the parameter is `out`. In this case, it does not try to add the parameter to the interpolated string (such code would not compile), but appends a constant text instead. But if the parameter can be read, it appends an expression using the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder.AddExpression*> method.

Let's now look at the `OverrideMethod` method. As in previous examples, this method is a template, so it contains both run-time and compile-time code.

* First, `OverrideMethod` calls `BuildInterpolatedString` to get the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>. Note that the interpolated string built by `BuildInterpolatedString` only includes the name and parameters of the method. We still want to append more information to these strings, such as the text `started`, `succeeded`, `returned` or `failed`. We build these 4 messages by first cloning the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> returned by `BuildInterpolatedString`, and then by appending the specific texts and expressions.

* <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> is a compile-time object. To generate the interpolated string from it, we call the <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilderExtensions.ToValue*> method. To be precise, the <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilderExtensions.ToValue*> method does not generate an interpolated string, but returns a run-time object of `dynamic` type that represents the interpolated string.

* Note that `BuildInterpolatedString` writes a different message when the method is `void` than when it returns a value. We implement this choice with the `if` in the method. Because the `if` condition, `meta.Target.Method.ReturnType.Is( typeof(void) )`, is a compile-time expression, Metalama interprets the whole `if` at compile time. Notice that the compile-time is has a special background color.

> [!div class="see-also"]
> <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>
> <xref:template-compile-time>
> <xref:template-dynamic-code>