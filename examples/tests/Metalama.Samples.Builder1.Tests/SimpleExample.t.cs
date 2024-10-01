using Metalama.Samples.Builder1;
using System.ComponentModel.DataAnnotations;
namespace Metalama.Samples.Builder1.Tests.SimpleExample;
[GenerateBuilder]
public partial class Song
{
  [Required]
  public string Artist { get; }
  [Required]
  public string Title { get; }
  public TimeSpan? Duration { get; }
  public string Genre { get; } = "General";
  private Song(string artist, string title, TimeSpan? duration, string genre)
  {
    Artist = artist;
    Title = title;
    Duration = duration;
    Genre = genre;
  }
  public Builder ToBuilder()
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
      Genre = source.Genre;
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
    private string _genre = "General";
    public string Genre
    {
      get
      {
        return _genre;
      }
      set
      {
        _genre = value;
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
      var instance = new Song(Artist, Title, Duration, Genre)!;
      return instance;
    }
  }
}