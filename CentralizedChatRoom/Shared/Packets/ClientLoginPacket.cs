namespace ChatRoom.Centralized.Shared.Packets;

public class ClientLoginPacket : INetworkPacket<ClientLoginPacket>
{
    public static string Signature => "CLgn";

    public required string Username { get; set; }
    public required byte[] PasswordHashed { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(Username);
        writer.Write(PasswordHashed.Length);
        writer.Write(PasswordHashed);
    }
    public static ClientLoginPacket ReadData(BinaryReader reader) => new()
    {
        Username = reader.ReadString(),
        PasswordHashed = reader.ReadBytes(reader.ReadInt32())
    };
}
