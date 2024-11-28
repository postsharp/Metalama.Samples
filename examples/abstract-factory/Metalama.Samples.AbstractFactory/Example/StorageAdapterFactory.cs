namespace Metalama.Samples.AbstractFactory.Example;

class StorageAdapterFactory : IStorageAdapterFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StorageAdapterFactory(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    public IStorageAdapter CreateStorageAdapter(string url)
    {
        if (url.StartsWith("https://"))
        {
            return new HttpStorageAdapter( this._httpClientFactory, url);
        }
        else
        {
            return new FileSystemStorageAdapter(url);
        }
    }
}