namespace Metalama.Samples.AbstractFactory.Example;

class TestStorageAdapter : IStorageAdapter
{
    private byte[] _buffer = [];
    
    public Task<Stream> OpenReadAsync()
    {
        return Task.FromResult<Stream>(new MemoryStream(this._buffer));
    }

    public async Task WriteAsync(Func<Stream, Task> write)
    {
        using var memoryStream = new MemoryStream();
        await write(memoryStream);
        this._buffer = memoryStream.ToArray();
    }
}