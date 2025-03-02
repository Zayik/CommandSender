using BarRaider.SdTools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        /// <summary>
        /// Sends a message to multiple IP addresses and returns success status along with any error messages.
        /// </summary>
        public (bool allSuccessful, List<string> errors) SendMessage(CommunicationMode communicationMode, string ipAddresses, int port, string message, bool canRetry = true)
        {
            // Handle extended ASCII characters
            string pattern = @"\\x([8-9a-fA-F]{2})";
            string output = Regex.Replace(message, pattern, match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());
            var decodedMessage = Regex.Unescape(output);
            byte[] data = Encoding.GetEncoding("latin1").GetBytes(decodedMessage);

            bool allMessagesSuccessful = true;
            List<string> errors = new List<string>();
            var tokens = ipAddresses.Split(';');

            foreach(var ipAddress in tokens)
            {
                if(communicationMode == CommunicationMode.Udp)
                {
                    var (success, error) = SendUdpMessage(ipAddress, port, data);
                    if(!success)
                    {
                        allMessagesSuccessful = false;
                        errors.Add($"UDP to {ipAddress}:{port} - {error}");
                    }
                }
                else
                {
                    var (success, error) = SendTcpMessage(ipAddress, port, data).GetAwaiter().GetResult(); // Synchronous call for simplicity
                    if(!success)
                    {
                        allMessagesSuccessful = false;
                        errors.Add($"TCP to {ipAddress}:{port} - {error}");
                    }
                }
            }

            return (allMessagesSuccessful, errors);
        }

        /// <summary>
        /// Sends a UDP message and returns success status with error message if failed.
        /// </summary>
        private (bool success, string errorMessage) SendUdpMessage(string ipAddress, int port, byte[] data)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                udpClient.Connect(ep);
                udpClient.Send(data, data.Length);
                return (true, null);
            }
            catch(Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"UDP Send Error: {ex}");
                udpClient.Close();
                udpClient.Dispose();
                udpClient = new UdpClient();
                return (false, ex.Message); // Return the specific error message
            }
        }

        /// <summary>
        /// Sends a TCP message asynchronously and returns success status with error message if failed.
        /// </summary>
        private async Task<(bool success, string errorMessage)> SendTcpMessage(string ipAddress, int port, byte[] data)
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

        /// <summary>
        /// Sends a TCP message and returns success status with error message if failed.
        /// </summary>
        public async Task<(bool success, string errorMessage)> SendTcpMessage(string clientIdentifier, string hostName, int port, byte[] message)
        {
            TcpClientWrapper clientWrapper = GetOrCreateClient(clientIdentifier, hostName, port);

            try
            {
                string msg = Encoding.GetEncoding("latin1").GetString(message);
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Tcp Client sending message: {msg}.");

                var (connectionSuccess, connectionError) = await EnsureConnection(clientWrapper);
                if(!connectionSuccess)
                {
                    Logger.Instance.LogMessage(TracingLevel.INFO, $"Connection not ensured: {connectionError}");
                    return (false, connectionError);
                }

                Logger.Instance.LogMessage(TracingLevel.INFO, "Tcp Client getting stream.");
                NetworkStream stream = clientWrapper.Client.GetStream();
                Logger.Instance.LogMessage(TracingLevel.INFO, "Tcp Client sending message Start.");
                await stream.WriteAsync(message, 0, message.Length);
                Logger.Instance.LogMessage(TracingLevel.INFO, "Tcp Client sending message Complete.");
                return (true, null);
            }
            catch(Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Error sending TCP message: {ex.Message}");
                await HandleDisconnection(clientWrapper);
                return (false, ex.Message); // Return the specific error message
            }
        }

        /// <summary>
        /// Ensures a TCP connection is active, returning success status and error message if failed.
        /// </summary>
        private async Task<(bool success, string errorMessage)> EnsureConnection(TcpClientWrapper wrapper)
        {
            lock(lockObject)
            {
                if(wrapper.IsConnecting)
                {
                    return (false, "Another connection attempt is already in progress.");
                }
            }

            if(IsConnected(wrapper.Client))
            {
                return (true, null);
            }

            return await Reconnect(wrapper);
        }

        /// <summary>
        /// Attempts to reconnect a TCP client, returning success status and error message if failed.
        /// </summary>
        private async Task<(bool success, string errorMessage)> Reconnect(TcpClientWrapper wrapper)
        {
            lock(lockObject)
            {
                wrapper.IsConnecting = true;
            }

            try
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Reconnecting.");

                wrapper.Client?.Close();
                wrapper.Client?.Dispose();
                wrapper.Client = new TcpClient { NoDelay = true };
                var connectTask = wrapper.Client.ConnectAsync(wrapper.HostName, wrapper.Port);

                if(await Task.WhenAny(connectTask, Task.Delay(5000)) == connectTask)
                {
                    await connectTask;
                    return (true, null);
                }
                else
                {
                    return (false, "Connection timed out after 5 seconds.");
                }
            }
            catch(Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Connection attempt failed: {ex.Message}");
                return (false, ex.Message); // Return the specific error message
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
            Logger.Instance.LogMessage(TracingLevel.INFO, "Tcp Client Disconnecting.");

            wrapper.Client?.Close();
            wrapper.Client?.Dispose();
            wrapper.Client = new TcpClient { NoDelay = true };
            await Reconnect(wrapper);
        }

        public void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Tcp Client Disposing.");

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