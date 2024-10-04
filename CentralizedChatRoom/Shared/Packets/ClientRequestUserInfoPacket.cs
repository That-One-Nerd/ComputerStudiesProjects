namespace ChatRoom.Centralized.Shared.Packets;

public class ClientRequestUserInfoPacket : INetworkPacket<ClientRequestUserInfoPacket>
{
    public static string Signature => "CRUI";

    public required Guid? UserId { get; set; }
    public required string? Username { get; set; }

    public void WriteData(BinaryWriter writer)
    {
        if (UserId.HasValue)
        {
            writer.Write(true);
            writer.Write(UserId.Value.ToByteArray());
            if (Username is not null) throw new("Cannot have both username and ID.");
        }
        else if (Username is not null)
        {
            writer.Write(false);
            writer.Write(Username);
        }
        else throw new("Either a username or an ID must be provided.");
    }
    public static ClientRequestUserInfoPacket ReadData(BinaryReader reader)
    {
        bool useGuid = reader.ReadBoolean();
        if (useGuid) return new()
        {
            UserId = new(reader.ReadBytes(16)),
            Username = null
        };
        else return new()
        {
            UserId = null,
            Username = reader.ReadString()
        };
    }
}
