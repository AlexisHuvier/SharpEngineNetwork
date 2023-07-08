using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.CSharp.RuntimeBinder;

namespace SharpEngineNetwork;

public class Server
{
    public delegate void RecievePacket(NetPeer peer, dynamic packet);

    public delegate void PeerConnection(NetPeer peer);
    public delegate void PeerDisconnection(NetPeer peer, DisconnectInfo info);
    public delegate void UpdateHandler();
    public delegate void NetworkError(IPEndPoint endPoint, SocketError error);
    
    public readonly List<Type> PacketTypes = new();
    private readonly EventBasedNetListener _listener = new();
    private readonly NetManager _server;

    public event PeerConnection PeerConnected;
    public event PeerDisconnection PeerDisconnected;
    public event RecievePacket PacketRecieved;
    public event UpdateHandler Update;
    public event NetworkError ErrorReceived;

    public Server(int port, string key = "")
    {
        _server = new NetManager(_listener);
        _server.Start(port);

        _listener.ConnectionRequestEvent += request => request.AcceptIfKey(key);
        _listener.PeerConnectedEvent += peer => PeerConnected?.Invoke(peer);
        _listener.PeerDisconnectedEvent += (peer, info) => PeerDisconnected?.Invoke(peer, info);
        _listener.NetworkReceiveEvent += NetworkReceive;
        _listener.NetworkErrorEvent += (point, error) => ErrorReceived?.Invoke(point, error);
    }

    private void NetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
    {
        var packetType = reader.GetString();
        foreach (var type in PacketTypes)
        {
            if (packetType != type.Name) continue;
            
            PacketRecieved?.Invoke(peer, Common.ReadPacket(reader, packetType, type));
            break;
        }
    }

    public void BroadcastPacket<T>(T packet) where T : notnull
    {
        foreach(var peer in _server.ConnectedPeerList)
            SendPacket(packet, peer);
    }

    public void SendPacket<T>(T packet, NetPeer peer) where T: notnull
    {
        if (!PacketTypes.Contains(packet.GetType()))
            throw new UnknownPacketException($"Packet : {packet.GetType()}");
        Common.SendPacket(peer, packet);
    }
    
    public void Start()
    {
        while (!Console.KeyAvailable)
        {
            _server.PollEvents();
            Update?.Invoke();
            Thread.Sleep(15);
        }
        
        _server.Stop();
    }
}