using Grpc.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using static Messages.EmployeeService;

namespace WpfGrpcClient
{
    class GrpcClient
    {
        public bool SslReady { get; private set; }
        public bool Connected { get; private set; }
        public Messages.EmployeeResponse EResult { get; private set; }
        #region Constructors
        public GrpcClient()
        {
            Environment.SetEnvironmentVariable("GRPC_DNS_RESOLVER", "native");
            SslReady = SetupSsl(caCertLocation: @"certs/ca.crt",
                certLocation: @"certs/client.crt",
                clientKeyLocation: @"certs/client.key");
        }
        #endregion

        #region Helper methods
        internal bool Connect()
        {
            var port = 9000;
            Connected = Connect("DESKTOP-AQ2TIF3", port, _credentials);
            return Connected;
        }

        private bool Connect(string hostUrl, int port, SslCredentials credentials)
        {
            _channel = new Channel(hostUrl, port, credentials);
            _client = new EmployeeServiceClient(_channel);
            try
            {
                Metadata metadata = new Metadata
                {
                    { "purpose", "ping" }
                };

                int badgeNumber = 1;
                var result =  _client.GetByBadgeNumberAsync(new Messages.GetByBadgeNumberRequest() { BadgeNumber = badgeNumber }, metadata);
                
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        internal async Task GetByBadgeAsync(int badgeNumber)
        {
            try
            {
                Metadata metadata = new Metadata
                {
                    {"purpose", "query" }
                };
                EResult = await _client.GetByBadgeNumberAsync(new Messages.GetByBadgeNumberRequest() { BadgeNumber = badgeNumber }, metadata);
                
                //return result.Employee.FirstName;
            }
            catch (Exception e)
            {
                //return $"Something went wrong {e.Message}";
                
            }

        }

        

        private bool SetupSsl(string caCertLocation, string certLocation, string clientKeyLocation)
        {
            try
            {
                if (string.IsNullOrEmpty(caCertLocation))
                {
                    throw new ArgumentException("message", nameof(caCertLocation));
                }

                if (string.IsNullOrEmpty(certLocation))
                {
                    throw new ArgumentException("message", nameof(certLocation));
                }

                if (string.IsNullOrEmpty(clientKeyLocation))
                {
                    throw new ArgumentException("message", nameof(clientKeyLocation));
                }

                _caCert = File.ReadAllText(caCertLocation);
                _cert = File.ReadAllText(certLocation);
                _key = File.ReadAllText(clientKeyLocation);

                _keyPair = new KeyCertificatePair(_cert, _key);
                _credentials = new SslCredentials(_caCert, _keyPair);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

            
        }

     
        #endregion
        #region Private fields
        string _caCert;
        string _cert;
        string _key;
        KeyCertificatePair _keyPair;
        SslCredentials _credentials;
        private ChannelBase _channel;
        EmployeeServiceClient _client;
        #endregion
    }
}
