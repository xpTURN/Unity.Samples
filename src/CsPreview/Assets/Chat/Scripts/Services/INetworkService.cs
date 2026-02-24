using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// Interface for network service - abstracts network layer for testability
/// </summary>
public interface INetworkService
{
    NetworkState CurrentState { get; }
    bool IsConnected { get; }

    UniTask ConnectAsync(string host, int port);
    UniTask DisconnectAsync();
    UniTask SendPacketAsync<T>(T packet) where T : INetworkPacket;

    event Action<NetworkState, string> OnStateChanged;
    event Action<INetworkPacket> OnPacketReceived;
}