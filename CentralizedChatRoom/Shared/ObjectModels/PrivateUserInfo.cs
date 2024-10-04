using System.Reflection;

namespace ChatRoom.Centralized.Shared.ObjectModels;

public class PrivateUserInfo : IPrivateInfo<PrivateUserInfo, PublicUserInfo>
{
    public required Guid UserID { get; set; }
    public required string Username { get; set; }
    public required byte[] PasswordHashed { get; set; }

    public PrivateUserInfo() { }

    public PrivateUserInfo(PublicUserInfo publicInfo)
    {
        foreach ((PropertyInfo hereProp, PropertyInfo thereProp) in IPrivateInfo<PrivateUserInfo, PublicUserInfo>.copyProps)
        {
            object? value = thereProp.GetValue(publicInfo);
            hereProp.SetValue(this, value);
        }
    }
    public PublicUserInfo ToPublic()
    {
        PublicUserInfo result = new();
        foreach ((PropertyInfo hereProp, PropertyInfo thereProp) in IPrivateInfo<PrivateUserInfo, PublicUserInfo>.copyProps)
        {
            object? value = hereProp.GetValue(this);
            thereProp.SetValue(result, value);
        }
        return result;
    }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write(UserID.ToByteArray());
        writer.Write(Username);
        writer.Write(PasswordHashed.Length);
        writer.Write(PasswordHashed);
    }
    public static PrivateUserInfo ReadData(BinaryReader reader) => new()
    {
        UserID = new(reader.ReadBytes(16)),
        Username = reader.ReadString(),
        PasswordHashed = reader.ReadBytes(reader.ReadInt32())
    };
}
