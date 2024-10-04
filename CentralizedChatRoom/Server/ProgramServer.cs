using ChatRoom.Centralized.Shared;
using ChatRoom.Centralized.Shared.ObjectModels;
using ChatRoom.Centralized.Shared.Packets;
using NLog;
using NLog.Targets;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace ChatRoom.Centralized.Server;

public static class ProgramServer
{
#pragma warning disable CS8618
    static ProgramServer()
    {
        logFactory = new()
        {
            Configuration = new()
        };
        logFactory.Configuration.AddTarget(new ColoredConsoleTarget("console"));
        logFactory.Configuration.AddTarget(new FileTarget("logFile")
        {
            FileName = "logFile.txt",
            FileNameKind = FilePathKind.Relative
        });
        logFactory.Configuration.AddRuleForAllLevels("console");
        logFactory.Configuration.AddRuleForAllLevels("logFile");
        logFactory.ReconfigExistingLoggers();
        logger = logFactory.GetCurrentClassLogger();

        info = new()
        {
            Name = "Computer Studies Chat Room",
            Description = "A chat room system I wrote as a computer studies project.",
            AllowsSignup = true,
            AesServer = Aes.Create()
        };
        info.AesServer.Padding = PaddingMode.ISO10126;
    }

    internal static readonly LogFactory logFactory;
    private static readonly Logger logger;

    public static readonly PrivateServerInfo info;
    public static UserDatabase users;
    public static MessageDatabase messages;

    private static TcpListener listener;
#pragma warning restore CS8618

    public static async Task Main()
    {
        logger.Info("Setting up chat room...");
        logger.Debug("Starting listener...");
        listener = new(IPAddress.Any, GlobalServerInfo.Port);
        listener.Start();
        logger.Debug("Done");
        users = new("users.dat", TimeSpan.FromSeconds(150));
        messages = new("messages.dat", TimeSpan.FromSeconds(10));
        logger.Info("Ready");

        string hostName = Dns.GetHostName();
        IPHostEntry host = Dns.GetHostEntry(hostName);

        logger.Info($"Hostname: {hostName}");
        for (int i = 0; i < host.AddressList.Length; i++)
        {
            logger.Info($"Local address: {host.AddressList[i]}");
        }

        listener.BeginAcceptTcpClient(OnIncomingConnection, null);
        await Task.Delay(-1);
    }

