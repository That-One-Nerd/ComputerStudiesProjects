using System.Text;

namespace ChatRoom.Centralized.Shared.Packets;

public class ClientCreateAccountPacket : INetworkPacket<ClientCreateAccountPacket>
{
    public static string Signature => "CSUp";

    public required string Username;
    public required byte[] PasswordHashed;

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(Username);
        writer.Write(PasswordHashed.Length);
        writer.Write(PasswordHashed);
    }
    public static ClientCreateAccountPacket ReadData(BinaryReader reader) => new()
    {
        Username = reader.ReadString(),
        PasswordHashed = reader.ReadBytes(reader.ReadInt32())
    };
}
