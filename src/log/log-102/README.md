---
uid: sample-log-102
---

# Sample: Logging the method name

In the previous example, the aspect logged a generic message when a method started. This was not very useful if you apply the aspect to several aspects. We will now improve the aspects so that it includes the name and signature of the executed method.

The resulting code is the following:

[!metalama-file Program.cs transformed]

The aspect at work is the following:

[!metalama-file LogAttribute.cs]

To get the method name and signature, we use the `meta.Target.Method` expression in a formatting string. You already know the `meta` pseudo-keyword from the previous example. It means that the expression on the right side is _magic_ and by magic, we mean that it can be evaluated at compile time.

