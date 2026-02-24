using System;
using System.Collections.Generic;
using VContainer;

/// <summary>
/// Chat Model - Contains chat data and business logic
/// </summary>
public class ChatModel
{
    private readonly List<ChatMessage> _messages = new();
    private readonly int _maxMessages = 100;

    public IReadOnlyList<ChatMessage> Messages => _messages;
    public int MessageCount => _messages.Count;

    public void AddMessage(ChatMessage message)
    {
        _messages.Add(message);

        // Remove old messages if exceeding max
        while (_messages.Count > _maxMessages)
        {
            _messages.RemoveAt(0);
        }
    }

    public void Clear()
    {
        _messages.Clear();
    }
}

/// <summary>
/// Represents a single chat message
/// </summary>
public class ChatMessage
{
    public string Id { get; init; } = string.Empty;
    public string SenderId { get; init; } = string.Empty;
    public string SenderName { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; }
    public bool IsOwn { get; init; }
    public MessageStatus Status { get; set; } = MessageStatus.Pending;
}

public enum MessageStatus
{
    Pending,
    Sent,
    Delivered,
    Failed
}