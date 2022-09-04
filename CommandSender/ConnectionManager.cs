using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CommandSender
{
    internal class ConnectionManager
    {
        private UdpClient udpClient;
        private static Dictionary<string, TcpClient> tcpClients = new Dictionary<string, TcpClient>(10);

        public void SendMessage(CommunicationMode communicationMode, string ipAddress, int port, string message, bool canRetry = true)
        {
            var decodedMessage = Regex.Unescape(message);

            byte[] data = Encoding.ASCII.GetBytes(decodedMessage);
            string tcpClientIdentifier = $"{ipAddress}::{port}";

            try
            {

                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                switch(communicationMode)
                {
                    case CommunicationMode.Udp: // Udp
                        udpClient.Connect(ep);
                        udpClient.Send(data, data.Length);
                        break;
                    case CommunicationMode.Tcp: // Tcp
                        if(!tcpClients.ContainsKey(tcpClientIdentifier))
                        {
                            tcpClients.Add(tcpClientIdentifier, new TcpClient() { NoDelay = true });
                        }
                        else
                        {
                            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                            // Make sure connection is still peachy
                            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(tcpClients[tcpClientIdentifier].Client.LocalEndPoint) && x.RemoteEndPoint.Equals(tcpClients[tcpClientIdentifier].Client.RemoteEndPoint)).ToArray();

                            bool connectionGood = false;
                            if(tcpConnections != null && tcpConnections.Length > 0)
                            {
                                TcpState stateOfConnection = tcpConnections.First().State;
                                if(stateOfConnection == TcpState.Established)
                                {
                                    connectionGood = true;
                                }
                            }

                            if(!connectionGood)
                            {
                                // No active tcp Connection to hostName:port
                                tcpClients[tcpClientIdentifier].Close();
                                tcpClients[tcpClientIdentifier].Dispose();
                                tcpClients[tcpClientIdentifier] = new TcpClient() { NoDelay = true };
                            }
                        }

                        if(!tcpClients[tcpClientIdentifier].Connected)
                        {
                            tcpClients[tcpClientIdentifier].Connect(ep);
                        }

                        if(tcpClients[tcpClientIdentifier].Connected)
                        {
                            var tcpStream = tcpClients[tcpClientIdentifier].GetStream();
                            tcpStream.Write(data, 0, data.Length);
                        }

                        break;
                }
            }
            catch(Exception ex)
            {
                // In the event an issue occurred with the socket that would throw an error, ignore it and reset our tcp client. 
                // The last thing we want is for the button to stop working.
                if(communicationMode == CommunicationMode.Tcp)
                {
                    tcpClients[tcpClientIdentifier].Close();
                    tcpClients[tcpClientIdentifier].Dispose();
                    tcpClients[tcpClientIdentifier] = new TcpClient() { NoDelay = true };
                }
                else
                {
                    udpClient.Close();
                    udpClient.Dispose();

                    udpClient = new UdpClient();

                }
                Console.WriteLine(ex.ToString());
            }
        }

        public void InitializeClients()
        {
            udpClient = new UdpClient();
            tcpClients = new Dictionary<string, TcpClient>();
        }

        public void Dispose()
        {
            udpClient.Close();
            udpClient.Dispose();

            foreach(var client in tcpClients.Values)
            {
                client.Close();
                client.Dispose();
            }
        }
    }

    public enum CommunicationMode
    {
        Udp = 0,
        Tcp = 1,
    }
}
