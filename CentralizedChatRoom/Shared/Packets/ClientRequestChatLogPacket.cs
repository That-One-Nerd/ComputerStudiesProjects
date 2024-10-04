namespace ChatRoom.Centralized.Shared.Packets;

public class ClientRequestChatLogPacket : INetworkPacket<ClientRequestChatLogPacket>
{
    public static string Signature => "CRCL";

    public required int Amount { get; set; }
    public required DateTimeOffset Latest { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(Amount);
        writer.Write(Latest.ToUnixTimeMilliseconds());
    }
    public static ClientRequestChatLogPacket ReadData(BinaryReader reader) => new()
    {
        Amount = reader.ReadInt32(),
        Latest = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64())
    };
}
