using System;
using Renci.SshNet;
using System.Threading.Tasks;

public class SshServer
{
    private SshClient _client;

    public SshServer(string host, int port, string username, string password)
    {
        _client = new SshClient(host, port, username, password);
    }

    public async Task ConnectAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                _client.Connect();
                Console.WriteLine("SSH connection established");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting via SSH: " + ex.Message);
            }
        });
    }

    public void Disconnect()
    {
        _client.Disconnect();
    }
}
