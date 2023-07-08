using SENetwork_DemoClient.Packet;
using SharpEngineNetwork;

namespace SENetwork_DemoClient;

public class DemoClient: Client
{
    public DemoClient() : base("localhost", 5000, "DEMO-KEY")
    {
        PacketTypes.Add(typeof(DemoPacket));
        PacketTypes.Add(typeof(Demo2Packet));

        PeerConnected += () =>
        {
            Console.WriteLine("CONNECTED, SEND PACKET...");
            SendPacket(new DemoPacket());
        };
        
        PacketRecieved += packet =>
        {
            if (packet is Demo2Packet)
                Console.WriteLine("DEMO 2 PACKED RECIEVED");
        };
    }
}