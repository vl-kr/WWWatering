using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;


namespace WWWatering_desktop
{
    public class SshTunneler : IDisposable
    {
        private int _remoteTunnelPort;
        private int _localTunnelPort;

        private PrivateKeyFile _privateKeyFile;
        private ConnectionInfo _connectionInfo;

        private SshClient? _client;
        private ForwardedPortRemote? _reverseTunnel;

        public SshTunneler(string serverIp, string sshPrivateKeyPath, string serverSshUsername,
            int serverSshPort, int remoteTunnelPort, int localTunnelPort)
        {
            _remoteTunnelPort = remoteTunnelPort;
            _localTunnelPort = localTunnelPort;

            _privateKeyFile = new PrivateKeyFile(sshPrivateKeyPath);
            PrivateKeyFile[] keyFiles = new[] { _privateKeyFile };
            _connectionInfo = new ConnectionInfo(serverIp, serverSshPort, serverSshUsername, new PrivateKeyAuthenticationMethod(serverSshUsername, keyFiles));
        }

        public void StartTunnel()
        {
            if(_client != null)
            {
                throw new Exception("Tunnel already started");
            }
            _client = new SshClient(_connectionInfo);
            _client.Connect();
            _reverseTunnel = new ForwardedPortRemote("127.0.0.1", (uint)_remoteTunnelPort, "127.0.0.1", (uint)_localTunnelPort);
            _client.AddForwardedPort(_reverseTunnel);
            _reverseTunnel.Start();
        }

        public void StopTunnel()
        {
            _reverseTunnel?.Stop();
            _client?.Disconnect();
        }

        public void Dispose()
        {
            StopTunnel();
            _reverseTunnel?.Dispose();
            _client?.Dispose();
        }

    }
}
