using System;
using Cysharp.Threading.Tasks;

using xpTURN.Text;

using UnityEngine;

/// <summary>
/// Mock network service for testing and demonstration
/// In production, replace with actual WebSocket/TCP implementation
/// </summary>
public class MockNetworkService : INetworkService
{
    private NetworkState _currentState = NetworkState.Disconnected;
    private string _mockUserId = "user_" + Guid.NewGuid().ToString()[..8];
    private string _mockUserName = "Player";

    public NetworkState CurrentState => _currentState;
    public bool IsConnected => _currentState == NetworkState.Connected;

    public event Action<NetworkState, string> OnStateChanged;
    public event Action<INetworkPacket> OnPacketReceived;

    public async UniTask ConnectAsync(string host, int port)
    {
        if (_currentState == NetworkState.Connected)
            return;

        SetState(NetworkState.Connecting, XString.Format($"Connecting to {host}:{port}..."));

        // Simulate network delay
        await UniTask.Delay(500);

        SetState(NetworkState.Connected, "Connected successfully");
    }

    public async UniTask DisconnectAsync()
    {
        if (_currentState == NetworkState.Disconnected)
            return;

        SetState(NetworkState.Disconnected, "Disconnected");
        await UniTask.CompletedTask;
    }

    public async UniTask SendPacketAsync<T>(T packet) where T : INetworkPacket
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[MockNetworkService] Cannot send packet: not connected");
            return;
        }

        // Simulate network delay
        await UniTask.Delay(100);

        // Handle different packet types
        switch (packet)
        {
            case ChatMessagePacket chatPacket:
                await HandleChatMessage(chatPacket);
                break;
        }
    }

    private async UniTask HandleChatMessage(ChatMessagePacket packet)
    {
        // Simulate server acknowledgment
        await UniTask.Delay(50);

        var ackPacket = new ChatMessageAckPacket
        {
            MessageId = packet.MessageId,
            Success = true
        };
        OnPacketReceived?.Invoke(ackPacket);

        // Simulate server broadcasting the message back (echo)
        await UniTask.Delay(50);

        var broadcastPacket = new ChatBroadcastPacket
        {
            SenderId = _mockUserId,
            SenderName = _mockUserName,
            Message = packet.Message,
            Timestamp = packet.Timestamp
        };
        OnPacketReceived?.Invoke(broadcastPacket);
    }

    private void SetState(NetworkState state, string message)
    {
        _currentState = state;
        OnStateChanged?.Invoke(state, message);
    }

    /// <summary>
    /// Simulate receiving a message from another user (for testing)
    /// </summary>
    public void SimulateIncomingMessage(string senderId, string senderName, string message)
    {
        if (!IsConnected) return;

        var packet = new ChatBroadcastPacket
        {
            SenderId = senderId,
            SenderName = senderName,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        OnPacketReceived?.Invoke(packet);
    }
}
