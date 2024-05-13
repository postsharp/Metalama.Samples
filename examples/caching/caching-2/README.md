---
uid: sample-cache-2
level: 250
---

# Caching example, step 2: adding eligibility

[!metalama-project-buttons .]

In this example, we will enhance the caching aspect created in the previous example. Although powerful, it has certain
limitations. A significant shortcoming we aim to address now is that the aspect generates invalid code when applied
to `void` methods or methods with `out` or `ref` parameter, causing user confusion. To prevent this, we will update the
aspect to report an error to the user when an invalid method is targeted.

To achieve this, we will implement the <xref:Metalama.Framework.Eligibility.IEligible`1.BuildEligibility*> method. This
method not only checks for errors but also ensures that such declarations will no longer recommend the aspect in the
code refactoring menu of your IDE.

[!metalama-file CacheAttribute.cs member="CacheAttribute.BuildEligibility"]

An error is now reported when the user tries to apply the aspect to an unsupported method:

[!metalama-file ../../tests/Metalama.Samples.Caching2.Tests/Eligibility.cs]

> [!div class="see-also"]
> <xref:eligibility>
