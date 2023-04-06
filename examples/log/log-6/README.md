---
uid: sample-log-6
level: 300
---

# Logging Example: Adding the aspect programmatically

[!metalama-project-buttons .]

In the previous examples, we have applied the logging aspect manually using the `[Log]` custom attribute. But if we must add logging to all methods of a namespace, doing it manually would be daunting.

You can programmatically filter the code model of your project and add aspects to desired methods by adding a <xref:Metalama.Framework.Fabrics.ProjectFabric> to your code. Fabrics are compile-time classes that get executed by Metalama during compilation or from your IDE. Fabrics can add aspects to any eligible target declaration. For details, see <xref:fabrics-adding-aspects>.

The following code adds the logging aspect to all `public` methods of `public` types.

[!metalama-file Fabric.cs]

> [!WARNING]
> It is important to exclude the `ToString` method from logging, otherwise an infinite recursion could be created.


As you can see, the `Calculator` class is transformed even if there is no longer any custom attribute.

[!metalama-compare Calculator.cs]

> [!WARNING]
> Including sensitive information (e.g., user credentials, personal data, etc.) in logs can pose a security risk. Be cautious when adding parameter values to logs and avoid exposing sensitive data.
> To remove sensitive information from the logs, see <xref:sample-log-7>


> [!div class="see-also"]
> <xref:fabrics-adding-aspects>

  
