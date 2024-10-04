using ChatRoom.Centralized.Shared.ObjectModels;

namespace ChatRoom.Centralized.Shared.Packets;

public class ServerChatLogPacket : INetworkPacket<ServerChatLogPacket>
{
    public static string Signature => "SCLP";

    public required MessageInfo[] Messages { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(Messages.Length);
        for (int i = 0; i < Messages.Length; i++) Messages[i].WriteData(writer);
    }
    public static ServerChatLogPacket ReadData(BinaryReader reader)
    {
        MessageInfo[] messages = new MessageInfo[reader.ReadInt32()];
        for (int i = 0; i < messages.Length; i++) messages[i] = MessageInfo.ReadData(reader);
        return new()
        {
            Messages = messages
        };
    }
}
