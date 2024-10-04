namespace ChatRoom.Centralized.Shared;

public class PacketSignatureException : Exception
{
    public PacketSignatureException()
        : base("The signature of this packet is not recognized.")
    { }
    public PacketSignatureException(string expected, string got)
        : base($"The signature of this packet does not match the expected signature (expected {expected}, got {got}).")
    { }
}
