using SENetwork_DemoCommon;
using SENetwork_DemoCommon.Packet;
using SharpEngineNetwork;

namespace SENetwork_DemoClient;

public class DemoClient: Client
{
    public DemoClient() : base("localhost", 5000, "DEMO-KEY")
    {
        this.AddPackets();

        PeerConnected += () =>
        {
            Console.WriteLine("CONNECTED, SEND PACKET...");
            SendPacket(new DemoPacket
            {
                Info = "Client Info"
            });
        };
        
        PacketRecieved += packet =>
        {
            if (packet is Demo2Packet demo2Packet)
                Console.WriteLine($"DEMO 2 PACKED RECIEVED : {demo2Packet.Info}");
        };

        ErrorReceived += (point, error) => Console.WriteLine($"Error from {point} : {error}");

        PeerDisconnected += (peer, info) => Console.WriteLine($"OMG Disconnected from {peer.EndPoint} : {info.Reason}");
    }
}