namespace SENetwork_DemoClient;

internal static class Program
{
    private static void Main(string[] args)
    {
        var demoClient = new DemoClient();

        while (!Console.KeyAvailable)
            demoClient.Update();
        
        demoClient.Shutdown();
    }
}
