---
uid: sample-cache-1
---

# Caching example: getting started

[!metalama-project-buttons .]

At first sight, caching is simple. Our aspect first generates the cache key. It then performs a cache lookup. If the method result is present in the cache, this cached result is returned. Otherwise, the method is executed and the return value is stored in the cache.

This is illustrated in the following example, which compares the source code to the transformed code.

[!metalama-compare Calculator.cs]

## Infrastructure code

The aspect relies on the `ICache` interface, which just has the two methods used by the aspect.

[!metalama-file ICache.cs]

A typical implementation of this interface would use <xref:System.Runtime.Caching.MemoryCache>.

## Aspect code

The aspect itself is rather simple:

[!metalama-file CacheAttribute.cs]

As usual, our aspect class derives from the <xref:Metalama.Framework.Aspects.OverrideMethodAspect> abstract class, which in turn derives from the <xref:System.Attribute?text=System.Attribute> class. This makes `CacheAttribute` a custom attribute. The <xref:Metalama.Framework.Aspects.OverrideMethodAspect.OverrideMethod*> method acts like a _template_. Most of the code in this template is injected into the target method, i.e., the method to which we add the `[ReportAndSwallowExceptionsAttribute]` custom attribute. Inside the <xref:Metalama.Framework.Aspects.OverrideMethodAspect.OverrideMethod*> implementation, the call to `meta.Proceed()` has a very special meaning. When the aspect is applied to the target, the call to `meta.Proceed()` is replaced by the original implementation, with a few syntactic changes to capture the return value.

The complexity of building the caching key is hidden in the `CacheKeyBuilder` class. It is a compile-time class.

[!metalama-file CacheKeyBuilder.cs]

The <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> class, as its name indicates, is a compile-time class that helps building interpolated strings. The `meta.Target` property exposes the context into which the template is applied. So, `meta.Target.Type` is the current type, `meta.Target.Method` is the current method, and so on. The `GetCachingKey` method builds an interpolated string for the current context. Parameters are represented at compile time by the <xref:Metalama.Framework.Code.IParameter> interface. The <xref:Metalama.Framework.Code.IExpression.Value?text=parameter.Value> expression, when it is evaluated at compile time, returns an object of `dynamic` type representing a _run-time_ expression, in this case, the name of the parameter.

When the aspect receives the <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder> instance from the `CacheKeyBuilder` class, it converts it to a run-time interpolated string by calling the <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilderExtensions.ToValue*> method. This method returns an object of `dynamic` representing the interpolated string, which can be cast to a `string` and used as the caching key.


> [!div class="see-also"]
> <xref:template-overview>
> <xref:Metalama.Framework.Code.SyntaxBuilders.InterpolatedStringBuilder>
> <xref:template-compile-time>
> <xref:template-dynamic-code>