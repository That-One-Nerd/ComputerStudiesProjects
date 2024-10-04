using ChatRoom.Centralized.Shared;
using ChatRoom.Centralized.Shared.ObjectModels;
using System.Reflection;
using System.Security.Cryptography;

namespace ChatRoom.Centralized.Server;

public class PrivateServerInfo : IPrivateInfo<PrivateServerInfo, PublicServerInfo>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool AllowsSignup { get; set; }
    public required Aes AesServer { get; set; }

    public PrivateServerInfo()
    {
        Name = "Unnamed Server";
        Description = "No Description Provided";
        AllowsSignup = true;
    }
    public PublicServerInfo ToPublic()
    {
        PublicServerInfo result = new();
        foreach ((PropertyInfo hereProp, PropertyInfo thereProp) in IPrivateInfo<PrivateServerInfo, PublicServerInfo>.copyProps)
        {
            object? value = hereProp.GetValue(this);
            thereProp.SetValue(result, value);
        }
        return result;
    }

    public PrivateServerInfo(PublicServerInfo publicInfo) : this()
    {
        foreach ((PropertyInfo hereProp, PropertyInfo thereProp) in IPrivateInfo<PrivateServerInfo, PublicServerInfo>.copyProps)
        {
            object? value = thereProp.GetValue(publicInfo);
            hereProp.SetValue(this, value);
        }
    }
}
