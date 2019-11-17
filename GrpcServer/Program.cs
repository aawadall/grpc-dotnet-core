using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace GrpcServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int Port = 9000;
            var cacert = File.ReadAllText(@"ca.crt");
            var cert = File.ReadAllText(@"server.crt");
            var key = File.ReadAllText(@"server.key");

            var keypair = new KeyCertificatePair(cert, key);

            var sslCredentials = new SslServerCredentials(new List<KeyCertificatePair>()
            {
                keypair
            }, cacert, forceClientAuth:false);

            Server server = new Server
            {
                Ports = { new ServerPort("0.0.0.0", Port, sslCredentials) }
            };

            server.Start();

            Console.WriteLine($"Starting Server on port {Port}");
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
