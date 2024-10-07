﻿using Metalama.Samples.Builder3;

namespace ClassLibrary1Metalama.Samples.Builder3.Tests.SimpleExample.GenericExample;

#pragma warning disable CS8618 //  Non-nullable property must contain a non-null value when exiting constructor.

[GenerateBuilder]
public partial class StringKeyedValue<T>
{
    public T Value { get; }
}