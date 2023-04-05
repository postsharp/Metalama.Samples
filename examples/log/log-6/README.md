---
uid: sample-log-6
level: 300
---

# Logging Example: Adding the aspect programmatically

[!metalama-project-buttons .]

In the previous examples, we have applied the logging aspect manually using the `[Log]` custom attribute. But what if we must add logging to all methods of a namespace? 

You can programmatically filter the code model of your project and add aspects to desired methods by adding a <xref:Metalama.Framework.Fabrics.ProjectFabric> to your code. Fabrics are compile-time classes that get executed by Metalama during compilation or from your IDE. Fabrics can add aspects to any eligible target declaration. For details, see <xref:fabrics-adding-aspects>.

The following code adds the logging aspect to all `public` methods of `public` types.

[!metalama-file Fabric.cs]

As you can see, the `Calculator` class is transformed even if there is no longer any custom attribute.

[!metalama-compare Calculator.cs]



> [!div class="see-also"]
> <xref:fabrics-adding-aspects>

  
