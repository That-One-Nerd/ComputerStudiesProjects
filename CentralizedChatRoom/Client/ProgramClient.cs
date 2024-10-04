/**********722871**********
 * Date: 10/4/2024
 * Programmer: Kyle Gilbert
 * Program Name: CentralizedChatRoom
 * Program Description: A chat room system I wrote with a custom packet system.
 *                      Connections are partially encrypted.
 **************************/

using ChatRoom.Centralized.Shared;
using ChatRoom.Centralized.Shared.ObjectModels;
using ChatRoom.Centralized.Shared.Packets;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ChatRoom.Centralized.Client;

public static class ProgramClient
{
    private static readonly TcpClient client = new();

    public static void Main()
    {
        Console.Write("Enter the IP address to connect to > ");
        IPAddress ip = IPAddress.Parse(Console.ReadLine()!);

        Console.Write("Testing connection...");
        client.Connect(new(ip, GlobalServerInfo.Port));
        NetworkStream stream = client.GetStream();
        PacketWriter writer = new(stream);
        PacketReader reader = new(stream);

        INetworkPacket packet = new ClientHelloPacket()
        {
            KeepOpen = true,
            TimeSent = DateTimeOffset.UtcNow
        };
        writer.Write(packet, null);

        packet = reader.ReadPacket(null);
        if (packet is not ServerHelloResponsePacket pHello) throw new("Didn't get back hello response, got something else!");

        Console.WriteLine($" Latency: {pHello.Latency} ms");
        Console.WriteLine($"""
            
            Server Name: {pHello.Information.Name}
                   Description: {pHello.Information.Description}
                   Sign-up Allowed: {(pHello.Information.AllowsSignup ? "Yes" : "No")}

            """);

        RSA clientRsa = RSA.Create();
        packet = new ClientRequestServerKeyPacket()
        {
            KeepOpen = true,
            ClientPublicKey = clientRsa.ExportRSAPublicKey()
        };
        writer.Write(packet, null);

        packet = reader.ReadPacket(null);
        if (packet is not ServerKeyResponsePacket pServerKey) throw new("Didn't get back the server's AES key, got something else!");

        Aes serverAes = Aes.Create();

        serverAes.BlockSize = BitConverter.ToInt32(clientRsa.Decrypt(pServerKey.BlockSize, RSAEncryptionPadding.Pkcs1));
        serverAes.FeedbackSize = BitConverter.ToInt32(clientRsa.Decrypt(pServerKey.FeedbackSize, RSAEncryptionPadding.Pkcs1));
        serverAes.IV = clientRsa.Decrypt(pServerKey.IV, RSAEncryptionPadding.Pkcs1);
        serverAes.KeySize = BitConverter.ToInt32(clientRsa.Decrypt(pServerKey.KeySize, RSAEncryptionPadding.Pkcs1));
        serverAes.Key = clientRsa.Decrypt(pServerKey.Key, RSAEncryptionPadding.Pkcs1);
        serverAes.Mode = (CipherMode)BitConverter.ToInt32(clientRsa.Decrypt(pServerKey.Mode, RSAEncryptionPadding.Pkcs1));
        serverAes.Padding = (PaddingMode)BitConverter.ToInt32(clientRsa.Decrypt(pServerKey.Padding, RSAEncryptionPadding.Pkcs1));

        Console.Write("(C)reate an account or (L)og in? > ");
    _tryChoice:
        string choice = Console.ReadLine()!;
    _tryLogin:

        string username, password;
        byte[] passwordHash;
        switch (choice.Trim().ToLower())
        {
            case "c":
                if (!pHello.Information.AllowsSignup)
                {
                    Console.WriteLine("Cannot create an account on this server.");
                    goto case "l";
                }
                Console.Write("Enter a username > ");
                username = Console.ReadLine()!;

                Console.Write("Enter a password > ");
                password = Console.ReadLine()!;
                passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

                packet = new ClientCreateAccountPacket()
                {
                    Username = username,
                    PasswordHashed = passwordHash
                };
                break;
            case "l":
                Console.Write("Enter your username > ");
                username = Console.ReadLine()!;

                Console.Write("Enter your password > ");
                password = Console.ReadLine()!;
                passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

                packet = new ClientLoginPacket()
                {
                    Username = username,
                    PasswordHashed = passwordHash
                };
                break;

            default: goto _tryChoice;
        }

        Console.Write("Logging in... ");
        writer.Write(packet, serverAes);

        packet = reader.ReadPacket(serverAes);
        if (packet is ServerErrorResponsePacket pLoginError)
        {
            Console.WriteLine($"Error: {pLoginError.Message}");
            goto _tryLogin;
        }
        Console.WriteLine("Ready!");

        Thread.Sleep(500);

        Console.Clear();
        packet = new ClientRequestChatLogPacket()
        {
            Amount = 30,
            Latest = DateTimeOffset.Now,
        };
        writer.Write(packet, serverAes);

        packet = reader.ReadPacket(serverAes);
        if (packet is not ServerChatLogPacket pChatLog) throw new("Got a packet other than ServerChatLogPacket!");

        MessageInfo[] messages = pChatLog.Messages;
        foreach (MessageInfo message in messages)
        {
            if (!cachedUsers.TryGetValue(message.UserId, out PublicUserInfo? user))
            {
                packet = new ClientRequestUserInfoPacket()
                {
                    UserId = message.UserId,
                    Username = null
                };
                writer.Write(packet, serverAes);
                packet = reader.ReadPacket(serverAes);

                if (packet is ServerUserInformationPacket pUserInfo)
                {
                    user = pUserInfo.UserInfo;
                    cachedUsers.Add(message.UserId, user);
                }
                else user = new() { Username = "Unknown User" };
            }

            Console.WriteLine($"""
              {message.SentAt,19:g}  {user.Username}
            {message.Contents}

            """);
        }

        Task.Run(() => MonitorThread(writer, reader, serverAes, Console.CursorTop));

        StringBuilder toSend = new();
        ConsoleKeyInfo key;
        while (true)
        {
            // Get key inputs.
            key = Console.ReadKey(true);

            if (!char.IsControl(key.KeyChar))
            {
                toSend.Append(key.KeyChar);
                lock (Console.Out)
                {
                    Console.Write(key.KeyChar);
                }
            }
            else if (key.Key == ConsoleKey.Backspace && toSend.Length > 0)
            {
                toSend.Remove(toSend.Length - 1, 1);
                lock (Console.Out)
                {
                    Console.CursorLeft--;
                    Console.Write(' ');
                    Console.CursorLeft--;
                }
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                // Send message.
                packet = new ClientSendMessagePacket()
                {
                    Content = toSend.ToString()
                };
                lock (writer)
                {
                    writer.Write(packet, serverAes);
                }
                lock (Console.Out)
                {
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', toSend.Length));
                    Console.CursorLeft = 0;
                }
                toSend.Clear();
                Thread.Sleep(100);
            }
        }
    }

    private static readonly Dictionary<Guid, PublicUserInfo> cachedUsers = new();
    private static Task MonitorThread(PacketWriter writer, PacketReader reader, Aes keys, int lastMessagePos)
    {
        while (true)
        {
            INetworkPacket packet = reader.ReadPacket(keys);
            if (packet is ServerMessageAlertPacket pAlert)
            {
                MessageInfo message = pAlert.Message;
                if (!cachedUsers.TryGetValue(message.UserId, out PublicUserInfo? user))
                {
                    packet = new ClientRequestUserInfoPacket()
                    {
                        UserId = message.UserId,
                        Username = null
                    };
                    lock (writer)
                    {
                        writer.Write(packet, keys);
                        packet = reader.ReadPacket(keys);
                    }

                    if (packet is ServerUserInformationPacket pUserInfo)
                    {
                        user = pUserInfo.UserInfo;
                        cachedUsers.Add(message.UserId, user);
                    }
                    else user = new() { Username = "Unknown User" };
                }

                lock (Console.Out)
                {
                    Console.WriteLine($"""
                      {message.SentAt,19:g}  {user.Username}
                    {message.Contents}

                    """);
                    lastMessagePos = Console.CursorTop;
                }
            }
        }
    }
}
