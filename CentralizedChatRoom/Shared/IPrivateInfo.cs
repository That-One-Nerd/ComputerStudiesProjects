using System.Reflection;

namespace ChatRoom.Centralized.Shared;

public interface IPrivateInfo<TPrivate, TPublic>
    where TPrivate : IPrivateInfo<TPrivate, TPublic>
    where TPublic : IPublicInfo, new()
{
    static IPrivateInfo()
    {
        copyProps = new();
        Type hereType = typeof(TPrivate),
             thereType = typeof(TPublic);
        foreach (PropertyInfo thereProp in thereType.GetProperties())
        {
            if (!thereProp.CanWrite || !thereProp.CanRead) continue;
            PropertyInfo hereProp = hereType.GetProperty(thereProp.Name)!;
            copyProps.Add((hereProp, thereProp));
        }
    }
    protected static readonly List<(PropertyInfo here, PropertyInfo there)> copyProps;
}
