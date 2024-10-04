using System.Net;

namespace ChatRoom.Centralized.Shared.Packets;

public class ServerErrorResponsePacket : INetworkPacket<ServerErrorResponsePacket>
{
    public static string Signature => "sErr";

    public required ServerErrorCode ErrorCode { get; set; }
    public required string Message { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write((int)ErrorCode);
        writer.Write(Message);
    }
    public static ServerErrorResponsePacket ReadData(BinaryReader reader) => new()
    {
        ErrorCode = (ServerErrorCode)reader.ReadUInt32(),
        Message = reader.ReadString()
    };
}
