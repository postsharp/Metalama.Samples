---
uid: sample-cache-4
---

# Caching example: cache key for external types

[!metalama-project-buttons .]

In the previous article, we designed a pattern where custom types could be included in a cache key if they implemented the `ICacheKey` interface. We created an aspect that automatically implemented this interface for all fields or properties annotated with the `[CacheKeyMember]` custom attribute.

There are still two issues with this design. First, how can we handle types for which we don't have the source code? Then, we would like our aspect to report an error when the user of the aspect tries to include in the cache key an item whose type is not supported. That is, the default behavior with unsupported types should now be to report an error instead of using the `ToString` method.

## ICacheKeyBuilder

To support external types, we define a new concept of cache key builder: an object that can build a cache key for another object. We define the `ICacheKeyBuilder` as follows:

[!metalama-file ICacheKeyBuilder.cs]

The generic parameter of the interface corresponds to the supported type of objects. The benefit of using a generic parameter is that we can generate the cache key without casting value-typed values into an `object`.

For instance, here is an implementation for `byte[]`:

[!metalama-file ByteArrayCacheKeyBuilder.cs]

## Compile-time API

If we want to be able to report errors _at compile time_ when someone attempts to include an unsupported type in the cache key, we need a compile-time API to configure the caching aspects. This can be done through a concept named _project extension_, explained in <xref:exposing-configuration>. We define a new compile-time class named `CachingOptions` that stores the mapping between types and their builders. We also store a list of types for which we want to use `ToString`.

[!metalama-file CachingOptions.cs]

We also define an extension method to ease the access to caching options:

[!metalama-file CachingProjectExtensions.cs]

It is now easy to configure the caching API using a fabric:

[!metalama-file Fabric.cs]

If you haven't heard about fabrics yet, here are a few words of introduction. Fabrics are compile-time types. Their `AmendProject` method is executed before any aspect. The `AmendProject` method is like a compile-time entry point: it is executed for the sole reason that it exists, just like the `Program.Main`, but at compile time. For more, see <xref:fabrics>.


## ICacheKeyBuilderProvider

At run time, it is useful to abstract the process of getting `ICacheKeyBuilder` instances under a provider pattern. That's why we define the `ICacheKeyBuilderProvider` interface:

[!metalama-file ICacheKeyBuilderProvider.cs]

Note that the `new()` constraint on the generic parameter allows for a trivial implementation of the class.

The `ICacheKeyBuilderProvider` implementation needs to be pulled from the dependency injection container. 

To avoid cache key objects to be instantiated from the dependency injection container, we update the `ICacheKey` interface to receive the provider from the caller:

[!metalama-file ICacheKey.cs]

## Generating the cache key item expression

The logic to generate the expression that gets the caching key of an object has now grown in complexity, with support for three cases, plus null handling.

* Implicit call to `ToString`.
* Call to `ICacheKeyBuilderProvider.GetCacheKeyBuilder`.
* Call to `ICacheKey.ToCacheKey`.

It is now easier to build the expression as a string instead of using a T# template. We have moved this logic to `CachingOptions`:

[!metalama-file CachingOptions.Internals.cs from="TryGetCacheKeyExpression:Start" to="TryGetCacheKeyExpression:End"]

This code uses the <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilder> class, which is essentially a wrapper over a `StringBuilder`. You can happen whatever you want to an <xref:Metalama.Framework.Code.SyntaxBuilders.ExpressionBuilder>, as long as it can be parsed back to a valid C# expression.


## Reporting errors for unsupported types

We want to report an error whenever an unsupported type is being used for a parameter of a cached method, or as a type for a field or property annotated with `[CacheKeyMember]`.

To detect unsupported types, we add the following code to `CachingOptions`:

The first line defines an error kind. Metalama requires the <xref:Metalama.Framework.Diagnostics.DiagnosticDefinition> to be defined in a static field or property. Then, if the type of the `expression` is invalid, this error is reported for that property or parameter. To learn more about reporting errors, see <xref:diagnostics>.

[!metalama-file CachingOptions.Internals.cs from="VerifyCacheKeyMember:Start" to="VerifyCacheKeyMember:End"]

This method must be reported from the `BuildAspect` method of the `CacheAttribute` and `GenerateCacheKeyAspect` aspect classes. Errors cannot be reported from template methods because templates are typically not executed at design time, except when using the _preview_ feature.

There is a limitation, however, which prevents us from detecting unsupported types at design time. When Metalama runs inside the editor, at design time, it does not execute all aspects for all files at every keystroke, but it only does it for the files that have been edited and their dependencies. Therefore, at design time, your aspect receives a _partial_ compilation. It can still see all the types in the project, but it does not see the aspects that have been applied to these types. 

So, when the `CachingOptions.VerifyCacheKeyMember` evaluates `Enhancements().HasAspect<GenerateCacheKeyAspect>()` at design time, the expression does _not_ return an accurate result. Therefore, we can run this method only when we have a _complete_ compilation, i.e. at compile time.

To verify attributes, we need to add this code to the `CacheAttribute` aspect class:

[!metalama-file CacheAttribute.cs from="BuildAspect:Start" to="BuildAspect:End"]


## Aspects in action

Finally, we can apply these aspects to some business code:

[!metalama-files BlobId.cs DatabaseFrontend.cs links="false"]


> [!div class="see-also"]
> <xref:exposing-configuration>
> <xref:fabrics>
> <xref:diagnostics>
