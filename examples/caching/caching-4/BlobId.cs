public class BlobId
{
    [CacheKeyMember]
    public string Container { get; }

    [CacheKeyMember]
    public byte[] Hash { get; }

    public BlobId( string container, byte[] hash )
    {
        this.Container = container;
        this.Hash = hash;
    }
}
