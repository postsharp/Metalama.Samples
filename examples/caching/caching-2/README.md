---
uid: sample-cache-2
---

# Caching example: adding eligibility

[!metalama-project-buttons .]

In this example, we will build upon the previous example where we created a caching aspect. This aspect has proven to be simple yet powerful. However, it has certain limitations. One significant shortcoming which requires addressing is that the aspect does not support `void` methods or methods with `out` or `ref` parameters. Attempting to apply the aspect to such methods results in invalid code, often causing confusion to the user. This time, we will address these shortfalls by asking the aspect to notify users of the invalid target declaration by reporting an error.

[!metalama-file ../../tests/Metalama.Samples.Caching2.Tests/Eligibility.cs]

## Aspect Code

To report an error when the aspect is applied to an invalid target declaration, we implement the <xref:Metalama.Framework.Eligibility.IEligible`1.BuildEligibility*> method. Implementing this method does more than check for errors.  It ensures that these declarations will no longer recommend the aspect from the code refactoring menu of your Integrated Development Environment (IDE).

[!metalama-file CacheAttribute.cs from="Start" to="End"]

> [!div class="see-also"]
> <xref:eligibility>


