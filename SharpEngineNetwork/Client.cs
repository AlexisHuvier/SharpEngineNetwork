using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.CSharp.RuntimeBinder;

namespace SharpEngineNetwork;

public class Client
{
    public delegate void RecievePacket(dynamic packet);
    public delegate void Connected();
    public delegate void NetworkError(IPEndPoint endPoint, SocketError error);
    public delegate void PeerDisconnection(NetPeer peer, DisconnectInfo info);
    
    public readonly List<Type> PacketTypes = new();
    private readonly EventBasedNetListener _listener = new();
    private readonly NetManager _client;
    private NetPeer _server;
    public bool IsRunning = false;
    
    public event RecievePacket PacketRecieved;
    public event Connected PeerConnected;
    public event PeerDisconnection PeerDisconnected;
    public event NetworkError ErrorReceived;

    public Client(string ip, int port, string key = "")
    {
        _client = new NetManager(_listener);
        _client.Start();
        _server = _client.Connect(ip, port, key);

        _listener.NetworkReceiveEvent += ReceiveEvent;
        _listener.PeerConnectedEvent += _ => PeerConnected?.Invoke();
        _listener.NetworkErrorEvent += (endpoint, error) => ErrorReceived?.Invoke(endpoint, error);
        _listener.PeerDisconnectedEvent += (peer, info) => PeerDisconnected?.Invoke(peer, info);
        
        IsRunning = true;
    }

    private void ReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
    {
        var packetType = reader.GetString();
        foreach (var type in PacketTypes)
        {
            if (packetType != type.Name) continue;
            
            PacketRecieved?.Invoke(Common.ReadPacket(reader, packetType, type));
            break;
        }
    }

    public void SendPacket<T>(T packet) where T : notnull
    {
        if (!PacketTypes.Contains(packet.GetType()))
            throw new UnknownPacketException($"Packet : {packet.GetType()}");
        Common.SendPacket(_server, packet);
    } 

    public void Update() => _client.PollEvents();

    public void Shutdown()
    {
        _client.Stop();
        IsRunning = false;
    }
}