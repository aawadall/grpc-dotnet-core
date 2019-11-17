using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Messages.EmployeeService;
using Messages;

namespace GrpcClient
{
    class Program
    {
        const int Port = 9000;
        static void Main(string[] args)
        {
            var option = 1; // int.Parse(args[0]);

            var cacert = File.ReadAllText(@"certs/ca.crt");
            var cert = File.ReadAllText(@"certs/client.crt");
            var key = File.ReadAllText(@"certs/client.key");
            var keypair = new KeyCertificatePair(cert, key);
            SslCredentials creds = new SslCredentials(cacert, keypair);
            var channel = new Channel("DESKTOP-AQ2TIF3", Port, creds);
            var client = new EmployeeServiceClient(channel);

            switch (option)
            {
                case 1:
                    SendMetadataAsync(client).Wait();
                    break;
                case 2:
                    GetByBadgeNumber(client).Wait();
                    break;
                case 3:
                    GetAll(client).Wait();
                    break;
                case 4:
                    AddPhoto(client).Wait();
                    break;
                

            }
        }

        public static async Task SendMetadataAsync(EmployeeServiceClient client)
        {
            Console.WriteLine($"Establishing hollow connection using:");
            
            Metadata metadata = new Metadata();
            metadata.Add("username", "username1");
            metadata.Add("password", "password1");

            foreach (var item in metadata)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
            try
            {
                await client.GetByBadgeNumberAsync(new Messages.GetByBadgeNumberRequest(), metadata);
            }
            catch (Exception e)
            {
                // Just swallow the expected exception
            }

        }
        public static async Task GetByBadgeNumber(EmployeeServiceClient client)
        {
            var res = await client.GetByBadgeNumberAsync(new Messages.GetByBadgeNumberRequest() { BadgeNumber = 2080 });
            Console.WriteLine(res.Employee);
        }

        public static async Task GetAll(EmployeeServiceClient client)
        {
            using (var call = client.GetAll(new Messages.GetAllRequest()))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine(responseStream.Current.Employee);
                }
            }
        }

        public static async Task AddPhoto(EmployeeServiceClient client)
        {
            Metadata md = new Metadata();
            md.Add("badgenumber", "2080");

            FileStream fs = File.OpenRead("Penguins.jpg");
            using (var call = client.AddPhoto())
            {
                var stream = call.RequestStream;
                while (true)
                {
                    byte[] buffer = new byte[64 * 1024];
                    int numRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                    if (numRead == 0)
                    {
                        break;
                    }
                    if (numRead < buffer.Length)
                    {
                        Array.Resize(ref buffer, numRead);
                    }

                    await stream.WriteAsync(new Messages.AddPhotoRequest() { Data = ByteString.CopyFrom(buffer) });
                }
                await stream.CompleteAsync();

                var res = await call.ResponseAsync;

                Console.WriteLine(res.IsOk);
            }

        }

        
    }
}
