---
uid: sample-log-2
level: 200
---

# Logging Example: Adding the method name

[!metalama-project-buttons .]

In the previous example, the aspect logged a generic message when a method started. This was not very useful if you applied the aspect to several aspects. We will now improve the aspects to include the executed method's name and signature.

The logging aspect now generates the _green_ code in the following snippet:

[!metalama-compare Calculator.cs ]

## Implementation

The aspect at work is the following:

[!metalama-file LogAttribute.cs]

We use the `meta.Target.Method` expression in a formatting string to get the method name and signature. You already know the `meta` pseudo-keyword from the previous example. It means that the expression on the right side is _magic_; by magic, we mean that it can be evaluated at compile time.

> [!div class="see-also"]
> <xref:template-overview>
  
