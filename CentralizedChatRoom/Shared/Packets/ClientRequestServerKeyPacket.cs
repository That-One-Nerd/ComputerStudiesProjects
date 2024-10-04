namespace ChatRoom.Centralized.Shared.Packets;

public class ClientRequestServerKeyPacket : INetworkPacket<ClientRequestServerKeyPacket>
{
    public static string Signature => "cKRS";

    public required bool KeepOpen { get; set; }
    public required byte[] ClientPublicKey { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(KeepOpen);
        writer.Write(ClientPublicKey.Length);
        writer.Write(ClientPublicKey);
    }
    public static ClientRequestServerKeyPacket ReadData(BinaryReader reader) => new()
    {
        KeepOpen = reader.ReadBoolean(),
        ClientPublicKey = reader.ReadBytes(reader.ReadInt32())
    };
}
