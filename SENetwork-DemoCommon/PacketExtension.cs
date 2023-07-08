using SENetwork_DemoCommon.Packet;
using SharpEngineNetwork;

namespace SENetwork_DemoCommon;

public static class PacketExtension
{
    public static void AddPackets(this Server server)
    {
        server.PacketTypes.Add(typeof(DemoPacket));
        server.PacketTypes.Add(typeof(Demo2Packet));
    }

    public static void AddPackets(this Client client)
    {
        client.PacketTypes.Add(typeof(DemoPacket));
        client.PacketTypes.Add(typeof(Demo2Packet));
    }
}