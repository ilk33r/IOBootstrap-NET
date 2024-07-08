
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Default.Batch;

public class Program
{
    private static DefaultStartup? Startup;
    
    static void Main(string[] args)
    {
        // Initialize
        Startup = new DefaultStartup(args);
        
        // Register processes
        Startup.RegisterProcess(typeof(IOPushSenderDefaultProcess));

        // Start batch
        Startup.Run();
    }
}
