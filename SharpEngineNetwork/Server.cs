using LiteNetLib;
using LiteNetLib.Utils;

namespace SharpEngineNetwork;

public class Server
{
    public delegate void RecievePacket(NetPeer peer, dynamic packet);

    public delegate void PeerConnection(NetPeer peer);
    public delegate void PeerDisconnection(NetPeer peer, DisconnectInfo info);
    public delegate void UpdateHandler();
    
    public readonly List<Type> PacketTypes = new();
    private readonly EventBasedNetListener _listener = new();
    private readonly NetManager _server;

    public event PeerConnection PeerConnected;
    public event PeerDisconnection PeerDisconnected;
    public event RecievePacket PacketRecieved;
    public event UpdateHandler Update;

    public Server(int port, string key = "")
    {
        _server = new NetManager(_listener);
        _server.Start(port);

        _listener.ConnectionRequestEvent += request => request.AcceptIfKey(key);
        _listener.PeerConnectedEvent += peer => PeerConnected?.Invoke(peer);
        _listener.PeerDisconnectedEvent += (peer, info) => PeerDisconnected?.Invoke(peer, info);
        _listener.NetworkReceiveEvent += NetworkReceive;
    }

    private void NetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
    {
        var packetType = reader.GetString();
        foreach (var type in PacketTypes)
        {
            if (packetType != type.Name) continue;
            
            var numberProp = reader.GetInt();
            dynamic? packet = Activator.CreateInstance(type);
            if(packet == null)
                throw new UnknownPacketException($"Packet : {packetType}");
                    
            for (var i = 0; i < numberProp; i++)
            {
                var propName = reader.GetString();
                var prop = type.GetProperty(propName);
                if (prop == null)
                    throw new UnknownPropertyException($"Property : {propName} - Packet : {packetType}");
                    
                #region Basic Type
        
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(packet, reader.GetString());
                else if(prop.PropertyType == typeof(int))
                    prop.SetValue(packet, reader.GetInt());
                else if(prop.PropertyType == typeof(short))
                    prop.SetValue(packet, reader.GetShort());
                else if(prop.PropertyType == typeof(byte))
                    prop.SetValue(packet, reader.GetByte());
                else if(prop.PropertyType == typeof(char))
                    prop.SetValue(packet, reader.GetChar());
                else if(prop.PropertyType == typeof(float))
                    prop.SetValue(packet, reader.GetFloat());
                else if(prop.PropertyType == typeof(double))
                    prop.SetValue(packet, reader.GetDouble());
                else if(prop.PropertyType == typeof(bool))
                    prop.SetValue(packet, reader.GetBool());

                #endregion

                #region Array Type
        
                else if (prop.PropertyType == typeof(string[]))
                    prop.SetValue(packet, reader.GetStringArray());
                else if(prop.PropertyType == typeof(int[]))
                    prop.SetValue(packet, reader.GetIntArray());
                else if(prop.PropertyType == typeof(short[]))
                    prop.SetValue(packet, reader.GetShortArray());
                else if(prop.PropertyType == typeof(float[]))
                    prop.SetValue(packet, reader.GetFloatArray());
                else if(prop.PropertyType == typeof(double[]))
                    prop.SetValue(packet, reader.GetDoubleArray());
                else if (prop.PropertyType == typeof(bool[]))
                    prop.SetValue(packet, reader.GetBoolArray());

                #endregion
            }
               
            PacketRecieved?.Invoke(peer, packet);
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
        
        var writer = new NetDataWriter();
        var properties = packet.GetType().GetProperties();
        writer.Put(packet.GetType().Name);
        writer.Put(properties.Length);
        foreach (var prop in properties)
        {
            var value = prop.GetValue(packet, null);
            if (value == null)
                continue;
            
            writer.Put(prop.Name);

            #region Basic Type
            
            if (prop.PropertyType == typeof(string))
                writer.Put((string)value);
            else if(prop.PropertyType == typeof(int))
                writer.Put((int)value);
            else if(prop.PropertyType == typeof(short))
                writer.Put((short)value);
            else if(prop.PropertyType == typeof(byte))
                writer.Put((byte)value);
            else if(prop.PropertyType == typeof(char))
                writer.Put((char)value);
            else if(prop.PropertyType == typeof(float))
                writer.Put((float)value);
            else if(prop.PropertyType == typeof(double))
                writer.Put((double)value);
            else if(prop.PropertyType == typeof(bool))
                writer.Put((bool)value);

            #endregion

            #region Array Type
            
            else if (prop.PropertyType == typeof(string[]))
                writer.PutArray((string[])value);
            else if(prop.PropertyType == typeof(int[]))
                writer.PutArray((int[])value);
            else if(prop.PropertyType == typeof(short[]))
                writer.PutArray((short[])value);
            else if(prop.PropertyType == typeof(float[]))
                writer.PutArray((float[])value);
            else if(prop.PropertyType == typeof(double[]))
                writer.PutArray((double[])value);
            else if (prop.PropertyType == typeof(bool[]))
                writer.PutArray((bool[])value);

            #endregion

            else
                throw new UnknownPropertyTypeException($"Type : {prop.PropertyType.Name}");
        }
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
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