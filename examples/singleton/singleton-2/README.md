---
uid: sample-singleton-2
---

# Example: The Modern Singleton Pattern

[!metalama-project-buttons .]

In modern applications, the classic singleton pattern often proves unsuitable. It has even been argued that it is an
antipattern. There are typically two good reasons for this:

* We may want to inject a dependency into the singleton, therefore we need a public constructor.
* We may want to isolate tests from each other, with each test having its own instance of each singleton. This is especially useful when unit tests are executed concurrently.

In the _modern singleton_ pattern, the singleton's lifetime is managed by the dependency injection container, typically
using the `AddSingleton` method of `IServiceCollection`.

In both scenarios, a public constructor is needed, one that accepts dependent services as parameters.

Unlike the classic pattern, modern singletons do not struggle with boilerplate code or consistency issues. However, they
do present a significant safety problem: there is nothing that prevents someone from creating multiple instances of the
class in production code by directly calling the constructor. Let's examine how we can mitigate this risk.

Consider a modern version of our `PerformanceCounterManager` service, which we used in the previous article. It now
depends on an `IPerformanceCounterUploader` interface that could have a production interface (for instance, to upload to
AWS) and a testing one (to store records for later inspection).

[!metalama-file PerformanceCounterManager.cs]

Typically, services are added to the `IServiceCollection` using code like this:

[!metalama-file Startup.cs]

Generally, the `Startup` class is the _only_ production component that should be allowed to create instances of
singletons. The only potential exceptions are unit tests.

To validate this constraint, we can use Metalama architecture validation (see <xref:validating-usage>) and permit the
constructor to be called only from the `Startup` class or from a test namespace. This can be accomplished by creating a
type-level `SingletonAttribute` aspect and using the <xref:Metalama.Extensions.Architecture> package to control
authorized references.

[!metalama-file SingletonAttribute.cs]

Now, if we attempt to instantiate the `PerformanceCounterManager` from production code, we receive a warning:

[!metalama-file IllegalUse.cs]

