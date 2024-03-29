﻿using Grpc.Core;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Messages.EmployeeService;

namespace GrpcServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int Port = 9000;
            var cacert = File.ReadAllText(@"certs/ca.crt");
            var cert = File.ReadAllText(@"certs/server.crt");
            var key = File.ReadAllText(@"certs/server.key");

            var keypair = new KeyCertificatePair(cert, key);

            var sslCredentials = new SslServerCredentials(new List<KeyCertificatePair>()
            {
                keypair
            }, cacert, false);

            Server server = new Server
            {
                Ports = { //new ServerPort("0.0.0.0", Port, sslCredentials),
                new ServerPort("[::]", Port, sslCredentials) } // listening on IP6 port 9000 
                
            };
            server.Services.Add(BindService(new EmployeeService()));
            foreach (var service in server.Services)
            {
                Console.WriteLine($"Service: {service}");
            }
            server.Start();
            
            Console.WriteLine($"Starting Server on port {Port}");
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }

        public class EmployeeService : EmployeeServiceBase
        {
            public EmployeeService()
            {
                Console.WriteLine($"new service {this.GetType().Name}");
            }
            public override async Task<EmployeeResponse> GetByBadgeNumber(GetByBadgeNumberRequest request, ServerCallContext context)
            {
                Console.WriteLine($"Service invoked");
                Metadata metadata = context.RequestHeaders;
                foreach (var entry in metadata)
                {
                    Console.WriteLine(entry.Key + ": " + entry.Value);
                }

                var result = Employees.employees.FirstOrDefault(e => e.BadgeNumber == request.BadgeNumber);

                if(result == null)
                {
                    throw new Exception($"Employee not found with Badge Number {request.BadgeNumber}");
                }
                return new EmployeeResponse()
                {
                    Employee = result
                };   
            }

            public override async Task GetAll(GetAllRequest request, IServerStreamWriter<EmployeeResponse> responseStream, ServerCallContext context)
            {
                foreach (var employee in Employees.employees)
                {
                    await responseStream.WriteAsync(new EmployeeResponse()
                    {
                        Employee = employee
                    });
                }

            }
        }
    }
}
