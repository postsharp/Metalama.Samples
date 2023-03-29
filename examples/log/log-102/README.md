---
uid: sample-log-102
---

# Example: Logging the method name

[!metalama-project-buttons .]

In the previous example, the aspect logged a generic message when a method started. This was not very useful if you apply the aspect to several aspects. We will now improve the aspects so that it includes the name and signature of the executed method.

The logging aspect now generates the _green_ code in the following snippet:

[!metalama-compare Calculator.cs ]

## Implementation

The aspect at work is the following:

[!metalama-file LogAttribute.cs]

To get the method name and signature, we use the `meta.Target.Method` expression in a formatting string. You already know the `meta` pseudo-keyword from the previous example. It means that the expression on the right side is _magic_ and by magic, we mean that it can be evaluated at compile time.

> [!div class="see-also"]
> <xref:template-overview>