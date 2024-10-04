namespace ChatRoom.Centralized.Shared.Packets;

public class ClientSendMessagePacket : INetworkPacket<ClientSendMessagePacket>
{
    public static string Signature => "CMes";

    public required string Content { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(Content);
    }
    public static ClientSendMessagePacket ReadData(BinaryReader reader) => new()
    {
        Content = reader.ReadString()
    };
}
