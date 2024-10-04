using ChatRoom.Centralized.Shared.ObjectModels;
using System.Text;

namespace ChatRoom.Centralized.Shared.Packets;

public class ServerHelloResponsePacket : INetworkPacket<ServerHelloResponsePacket>
{
    public static string Signature => "sHIr";

    public long Latency => (long)(ServerRecieved - ClientSent).TotalMilliseconds;

    public required DateTimeOffset ClientSent { get; set; }
    public required DateTimeOffset ServerRecieved { get; set; }

    public required PublicServerInfo Information { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(ClientSent.ToUnixTimeMilliseconds());
        writer.Write(ServerRecieved.ToUnixTimeMilliseconds());
        writer.Write(Information.Name);
        writer.Write(Information.Description);
    }
    public static ServerHelloResponsePacket ReadData(BinaryReader reader) => new()
    {
        ClientSent = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64()),
        ServerRecieved = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64()),
        Information = new()
        {
            Name = reader.ReadString(),
            Description = reader.ReadString()
        }
    };
}
