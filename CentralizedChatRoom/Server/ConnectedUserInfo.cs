using ChatRoom.Centralized.Shared;
using ChatRoom.Centralized.Shared.ObjectModels;
using System.Net.Sockets;

namespace ChatRoom.Centralized.Server;

public class ConnectedUserInfo
{
    public required Guid UserId { get; set; }
    public required PrivateUserInfo UserInfo { get; set; }
    public required TcpClient TcpConnection { get; set; }
    public required PacketWriter PacketWriter { get; set; }
    public required PacketReader PacketReader { get; set; }

    public readonly object LOCK = new();
}
