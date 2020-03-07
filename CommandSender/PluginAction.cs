using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommandSender
{
    [PluginActionId("com.biffmasterzay.commandsender")]
    public class PluginAction : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();
                instance.IPAddress = "127.0.0.1";
                instance.Port = 45671;
                return instance;
            }

            [JsonProperty(PropertyName = "ipAddress")]
            public string IPAddress { get; set; }

            [JsonProperty(PropertyName = "port")]
            public int Port { get; set; }

            [JsonProperty(PropertyName = "command")]
            public string Command { get; set; }
        }

        #region Private Members

        private PluginSettings settings;

        private UdpClient udpClient;

        #endregion
        public PluginAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if(payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
            udpClient = new UdpClient();
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
            udpClient.Close();
            udpClient.Dispose();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");

            byte[] data = Encoding.ASCII.GetBytes("Hello World");

            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(settings.IPAddress), settings.Port);
                udpClient.Connect(ep);
                udpClient.Send(data, data.Length);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override void KeyReleased(KeyPayload payload) { }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}