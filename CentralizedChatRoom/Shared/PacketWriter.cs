using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ChatRoom.Centralized.Shared;

public class PacketWriter : BinaryWriter
{
    public PacketWriter(NetworkStream stream) : base(stream) { }

    public void Write(INetworkPacket packet, Aes? keys)
    {
        INetworkPacket.PacketInfo info = INetworkPacket.packetTypes.SingleOrDefault(
            x => x.type == packet.GetType()) ?? throw new PacketSignatureException();
        if (info.signature.Length != 4) throw new("Invalid packet signature length.");
        Write(Encoding.UTF8.GetBytes(info.signature));
        if (char.IsUpper(info.signature.First()))
        {
            // Encrypted packet.
            if (keys is null) throw new PacketEncryptionException(true);
            byte[] contents;
            using (MemoryStream temp = new())
            {
                using (CryptoStream encrypted = new(temp, keys.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using BinaryWriter encryptedWriter = new(encrypted);
                    packet.WriteData(encryptedWriter);
                }
                contents = temp.GetBuffer();
            };
            Write(contents.Length);
            Write(contents);
        }
        else
        {
            // Unencrypted packet.
            packet.WriteData(this);
        }
    }
    public void Write<T>(T packet, Aes? keys) where T : INetworkPacket<T>
    {
        if (T.Signature.Length != 4) throw new("Invalid packet length.");
        if (char.IsUpper(T.Signature.First()))
        {
            // Encrypted packet.
            if (keys is null) throw new PacketEncryptionException(true);
            MemoryStream temp = new();
            CryptoStream encrypted = new(temp, keys.CreateEncryptor(), CryptoStreamMode.Write);
            BinaryWriter encryptedWriter = new(encrypted);
            packet.WriteData(encryptedWriter);
            encrypted.Flush();
            byte[] contents = temp.GetBuffer();
            temp.Close();
            Write(contents.Length);
            Write(contents);
        }
        else
        {
            // Unencrypted packet.
            packet.WriteData(this);
        }
    }
}
