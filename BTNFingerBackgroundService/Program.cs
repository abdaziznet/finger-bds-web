using BTNFingerBackgroundService;
using Microsoft.AspNetCore.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(services =>
    {
        services.UseStartup<Worker>();
    }).ConfigureWebHost(config =>
    {
        config.UseUrls("http://localhost:8080");
    })
    .Build();

host.Run();
