using System.Text;

namespace ChatRoom.Centralized.Shared.Packets;

public class ClientHelloPacket : INetworkPacket<ClientHelloPacket>
{
    public static string Signature => "cHI ";

    public required bool KeepOpen { get; set; }
    public required DateTimeOffset TimeSent { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(KeepOpen);
        writer.Write(TimeSent.ToUnixTimeMilliseconds());
    }
    public static ClientHelloPacket ReadData(BinaryReader reader) => new()
    {
        KeepOpen = reader.ReadBoolean(),
        TimeSent = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64())
    };
}
