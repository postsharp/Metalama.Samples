namespace Metalama.Samples.AbstractFactory.Example;

interface IStorageAdapterFactory
{
    IStorageAdapter CreateStorageAdapter( string url );
}