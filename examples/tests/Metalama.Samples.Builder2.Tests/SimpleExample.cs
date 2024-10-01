﻿using Metalama.Samples.Builder2;
using System.ComponentModel.DataAnnotations;

namespace Metalama.Samples.Builder2.Tests.SimpleExample;

#pragma warning disable CS8618 //  Non-nullable property must contain a non-null value when exiting constructor. 

[GenerateBuilder]
public partial class Song
{
    [Required]
    public string Artist { get; }
    
    [Required]
    public string Title { get; }
    
    public TimeSpan? Duration { get; }
}