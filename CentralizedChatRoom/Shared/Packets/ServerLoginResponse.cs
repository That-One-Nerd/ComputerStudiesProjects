namespace ChatRoom.Centralized.Shared.Packets;

public class ServerLoginResponse : INetworkPacket<ServerLoginResponse>
{
    public static string Signature => "SLgn";

    public void WriteData(BinaryWriter writer) { }
    public static ServerLoginResponse ReadData(BinaryReader reader) => new();
}
