using BarRaider.SdTools;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandSender
{
    internal class ConnectionManager : IDisposable
    {
        private UdpClient udpClient;
        private readonly TcpConnectionManager tcpConnectionManager;

        public ConnectionManager()
        {
            InitializeClients();
            tcpConnectionManager = new TcpConnectionManager();
        }

        public bool SendMessage(CommunicationMode communicationMode, string ipAddresses, int port, string message, bool canRetry = true)
        {
            // Handle extended ASCII characters
            string pattern = @"\\x([8-9a-fA-F]{2})";
            string output = Regex.Replace(message, pattern, match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());
            var decodedMessage = Regex.Unescape(output);
            byte[] data = Encoding.GetEncoding("latin1").GetBytes(decodedMessage);

            bool allMessagesSuccessful = true;
            var tokens = ipAddresses.Split(';');

            foreach(var ipAddress in tokens)
            {
                allMessagesSuccessful &= communicationMode == CommunicationMode.Udp
                    ? SendUdpMessage(ipAddress, port, data)
                    : SendTcpMessage(ipAddress, port, data).GetAwaiter().GetResult(); // Using GetAwaiter for sync method
            }

            return allMessagesSuccessful;
        }

        private bool SendUdpMessage(string ipAddress, int port, byte[] data)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                udpClient.Connect(ep);
                udpClient.Send(data, data.Length);
                return true;
            }
            catch(Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"UDP Send Error: {ex}");

                udpClient.Close();
                udpClient.Dispose();
                udpClient = new UdpClient();
                return false;
            }
        }

        private async Task<bool> SendTcpMessage(string ipAddress, int port, byte[] data)
        {
            string clientIdentifier = $"{ipAddress}::{port}";
            return await tcpConnectionManager.SendTcpMessage(clientIdentifier, ipAddress, port, data);
        }

        public void InitializeClients()
        {
            udpClient = new UdpClient();
        }

        public void Dispose()
        {
            udpClient?.Close();
            udpClient?.Dispose();
            tcpConnectionManager?.Dispose();
        }
    }

    // TcpConnectionManager class from previous response
    internal class TcpConnectionManager
    {
        private readonly Dictionary<string, TcpClientWrapper> tcpClients = new Dictionary<string, TcpClientWrapper>();
        private readonly object lockObject = new object();

        private class TcpClientWrapper
        {
            public TcpClient Client { get; set; }
            public string HostName { get; set; }
            public int Port { get; set; }
            public bool IsConnecting { get; set; }
        }

        public async Task<bool> SendTcpMessage(string clientIdentifier, string hostName, int port, byte[] message)
        {
            TcpClientWrapper clientWrapper = GetOrCreateClient(clientIdentifier, hostName, port);

            try
            {
                if(!await EnsureConnection(clientWrapper))
                {
                    return false;
                }

                NetworkStream stream = clientWrapper.Client.GetStream();
                await stream.WriteAsync(message, 0, message.Length);
                return true;
            }
            catch(Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Error sending TCP message: {ex.Message}");
                await HandleDisconnection(clientWrapper);
                return false;
            }
        }

        private async Task<bool> EnsureConnection(TcpClientWrapper wrapper)
        {
            lock(lockObject)
            {
                if(wrapper.IsConnecting)
                {
                    return false;
                }
            }

            if(IsConnected(wrapper.Client))
            {
                return true;
            }

            return await Reconnect(wrapper);
        }

        private async Task<bool> Reconnect(TcpClientWrapper wrapper)
        {
            lock(lockObject)
            {
                wrapper.IsConnecting = true;
            }

            try
            {
                wrapper.Client?.Close();
                wrapper.Client?.Dispose();
                wrapper.Client = new TcpClient { NoDelay = true };
                var connectTask = wrapper.Client.ConnectAsync(wrapper.HostName, wrapper.Port);

                if(await Task.WhenAny(connectTask, Task.Delay(5000)) == connectTask)
                {
                    await connectTask;
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Connection attempt failed: {ex.Message}");
                return false;
            }
            finally
            {
                lock(lockObject)
                {
                    wrapper.IsConnecting = false;
                }
            }
        }

        private bool IsConnected(TcpClient client)
        {
            if(client == null || !client.Connected)
                return false;

            try
            {
                return !(client.Client.Poll(1, SelectMode.SelectRead) && client.Client.Available == 0);
            }
            catch
            {
                return false;
            }
        }

        private TcpClientWrapper GetOrCreateClient(string identifier, string hostName, int port)
        {
            lock(lockObject)
            {
                if(!tcpClients.TryGetValue(identifier, out TcpClientWrapper wrapper))
                {
                    wrapper = new TcpClientWrapper
                    {
                        HostName = hostName,
                        Port = port,
                        Client = new TcpClient { NoDelay = true }
                    };
                    tcpClients[identifier] = wrapper;
                }
                return wrapper;
            }
        }

        private async Task HandleDisconnection(TcpClientWrapper wrapper)
        {
            wrapper.Client?.Close();
            wrapper.Client?.Dispose();
            wrapper.Client = new TcpClient { NoDelay = true };
            await Reconnect(wrapper);
        }

        public void Dispose()
        {
            lock(lockObject)
            {
                foreach(var client in tcpClients.Values)
                {
                    client.Client?.Close();
                    client.Client?.Dispose();
                }
                tcpClients.Clear();
            }
        }
    }

    public enum CommunicationMode
    {
        Udp = 0,
        Tcp = 1,
    }
}