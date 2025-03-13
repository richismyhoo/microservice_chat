using System;
using ChatToDb.Chat;
using ChatToDb.Context;
using ChatToDb.DataLayer;
using ChatToDb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during startup: {ex.Message}");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(connectionString));
                    
                    services.AddScoped<ChatRepository>();
                    
                    services.AddSingleton<IHostedService, RabbitListenerService>();
                    
                    services.AddSingleton(provider =>
                    {
                        var rabbitService = RabbitConnectionService.CreateAsync("localhost").GetAwaiter().GetResult();
                        return rabbitService;
                    });
                    
                    services.AddGrpc();
                });

                webBuilder.Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<ChatHistoryService>();
                        
                        endpoints.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client...");
                    });
                });
                
                webBuilder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5230, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });

                webBuilder.UseUrls("http://localhost:5230");
            });
}
