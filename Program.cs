using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        var loader = new StorageLoader(Storage.Instance);
        loader.Load("/data/");
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.None))
            .UseUrls("http://0.0.0.0:80")
            .Build();
}

