namespace ChatRoom.Centralized.Shared.ObjectModels;

public class PublicUserInfo : IPublicInfo
{
    public string Username { get; set; } = "";

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(Username);
    }
    public static PublicUserInfo ReadData(BinaryReader reader) => new()
    {
        Username = reader.ReadString(),
    };
}
