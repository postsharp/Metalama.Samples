using Metalama.Samples.Builder1;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary1Metalama.Samples.Builder1.Tests.SimpleExample;

[GenerateBuilder]
public partial class Song
{
    [Required]
    public string Artist { get; }
    
    [Required]
    public string Title { get; }
    
    public TimeSpan? Duration { get; }
}