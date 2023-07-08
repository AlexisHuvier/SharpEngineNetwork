using SENetwork_DemoServer.Packet;
using SharpEngineNetwork;

namespace SENetwork_DemoServer;

public class DemoServer: Server
{
    public DemoServer() : base(5000, "DEMO-KEY")
    {
        PacketTypes.Add(typeof(DemoPacket));
        PacketTypes.Add(typeof(Demo2Packet));

        PeerConnected += peer => Console.WriteLine($"CONNECTION : {peer.EndPoint}");

        PacketRecieved += (peer, packet) =>
        {
            if (packet is DemoPacket)
            {
                Console.WriteLine("DEMO PACKED RECIEVED");
                BroadcastPacket(new Demo2Packet());
            }
        };
        
        Start();
    }
}