using ChatRoom.Centralized.Shared.Packets;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ChatRoom.Centralized.Shared;

public class PacketReader : BinaryReader
{
    public PacketReader(NetworkStream stream) : base(stream) { }

    public INetworkPacket ReadPacket(Aes? keys) => ReadPacket(keys, out _);
    public INetworkPacket ReadPacket(Aes? keys, out string outSignature)
    {
        string signature = Encoding.UTF8.GetString(ReadBytes(4));
        INetworkPacket.PacketInfo info = INetworkPacket.packetTypes.SingleOrDefault(
            x => x.signature.Equals(signature)) ?? throw new PacketSignatureException();
        outSignature = signature;

        if (char.IsUpper(signature.First()))
        {
            // Encrypted packet.
            if (keys is null) throw new PacketEncryptionException(true);

            int length = ReadInt32();
            byte[] buffer = ReadBytes(length);
            INetworkPacket result;
            using (MemoryStream temp = new(buffer))
            {
                using CryptoStream encrypted = new(temp, keys.CreateDecryptor(), CryptoStreamMode.Read);
                using BinaryReader encryptedReader = new(encrypted);
                result = (INetworkPacket)info.readDataMethod.Invoke(null, new object[] { encryptedReader })!;
            }
            return result;
        }
        else return (INetworkPacket)info.readDataMethod.Invoke(null, new object[] { this })!;
    }
}
