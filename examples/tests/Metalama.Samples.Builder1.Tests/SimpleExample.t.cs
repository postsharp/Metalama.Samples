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
  private Song(string artist, string title, TimeSpan? duration)
  {
    Artist = artist;
    Title = title;
    Duration = duration;
  }
  public Builder ToBuilder()
  {
    var builder = new Builder(Artist, Title);
    builder.Duration = Duration;
    return builder;
  }
  public class Builder
  {
    public Builder(string artist, string title)
    {
      Artist = artist;
      Title = title;
    }
    private string _artist = default !;
    public string Artist
    {
      get
      {
        return _artist;
      }
      set
      {
        _artist = value;
      }
    }
    private TimeSpan? _duration;
    public TimeSpan? Duration
    {
      get
      {
        return _duration;
      }
      set
      {
        _duration = value;
      }
    }
    private string _title = default !;
    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        _title = value;
      }
    }
    public Song Build()
    {
      return new Song(Artist, Title, Duration)!;
    }
  }
}