    private static void OnIncomingConnection(IAsyncResult result)
    {
        TcpClient request = listener.EndAcceptTcpClient(result);
        listener.BeginAcceptTcpClient(OnIncomingConnection, null);

        EndPoint? clientEndPoint = request.Client.LocalEndPoint;
        Logger logger = logFactory.GetLogger(clientEndPoint?.ToString());

        logger.Trace($"Connection established.");
        NetworkStream incoming = request.GetStream();
        PacketWriter writer = new(incoming);
        PacketReader reader = new(incoming);

        int exceptionCount = 0;
        INetworkPacket packet;
    _readPackets:
        try
        {
            while (request.Connected)
            {
                packet = reader.ReadPacket(info.AesServer, out string packetSig);
                logger.Trace($"Recieved packet {packetSig}");
                if (packet is ClientHelloPacket pHello)
                {
                    logger.Info($"Hello sent at {pHello.TimeSent}");
                    packet = new ServerHelloResponsePacket()
                    {
                        ClientSent = pHello.TimeSent,
                        ServerRecieved = DateTimeOffset.UtcNow,
                        Information = info.ToPublic()
                    };
                    writer.Write(packet, info.AesServer);
                    if (!pHello.KeepOpen)
                    {
                        logger.Debug("Incoming packet has not requested to keep the connection open");
                        request.Close();
                    }
                }
                else if (packet is ClientRequestServerKeyPacket pRequestKey)
                {
                    logger.Info($"User is requesting server encryption key.");
                    RSA rsaServer = RSA.Create();
                    rsaServer.ImportRSAPublicKey(pRequestKey.ClientPublicKey, out _);

                    byte[] blockSizeRaw = BitConverter.GetBytes(info.AesServer.BlockSize),
                           feedbackSizeRaw = BitConverter.GetBytes(info.AesServer.FeedbackSize),
                           keySizeRaw = BitConverter.GetBytes(info.AesServer.KeySize),
                           modeRaw = BitConverter.GetBytes((int)info.AesServer.Mode),
                           paddingRaw = BitConverter.GetBytes((int)info.AesServer.Padding);

                    packet = new ServerKeyResponsePacket()
                    {
                        BlockSize = rsaServer.Encrypt(blockSizeRaw, RSAEncryptionPadding.Pkcs1),
                        FeedbackSize = rsaServer.Encrypt(feedbackSizeRaw, RSAEncryptionPadding.Pkcs1),
                        IV = rsaServer.Encrypt(info.AesServer.IV, RSAEncryptionPadding.Pkcs1),
                        Key = rsaServer.Encrypt(info.AesServer.Key, RSAEncryptionPadding.Pkcs1),
                        KeySize = rsaServer.Encrypt(keySizeRaw, RSAEncryptionPadding.Pkcs1),
                        Mode = rsaServer.Encrypt(modeRaw, RSAEncryptionPadding.Pkcs1),
                        Padding = rsaServer.Encrypt(paddingRaw, RSAEncryptionPadding.Pkcs1)
                    };
                    writer.Write(packet, info.AesServer);
                    if (!pRequestKey.KeepOpen)
                    {
                        logger.Debug("Incoming packet has not requested to keep the connection open");
                        request.Close();
                    }
                }
                else if (packet is ClientCreateAccountPacket pCreateAcc)
                {
                    logger.Info($"Attempting to create account (username {pCreateAcc.Username})");

                    if (pCreateAcc.Username.Any(char.IsWhiteSpace))
                    {
                        // Username has a space.
                        packet = new ServerErrorResponsePacket()
                        {
                            ErrorCode = ServerErrorCode.InvalidUsername,
                            Message = "Usernames cannot contain any whitespace characters.",
                        };
                        writer.Write(packet, info.AesServer);
                        logger.Info("Invalid username!");
                    }

                    PrivateUserInfo? user = users.SingleOrDefault(x => x.Username.ToLower() == pCreateAcc.Username.Trim().ToLower());
                    if (user is not null)
                    {
                        // User already exists with that username (case insensitive), invalid.
                        packet = new ServerErrorResponsePacket()
                        {
                            ErrorCode = ServerErrorCode.InvalidUsername,
                            Message = "That username already exists."
                        };
                        writer.Write(packet, info.AesServer);
                        logger.Info("Username is taken!");
                        continue;
                    }

                    user = new()
                    {
                        UserID = Guid.NewGuid(),
                        Username = pCreateAcc.Username,
                        PasswordHashed = pCreateAcc.PasswordHashed,
                    };
                    users.Users.Add(user);
                    logger.Info("Created new user.");
                    users.Save();

                    ConnectedUserInfo connectedUser = new()
                    {
                        UserId = user.UserID,
                        UserInfo = user,
                        TcpConnection = request,
                        PacketWriter = writer,
                        PacketReader = reader
                    };
                    HandleUser(connectedUser);
                }
                else if (packet is ClientLoginPacket pLogin)
                {
                    logger.Info($"Attempting to login as {pLogin.Username}");
                    PrivateUserInfo? user = users.SingleOrDefault(x => x.Username.ToLower() == pLogin.Username.Trim().ToLower());

                    if (user is null || pLogin.PasswordHashed.Equals(user.PasswordHashed))
                    {
                        // Either the username doesn't match or the password doesn't match.
                        packet = new ServerErrorResponsePacket()
                        {
                            ErrorCode = ServerErrorCode.UnknownUser,
                            Message = "The username or password is incorrect.",
                        };
                        writer.Write(packet, info.AesServer);
                        if (user is null) logger.Info("No user with that name exists!");
                        else logger.Info("Password is incorrect!");
                        continue;
                    }
                    logger.Info("Logged in!");

                    ConnectedUserInfo connectedUser = new()
                    {
                        UserId = user.UserID,
                        UserInfo = user,
                        TcpConnection = request,
                        PacketWriter = writer,
                        PacketReader = reader
                    };
                    HandleUser(connectedUser);
                }
                else
                {
                    logger.Warn("Unhandled packet type!");
                    packet = new ServerErrorResponsePacket()
                    {
                        ErrorCode = ServerErrorCode.UnknownPacket,
                        Message = "Unhandled packet type"
                    };
                    writer.Write(packet, info.AesServer);
                    exceptionCount++;
                    continue;
                }
                exceptionCount = 0;
            }
        }
        catch (IOException ex)
        {
            logger.Error($"An IO exception has occurred: {ex.Message}");
            connectedUsers.RemoveAll(x => x.TcpConnection == request);
        }
        catch (Exception ex)
        {
            exceptionCount++;
            logger.Error($"An unknown exception has occurred! {ex.Message}");
            if (exceptionCount < 5)
            {
                logger.Debug("The server will attempt to continue the connection.");
                packet = new ServerErrorResponsePacket()
                {
                    ErrorCode = ServerErrorCode.UnknownError,
                    Message = "A server-side error has occurred."
                };
                writer.Write(packet, info.AesServer);
                goto _readPackets;
            }
            else
            {
                logger.Warn("Too many exceptions! This connection will be closed.");
            }
        }

        logger.Trace($"Connection closed.");
    }

