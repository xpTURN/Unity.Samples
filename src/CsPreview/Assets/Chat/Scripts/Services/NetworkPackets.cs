using System;

/// <summary>
/// Base interface for all network packets
/// </summary>
public interface INetworkPacket
{
    ushort PacketId { get; }
}

/// <summary>
/// Packet IDs for network communication
/// </summary>
public static class PacketIds
{
    public const ushort ChatMessage = 1001;
    public const ushort ChatMessageAck = 1002;
    public const ushort UserJoin = 1003;
    public const ushort UserLeave = 1004;
    public const ushort Ping = 9001;
    public const ushort Pong = 9002;
}

/// <summary>
/// Chat message packet sent to server
/// </summary>
[Serializable]
public class ChatMessagePacket : INetworkPacket
{
    public ushort PacketId => PacketIds.ChatMessage;
    public string MessageId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}

/// <summary>
/// Chat message acknowledgment from server
/// </summary>
[Serializable]
public class ChatMessageAckPacket : INetworkPacket
{
    public ushort PacketId => PacketIds.ChatMessageAck;
    public string MessageId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Chat message received from server (broadcast)
/// </summary>
[Serializable]
public class ChatBroadcastPacket : INetworkPacket
{
    public ushort PacketId => PacketIds.ChatMessage;
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}

/// <summary>
/// User join notification
/// </summary>
[Serializable]
public class UserJoinPacket : INetworkPacket
{
    public ushort PacketId => PacketIds.UserJoin;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// User leave notification
/// </summary>
[Serializable]
public class UserLeavePacket : INetworkPacket
{
    public ushort PacketId => PacketIds.UserLeave;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}