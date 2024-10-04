using ChatRoom.Centralized.Shared.ObjectModels;

namespace ChatRoom.Centralized.Shared.Packets;

public class ServerUserInformationPacket : INetworkPacket<ServerUserInformationPacket>
{
    public static string Signature => "SUsr";

    public required PublicUserInfo UserInfo { get; set; }

    public void WriteData(BinaryWriter writer) => UserInfo.WriteData(writer);
    public static ServerUserInformationPacket ReadData(BinaryReader reader) => new()
    {
        UserInfo = PublicUserInfo.ReadData(reader)
    };
}
