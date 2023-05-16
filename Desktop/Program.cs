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
        private static string _serverIP;
        private static string _sshPrivateKeyPath;
        private static string _serverSshUsername;
        private static int _serverSshPort;
        private static int _remoteTunnelPort;
        private static int _localTunnelPort;

        private static string _humidityLogPath;
        private static string _webServerLogPath;
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddXmlFile("App.config");
            IConfiguration configuration = builder.Build();

            LoadConfig(configuration);

            var tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            HumidityLogger humidityLogger = new HumidityLogger(_humidityLogPath);
            humidityLogger.StartLogging();

            FileLogger fileLogger = new FileLogger(_webServerLogPath, false);
            Logger.UnregisterLogger<ConsoleLogger>();
            Logger.RegisterLogger(fileLogger);

            int webPort = _localTunnelPort;
            string url = $"http://127.0.0.1:{webPort}";

            using (var server = CreateWebServer(url))
            {
                server.Start();

                Console.WriteLine($"Server listening at {url}.");

                SshTunneler sshTunneler = new SshTunneler(_serverIP, _sshPrivateKeyPath, _serverSshUsername, _serverSshPort, _remoteTunnelPort, _localTunnelPort);
                Task sshTunnelTask = Task.Run(async () => await sshTunneler.StartTunnelAsync(cancellationToken));
                Console.WriteLine($"Tunnel between localhost:{_remoteTunnelPort} and localhost:{_localTunnelPort} started");
                Console.WriteLine("Press any key to stop");

                Console.ReadLine();
                humidityLogger.StopLogging();
                tokenSource.Cancel();
            }
        }

        private static void LoadConfig(IConfiguration configuration)
        {
            _serverIP = configuration["AppSettings:SshTunnel:ServerIP"];
            _sshPrivateKeyPath = configuration["AppSettings:SshTunnel:SshPrivateKeyPath"];
            _serverSshUsername = configuration["AppSettings:SshTunnel:ServerSshUsername"];
            _serverSshPort = Convert.ToInt32(configuration["AppSettings:SshTunnel:ServerSshPort"]);
            _remoteTunnelPort = Convert.ToInt32(configuration["AppSettings:SshTunnel:ServerTunnelingPort"]);
            _localTunnelPort = Convert.ToInt32(configuration["AppSettings:SshTunnel:LocalTunnelingPort"]);

            _humidityLogPath = configuration["AppSettings:Logging:HumidityLogPath"];
            _webServerLogPath = configuration["AppSettings:Logging:WebServerLogPath"];
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
