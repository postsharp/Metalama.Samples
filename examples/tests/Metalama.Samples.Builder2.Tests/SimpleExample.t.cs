using Metalama.Samples.Builder2;
using System.ComponentModel.DataAnnotations;
namespace ClassLibrary1Metalama.Samples.Builder2.Tests.SimpleExample;
#pragma warning disable CS8618 //  Non-nullable property must contain a non-null value when exiting constructor.
[GenerateBuilder]
public partial class Song
{
  [Required]
  public string Artist { get; }
  [Required]
  public string Title { get; }
  public TimeSpan? Duration { get; }
  protected Song(string artist, string title, TimeSpan? duration)
  {
    Artist = artist;
    Title = title;
    Duration = duration;
  }
  public virtual Builder ToBuilder()
  {
    return new Builder(this);
  }
  public class Builder
  {
    public Builder(string artist, string title)
    {
      Artist = artist;
      Title = title;
    }
    protected internal Builder(Song source)
    {
      Artist = source.Artist;
      Title = source.Title;
      Duration = source.Duration;
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