namespace ChatRoom.Centralized.Shared.Packets;

public class ServerKeyResponsePacket : INetworkPacket<ServerKeyResponsePacket>
{
    public static string Signature => "sKR ";

    public required byte[] BlockSize { get; set; }
    public required byte[] FeedbackSize { get; set; }
    public required byte[] IV { get; set; }
    public required byte[] Key { get; set; }
    public required byte[] KeySize { get; set; }
    public required byte[] Mode { get; set; }
    public required byte[] Padding { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(BlockSize.Length);
        writer.Write(BlockSize);
        writer.Write(FeedbackSize.Length);
        writer.Write(FeedbackSize);
        writer.Write(IV.Length);
        writer.Write(IV);
        writer.Write(Key.Length);
        writer.Write(Key);
        writer.Write(KeySize.Length);
        writer.Write(KeySize);
        writer.Write(Mode.Length);
        writer.Write(Mode);
        writer.Write(Padding.Length);
        writer.Write(Padding);
    }
    public static ServerKeyResponsePacket ReadData(BinaryReader reader) => new()
    {
        BlockSize = reader.ReadBytes(reader.ReadInt32()),
        FeedbackSize = reader.ReadBytes(reader.ReadInt32()),
        IV = reader.ReadBytes(reader.ReadInt32()),
        Key = reader.ReadBytes(reader.ReadInt32()),
        KeySize = reader.ReadBytes(reader.ReadInt32()),
        Mode = reader.ReadBytes(reader.ReadInt32()),
        Padding = reader.ReadBytes(reader.ReadInt32())
    };
}
