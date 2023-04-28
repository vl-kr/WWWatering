using System;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;
using EmbedIO;
using EmbedIO.WebApi;

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

            int webPort = localTunnelPort;
            string url = $"http://127.0.0.1:{webPort}";

            using (var server = CreateWebServer(url))
            {
                server.Start();

                Console.WriteLine($"Server listening at {url}. Press Enter to stop.");
                Console.ReadLine();
            }

        }

        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o.WithUrlPrefix(url).WithMode(HttpListenerMode.EmbedIO))
                .WithWebApi("/api", m => m.WithController<APIs>());

            return server;
        }
    }
}
