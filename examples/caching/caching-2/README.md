---
uid: sample-cache-2
---

# Caching example: adding eligibility

[!metalama-project-buttons .]

In the previous example, we have created a simple but mighty caching aspect. It has however a few limitations: it does not support `void` methods or methods with `out` or `ref` parameters. If the aspect is applied to such methods, the aspect will generate invalid code, and the user will be left confused. In this example, we will ask the aspect to report an error when applied to such methods.

[!metalama-file ../../tests/Metalama.Samples.Caching2.Tests/Eligibility.cs]

## Aspect code


[!metalama-file CacheAttribute.cs]



> [!div class="see-also"]
