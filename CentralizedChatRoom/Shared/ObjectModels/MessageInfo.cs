namespace ChatRoom.Centralized.Shared.ObjectModels;

public class MessageInfo
{
    public required Guid MessageId { get; set; }
    public required Guid UserId { get; set; }
    public required DateTimeOffset SentAt { get; set; }
    public required string Contents { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(MessageId.ToByteArray());
        writer.Write(UserId.ToByteArray());
        writer.Write(SentAt.ToUnixTimeMilliseconds());
        writer.Write(Contents);
    }
    public static MessageInfo ReadData(BinaryReader reader) => new()
    {
        MessageId = new(reader.ReadBytes(16)),
        UserId = new(reader.ReadBytes(16)),
        SentAt = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64()),
        Contents = reader.ReadString()
    };
}
