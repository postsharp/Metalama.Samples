namespace Metalama.Samples.AbstractFactory.Example;

interface IStorageAdapter
{
    Task<Stream> OpenReadAsync();
    Task WriteAsync( Func<Stream,Task> write );
}