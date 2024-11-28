namespace Metalama.Samples.AbstractFactory.Example;

class FileSystemStorageAdapter : IStorageAdapter
{
    private readonly string _filePath;

    public FileSystemStorageAdapter(string filePath)
    {
        this._filePath = filePath;
    }

    public Task<Stream> OpenReadAsync()
    {
        return Task.FromResult( (Stream) File.OpenRead(this._filePath));
    }

    public async Task WriteAsync(Func<Stream, Task> write)
    {
        await using var stream = File.OpenWrite(this._filePath);
        await write(stream);
    }
}