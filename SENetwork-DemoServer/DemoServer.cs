using SENetwork_DemoCommon;
using SENetwork_DemoCommon.Packet;
using SharpEngineNetwork;

namespace SENetwork_DemoServer;

public class DemoServer: Server
{
    public DemoServer() : base(5000, "DEMO-KEY")
    {
        this.AddPackets();

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