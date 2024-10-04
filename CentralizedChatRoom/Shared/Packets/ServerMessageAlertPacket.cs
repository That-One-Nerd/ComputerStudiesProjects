using ChatRoom.Centralized.Shared.ObjectModels;

namespace ChatRoom.Centralized.Shared.Packets;

public class ServerMessageAlertPacket : INetworkPacket<ServerMessageAlertPacket>
{
    public static string Signature => "SAUM";

    public required MessageInfo Message { get; set; }

    public void WriteData(BinaryWriter writer) => Message.WriteData(writer);
    public static ServerMessageAlertPacket ReadData(BinaryReader reader) => new()
    {
        Message = MessageInfo.ReadData(reader)
    };
}
