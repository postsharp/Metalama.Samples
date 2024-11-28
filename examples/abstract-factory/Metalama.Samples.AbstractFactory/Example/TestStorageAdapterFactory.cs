using System.Collections.Concurrent;

namespace Metalama.Samples.AbstractFactory.Example;

class TestStorageAdapterFactory : IStorageAdapterFactory
{
    private readonly ConcurrentDictionary<string,TestStorageAdapter> _storageAdapters = new ();
    
    public IStorageAdapter CreateStorageAdapter(string url)
    {
        return this._storageAdapters.GetOrAdd(url, s => new TestStorageAdapter());
    }
}