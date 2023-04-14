---
uid: sample-log-6
level: 300
---

# Logging example, step 6: Adding the aspect programmatically

[!metalama-project-buttons .]

In the previous examples, we applied the logging aspect manually using the `[Log]` custom attribute. But when we must add logging to all methods of a namespace, doing it manually becomes tedious.

We can programmatically filter the code model of our project and add aspects to desired methods by adding a <xref:Metalama.Framework.Fabrics.ProjectFabric> to our code. Fabrics are compile-time classes executed by Metalama during compilation or from our IDE. They can add aspects to any eligible target declaration. For details, see <xref:fabrics-adding-aspects>.

The following code adds the logging aspect to all `public` methods of `public` types:

[!metalama-file Fabric.cs]

> [!WARNING]
> It is important to exclude the `ToString` method from logging; otherwise, infinite recursion could occur.


We can see that the `Calculator` class has been transformed, even though there is no longer any custom attribute:

[!metalama-compare Calculator.cs]

> [!WARNING]
> Including sensitive information (such as user credentials or personal data) in logs can pose a security risk. You should be cautious when adding parameter values to logs and avoid exposing sensitive data.
> To remove sensitive information from the logs, see <xref:sample-log-7>.

> [!div class="see-also"]
> <xref:fabrics-adding-aspects>
