using System;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;
using EmbedIO;
using EmbedIO.WebApi;
using Swan.Logging;

namespace WWWatering_desktop
{
    class Program
    {

        static void Main(string[] args)
        {

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddXmlFile("App.config");
            IConfiguration configuration = builder.Build();

            string serverIP = configuration["AppSettings:SshTunnel:ServerIP"];
            string sshPrivateKeyPath = configuration["AppSettings:SshTunnel:SshPrivateKeyPath"];
            string serverSshUsername = configuration["AppSettings:SshTunnel:ServerSshUsername"];
            int serverSshPort = Convert.ToInt32(configuration["AppSettings:SshTunnel:ServerSshPort"]);
            int remoteTunnelPort = Convert.ToInt32(configuration["AppSettings:SshTunnel:ServerTunnelingPort"]);
            int localTunnelPort = Convert.ToInt32(configuration["AppSettings:SshTunnel:LocalTunnelingPort"]);

            SshTunneler sshTunneler = new SshTunneler(serverIP, sshPrivateKeyPath, serverSshUsername, serverSshPort, remoteTunnelPort, localTunnelPort);
            sshTunneler.StartTunnel();
            Console.WriteLine($"Tunnel between localhost:{remoteTunnelPort} and localhost:{localTunnelPort} started");

            HumidityLogger humidityLogger = new HumidityLogger("humidityLog.txt");

            FileLogger fileLogger = new FileLogger("webServerLog.txt", false);
            Logger.UnregisterLogger<ConsoleLogger>();
            Logger.RegisterLogger(fileLogger);

            int webPort = localTunnelPort;
            string url = $"http://127.0.0.1:{webPort}";

            using (var server = CreateWebServer(url))
            {
                server.Start();

                Console.WriteLine($"Server listening at {url}. Press Enter to stop.");

                var manualResetEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    sshTunneler.Dispose();
                    manualResetEvent.Set(); // Release the main thread
                };
                Console.ReadLine();
            }
        }

        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => 
                o.WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithWebApi("/api", m => m.WithController<APIs>());

            return server;
        }
    }
}