    private static readonly List<ConnectedUserInfo> connectedUsers = new();
    private static void HandleUser(ConnectedUserInfo user)
    {
        Logger logger = logFactory.GetLogger(user.UserInfo.Username);
        INetworkPacket packet = new ServerLoginResponse();

        TcpClient client = user.TcpConnection;
        PacketWriter writer = user.PacketWriter;
        PacketReader reader = user.PacketReader;

        writer.Write(packet, info.AesServer);
        connectedUsers.Add(user);

        while (client.Connected)
        {
            string packetSig;
            lock (reader) packet = reader.ReadPacket(info.AesServer, out packetSig);
            logger.Trace($"Recieved packet {packetSig}");

            if (packet is ClientRequestChatLogPacket pChatLog)
            {
                logger.Info($"Requesting chat log of {pChatLog.Amount} messages ending at {pChatLog.Latest}.");
                int endIndex = messages.Messages.Count - 1;
                while (endIndex >= 0 && messages.Messages[endIndex].SentAt > pChatLog.Latest) endIndex--;
                int startIndex = endIndex - pChatLog.Amount + 1;

                if (startIndex < 0) startIndex = 0;

                MessageInfo[] result = new MessageInfo[endIndex - startIndex + 1];
                messages.Messages.CopyTo(startIndex, result, 0, result.Length);
                
                packet = new ServerChatLogPacket()
                {
                    Messages = result
                };
                lock (writer) writer.Write(packet, info.AesServer);
            }
            else if (packet is ClientRequestUserInfoPacket pUserInfo)
            {
                logger.Info($"Requesting public information for {(pUserInfo.UserId.HasValue ? pUserInfo.UserId : pUserInfo.Username)}");
                PrivateUserInfo? otherUser;
                if (pUserInfo.UserId.HasValue) otherUser = users.SingleOrDefault(x => x.UserID == pUserInfo.UserId);
                else otherUser = users.SingleOrDefault(x => x.Username == pUserInfo.Username);

                if (otherUser is null)
                {
                    // No user found.
                    packet = new ServerErrorResponsePacket()
                    {
                        ErrorCode = ServerErrorCode.UnknownUser,
                        Message = "User not found."
                    };
                    lock (writer) writer.Write(packet, info.AesServer);
                    continue;
                }

                packet = new ServerUserInformationPacket()
                {
                    UserInfo = otherUser.ToPublic()
                };
                lock (writer) writer.Write(packet, info.AesServer);
            }
            else if (packet is ClientSendMessagePacket pMessage)
            {
                MessageInfo message = new()
                {
                    MessageId = Guid.NewGuid(),
                    UserId = user.UserId,
                    SentAt = DateTimeOffset.Now,
                    Contents = pMessage.Content
                };
                messages.Messages.Add(message);

                // Notify users.
                packet = new ServerMessageAlertPacket()
                {
                    Message = message
                };
                foreach (ConnectedUserInfo other in connectedUsers)
                {
                    lock (other.PacketWriter)
                    {
                        if (other.TcpConnection.Connected)
                        {
                            other.PacketWriter.Write(packet, info.AesServer);
                        }
                    }
                }
            }
            else
            {
                logger.Warn("Unhandled packet type!");
                packet = new ServerErrorResponsePacket()
                {
                    ErrorCode = ServerErrorCode.UnknownPacket,
                    Message = "Unhandled packet type"
                };
                lock (writer) writer.Write(packet, info.AesServer);
                continue;
            }
        }

        connectedUsers.Remove(user);
    }
}
