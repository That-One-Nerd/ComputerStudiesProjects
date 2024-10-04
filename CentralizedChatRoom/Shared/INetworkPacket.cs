using System.Reflection;

namespace ChatRoom.Centralized.Shared;

public interface INetworkPacket
{
    internal static readonly List<PacketInfo> packetTypes = new();
    static INetworkPacket()
    {
        Type[] allTypes = Assembly.GetAssembly(typeof(INetworkPacket))!.GetTypes();
        IEnumerable<Type> validTypes = allTypes.Where(x => x.GetInterface("INetworkPacket") is not null);
        foreach (Type t in validTypes)
        {
            if (t.Name.StartsWith("INetworkPacket")) continue; // Ignore the base interfaces.
            PropertyInfo? sigProperty = t.GetProperty("Signature", BindingFlags.Public | BindingFlags.Static);
            if (sigProperty is null) continue; // Likely not a INetworkPacket<TSelf> derivative, ignore.

            string signature = (string)sigProperty.GetValue(null)!;
            MethodInfo readDataMethod = t.GetMethod("ReadData", BindingFlags.Public | BindingFlags.Static)!;
            packetTypes.Add(new()
            {
                type = t,
                signature = signature,
                readDataMethod = readDataMethod,
            });
        }
    }
    internal void WriteData(BinaryWriter writer);

    internal class PacketInfo
    {
        public required Type type;
        public required string signature;
        public required MethodInfo readDataMethod;
    }
}

public interface INetworkPacket<TSelf> : INetworkPacket where TSelf : INetworkPacket<TSelf>
{
    public static abstract string Signature { get; }

    internal static abstract TSelf ReadData(BinaryReader reader);
}
