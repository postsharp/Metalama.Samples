using System.ComponentModel.DataAnnotations;
namespace Metalama.Samples.Builder2.Tests.DerivedType;
#pragma warning disable CS8618 //  Non-nullable property must contain a non-null value when exiting constructor.
[GenerateBuilder]
public class Article
{
  [Required]
  public string Url { get; }
  [Required]
  public string Name { get; }
  protected Article(string url, string name)
  {
    Url = url;
    Name = name;
  }
  public virtual Builder ToBuilder()
  {
    return new Builder(this);
  }
  public class Builder
  {
    public Builder(string url, string name)
    {
      Url = url;
      Name = name;
    }
    protected internal Builder(Article source)
    {
      Url = source.Url;
      Name = source.Name;
    }
    private string _name = default !;
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
      }
    }
    private string _url = default !;
    public string Url
    {
      get
      {
        return _url;
      }
      set
      {
        _url = value;
      }
    }
    public Article Build()
    {
      var instance = new Article(Url, Name);
      return instance;
    }
  }
}
public class WebArticle : Article
{
  public string Keywords { get; }
  protected WebArticle(string keywords, string url, string name) : base(url, name)
  {
    Keywords = keywords;
  }
  public override Builder ToBuilder()
  {
    return new Builder(this);
  }
  public new class Builder : Article.Builder
  {
    public Builder(string url, string name) : base(url, name)
    {
    }
    protected internal Builder(WebArticle source) : base(source)
    {
      Keywords = source.Keywords;
    }
    private string _keywords = default !;
    public string Keywords
    {
      get
      {
        return _keywords;
      }
      set
      {
        _keywords = value;
      }
    }
    public new WebArticle Build()
    {
      var instance = new WebArticle(Keywords, Url, Name);
      return instance;
    }
  }
}