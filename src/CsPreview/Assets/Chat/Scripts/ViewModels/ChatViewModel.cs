using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;
using VitalRouter;

/// <summary>
/// Chat ViewModel - Mediator between Model, View, and Network
/// Handles network packet processing and converts to UI events
/// </summary>
public class ChatViewModel : IDisposable
{
    private readonly ChatModel _model;
    private readonly INetworkService _networkService;
    private readonly ICommandPublisher _publisher;
    private readonly string _currentUserId;

    // Properties that can be bound from the View
    public IReadOnlyList<ChatMessage> Messages => _model.Messages;
    public int MessageCount => _model.MessageCount;
    public bool IsConnected => _networkService.IsConnected;
    public NetworkState ConnectionState => _networkService.CurrentState;

    [Inject]
    public ChatViewModel(ChatModel model, INetworkService networkService, ICommandPublisher publisher)
    {
        _model = model;
        _networkService = networkService;
        _publisher = publisher;
        _currentUserId = "user_" + Guid.NewGuid().ToString()[..8];

        // Subscribe to network events
        _networkService.OnStateChanged += OnNetworkStateChanged;
        _networkService.OnPacketReceived += OnPacketReceived;
    }

    public async UniTask ConnectAsync(string host = "localhost", int port = 8080)
    {
        await _networkService.ConnectAsync(host, port);
    }

    public async UniTask DisconnectAsync()
    {
        await _networkService.DisconnectAsync();
    }

    public async UniTask SendMessageAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || !IsConnected)
            return;

        var messageId = Guid.NewGuid().ToString();
        var timestamp = DateTimeOffset.UtcNow;

        // Add message to local model immediately (optimistic update)
        var chatMessage = new ChatMessage
        {
            Id = messageId,
            SenderId = _currentUserId,
            SenderName = "Me",
            Content = message,
            Timestamp = timestamp,
            IsOwn = true,
            Status = MessageStatus.Pending
        };
        _model.AddMessage(chatMessage);

        // Notify View of new message
        await _publisher.PublishAsync(new ChatMessageReceivedEvent(
            _currentUserId, "Me", message, timestamp.ToUnixTimeMilliseconds()));

        // Send packet to server
        var packet = new ChatMessagePacket
        {
            MessageId = messageId,
            Message = message,
            Timestamp = timestamp.ToUnixTimeMilliseconds()
        };
        await _networkService.SendPacketAsync(packet);
    }

    private void OnNetworkStateChanged(NetworkState state, string message)
    {
        // Publish network state change to View
        _publisher.PublishAsync(new NetworkStateChangedEvent(state, message)).AsUniTask().Forget();
    }

    private void OnPacketReceived(INetworkPacket packet)
    {
        // Route packet to appropriate handler
        switch (packet)
        {
            case ChatBroadcastPacket chatPacket:
                HandleChatBroadcast(chatPacket);
                break;
            case ChatMessageAckPacket ackPacket:
                HandleChatAck(ackPacket);
                break;
        }
    }

    private void HandleChatBroadcast(ChatBroadcastPacket packet)
    {
        // Skip own messages (already added via optimistic update)
        if (packet.SenderId == _currentUserId)
            return;

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = packet.SenderId,
            SenderName = packet.SenderName,
            Content = packet.Message,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(packet.Timestamp),
            IsOwn = false,
            Status = MessageStatus.Delivered
        };
        _model.AddMessage(chatMessage);

        // Notify View of received message
        _publisher.PublishAsync(new ChatMessageReceivedEvent(
            packet.SenderId, packet.SenderName, packet.Message, packet.Timestamp)).AsUniTask().Forget();
    }

    private void HandleChatAck(ChatMessageAckPacket packet)
    {
        // Update message status in model
        foreach (var msg in _model.Messages)
        {
            if (msg.Id == packet.MessageId)
            {
                msg.Status = packet.Success ? MessageStatus.Sent : MessageStatus.Failed;
                break;
            }
        }

        // Notify View of send confirmation
        _publisher.PublishAsync(new ChatMessageSentEvent(packet.MessageId, packet.Success)).AsUniTask().Forget();
    }

    public void Dispose()
    {
        _networkService.OnStateChanged -= OnNetworkStateChanged;
        _networkService.OnPacketReceived -= OnPacketReceived;
    }
}