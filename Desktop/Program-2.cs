/*using System;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;

namespace WWWatering_desktop
{
    class Program
    {
        static void Main(string[] args)
        {

            
            string serverIp = "16.16.115.4";
            int serverPort = 22;
            int remotePort = 8888;
            int localPort = 80; // The port your service is running on
            string username = "ec2-user";
            string privateKeyPath = @"C:\Users\29161\.ssh\AWS-EC2-WWWatering.pem";

            // Load private key for authentication
            PrivateKeyFile privateKeyFile = new PrivateKeyFile(privateKeyPath);
            var keyFiles = new[] { privateKeyFile };

            var connectionInfo = new ConnectionInfo(serverIp, serverPort, username, new PrivateKeyAuthenticationMethod(username, keyFiles));

            // Create a reverse SSH tunnel
            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();

                var reverseTunnel = new ForwardedPortRemote("192.169.99.99", (uint)remotePort, "192.168.100.1", (uint)localPort);
                client.AddForwardedPort(reverseTunnel);
                reverseTunnel.Start();

                Console.WriteLine("Reverse SSH tunnel established. Press any key to close the tunnel...");
                Console.ReadKey();

                reverseTunnel.Stop();
                client.Disconnect();
            }
        }
    }
}
*/