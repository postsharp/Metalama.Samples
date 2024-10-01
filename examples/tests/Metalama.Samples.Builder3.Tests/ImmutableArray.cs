﻿using Metalama.Samples.Builder3;
using System.Collections.Immutable;

namespace ClassLibrary1Metalama.Samples.Builder3.Tests.SimpleExample._ImmutableArray;

#pragma warning disable CS8618 //  Non-nullable property must contain a non-null value when exiting constructor.

[GenerateBuilder]
public partial class ColorWheel
{
    public ImmutableArray<string> Colors { get; }
}