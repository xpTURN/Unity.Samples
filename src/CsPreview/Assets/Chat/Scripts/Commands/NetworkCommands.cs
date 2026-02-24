using VitalRouter;

/// <summary>
/// Network connection state change event
/// </summary>
public readonly record struct NetworkStateChangedEvent : ICommand
{
    public NetworkState State { get; init; }
    public string Message { get; init; }

    public NetworkStateChangedEvent(NetworkState state, string message = "")
    {
        State = state;
        Message = message;
    }
}

/// <summary>
/// Chat message received from server
/// </summary>
public readonly record struct ChatMessageReceivedEvent : ICommand
{
    public string SenderId { get; init; }
    public string SenderName { get; init; }
    public string Message { get; init; }
    public long Timestamp { get; init; }

    public ChatMessageReceivedEvent(string senderId, string senderName, string message, long timestamp)
    {
        SenderId = senderId;
        SenderName = senderName;
        Message = message;
        Timestamp = timestamp;
    }
}

/// <summary>
/// Chat message sent confirmation
/// </summary>
public readonly record struct ChatMessageSentEvent : ICommand
{
    public string MessageId { get; init; }
    public bool Success { get; init; }

    public ChatMessageSentEvent(string messageId, bool success)
    {
        MessageId = messageId;
        Success = success;
    }
}

/// <summary>
/// Network connection states
/// </summary>
public enum NetworkState
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
    Error
}
