namespace ChatRoom.Centralized.Shared;

public class PacketEncryptionException : Exception
{
    public PacketEncryptionException(bool should) : base(should ?
        "This packet must be encrypted." : "This packet cannot be encrypted.")
    { }
}
