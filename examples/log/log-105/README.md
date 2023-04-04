---
uid: sample-log-105
---

# Example: ILogger without dependency injection

[!metalama-project-buttons .]

Let's do a step back. In the previous previous example, we used the heavy magic of the <xref:Metalama.Extensions.DependencyInjection.IntroduceDependencyAttribute?text=[IntroduceDependency]> custom attribute. What if you want your aspect to rely on an _existing_ `ILogger` field, and report errors if the field does not exist?

[!metalama-compare Calculator.cs ]

Unlike the previous example, the source code now contains an `ILogger` field, initialized from the constructor. However, we need to make sure that the aspect reports the helpful error messages when the field is missing or has an incorrect type.


[!metalama-file ../../tests/Metalama.Samples.Log105.Tests/MissingFieldOrProperty.cs]

[!metalama-file ../../tests/Metalama.Samples.Log105.Tests/FieldOfWrongType.cs]

Additionally, our aspect have to report an error when it is applied on a static method.

[!metalama-file ../../tests/Metalama.Samples.Log105.Tests/StaticMethod.cs]


## Implementation

How can we implement these new requirements? Let's look at the new implementation of the aspect.

[!metalama-file LogAttribute.cs]

We added some logic to handle the `ILogger` field or property.

### LogAttribute class

First, the `LogAttribute` class is now derived from <xref:Metalama.Framework.Aspects.MethodAspect> instead of <xref:Metalama.Framework.Aspects.OverrideMethodAspect>. Our motivation for this change is that we need the `OverrideMethod` method to have a parameter that is not available in the <xref:Metalama.Framework.Aspects.OverrideMethodAspect> method.

### BuildAspect method

The `BuildAspect` is the entry point of the aspect. This methods does the following things:
	
* First it looks for a field named `_logger` or a property named `Logger`. Note that it uses the <xref:Metalama.Framework.Code.INamedType.AllFields> and <xref:Metalama.Framework.Code.INamedType.AllProperties> collections, which include the members inherited from the base types. Therefore, we will also find the field or property defined in base types.

* If no field or property is found, the `BuildAspect` methods reports an error. Note that the error is defined as a static field of the class. To read more about reporting errors, see <xref:diagnostics>.

* Then the `BuildAspect` method verifies the type of the `_logger` field or property, and reports an error if the type is incorrect.

* Finally, the `BuildAspect` method overrides the target method by calling <xref:Metalama.Framework.Advising.IAdviceFactory.Override*?text=builder.Advice.Override> and by passing the name of the template method that will override the target method. It passes the <xref:Metalama.Framework.Code.IFieldOrProperty> by passing an object to the `args` parameter, where the property names of the anonymous type must match the parameter names of the template method.

To learn more about imperative advising of methods, see <xref:overriding-method>. To learn more about template parameters, see <xref:template-parameters>.

### OverrideMethod method

The `OverrideMethod` looks familiar, except for a few differences. The field or property that references the `ILogger` is given by the `loggerFieldOrProperty` parameter, which we set from the `BuildAspect` method. This parameter is of <xref:Metalama.Framework.Code.IFieldOrProperty> type, which is a compile-time type, representing metadata. Now, we need to access this field or property at run time. We do this using the <xref:Metalama.Framework.Code.IExpression.Value> property, which is indeed very special. It returns an object which, for the code of your template, is of `dynamic` type. When Metalama sees this `dynamic` object, it replaces it by the syntax that represents the field or property, i.e. `this._logger` or `this.Logger` in this case. So, the line `var logger = (ILogger) loggerFieldOrProperty.Value!;` generates code that stores the `ILogger` field or property into a local variable, which we can the use in run-time part of the template.

### BuildEligibility

The last requirement to implement is to report an error when the aspect is applied to a static method. 

We achieve this using the (elibigility)[xref:eligibility] feature. The benefit of using this feature, instead of reporting an error using `builder.Diagnostics.Report`, is that, with eligibility, the IDE will not even propose the aspect in a refactoring menu for a target that is not eligible. This is less confusing for the user of the aspect.


> [!div class="see-also"]
> <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>
> <xref:template-compile-time>
> <xref:template-dynamic-code>
> <xref:overriding-method>
> <xref:template-parameters>
> <xref:eligibility>