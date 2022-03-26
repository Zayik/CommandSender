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
        readonly static int MAXSTATES = 10;
        private class PluginSettings
        {
            PluginSettings()
            {
                commands = new List<CommandAction>(MAXSTATES);
                for(int i = 0; i < MAXSTATES; i++)
                {
                    commands.Add(new CommandAction());
                }
            }

            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();

                instance.commands = new List<CommandAction>(MAXSTATES);
                for (int i = 0; i < MAXSTATES; i++)
                {
                    instance.commands.Add(new CommandAction() { IPAddress = "127.0.0.1", Port = 45671 });
                }

                return instance;
            }

            List<CommandAction> commands = new List<CommandAction>(10);

            public int CurrentState = 0;
            public CommandAction CurrentCommandAction { get { return commands[CurrentState]; } }

            [JsonProperty(PropertyName = "desiredStates")]
            public int DesiredStates { get; set; }
            //{
            //    get
            //    {
            //        return desiredStates;
            //    }
            //    set
            //    {
            //        if(value <= 0)
            //            value = 1;
            //        if(value > MAXSTATES)
            //            value = MAXSTATES;
            //        desiredStates = value;
            //    }
            //}

            #region State0
            [JsonProperty(PropertyName = "ipAddress0")]
            public string IPAddress0
            {
                get
                {
                    return commands[0].IPAddress;
                }
                set
                {
                    commands[0].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port0")]
            public int? Port0
            {
                get
                {
                    return commands[0].Port;
                }
                set
                {
                    commands[0].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed0")]
            public string CommandPressed0
            {
                get
                {
                    return commands[0].CommandPressed;
                }
                set
                {
                    commands[0].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased0")]
            public string CommandReleased0
            {
                get
                {
                    return commands[0].CommandReleased;
                }
                set
                {
                    commands[0].CommandReleased = value;
                }
            }
            #endregion

            #region State1
            [JsonProperty(PropertyName = "ipAddress1")]
            public string IPAddress1
            {
                get
                {
                    return commands[1].IPAddress;
                }
                set
                {
                    commands[1].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port1")]
            public int? Port1
            {
                get
                {
                    return commands[1].Port;
                }
                set
                {
                    commands[1].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed1")]
            public string CommandPressed1
            {
                get
                {
                    return commands[1].CommandPressed;
                }
                set
                {
                    commands[1].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased1")]
            public string CommandReleased1
            {
                get
                {
                    return commands[1].CommandReleased;
                }
                set
                {
                    commands[1].CommandReleased = value;
                }
            }
            #endregion

            #region State2
            [JsonProperty(PropertyName = "ipAddress2")]
            public string IPAddress2
            {
                get
                {
                    return commands[2].IPAddress;
                }
                set
                {
                    commands[2].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port2")]
            public int? Port2
            {
                get
                {
                    return commands[2].Port;
                }
                set
                {
                    commands[2].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed2")]
            public string CommandPressed2
            {
                get
                {
                    return commands[2].CommandPressed;
                }
                set
                {
                    commands[2].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased2")]
            public string CommandReleased2
            {
                get
                {
                    return commands[2].CommandReleased;
                }
                set
                {
                    commands[2].CommandReleased = value;
                }
            }
            #endregion

            #region State3
            [JsonProperty(PropertyName = "ipAddress3")]
            public string IPAddress3
            {
                get
                {
                    return commands[3].IPAddress;
                }
                set
                {
                    commands[3].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port3")]
            public int? Port3
            {
                get
                {
                    return commands[3].Port;
                }
                set
                {
                    commands[3].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed3")]
            public string CommandPressed3
            {
                get
                {
                    return commands[3].CommandPressed;
                }
                set
                {
                    commands[3].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased3")]
            public string CommandReleased3
            {
                get
                {
                    return commands[3].CommandReleased;
                }
                set
                {
                    commands[3].CommandReleased = value;
                }
            }
            #endregion

            #region State4
            [JsonProperty(PropertyName = "ipAddress4")]
            public string IPAddress4
            {
                get
                {
                    return commands[4].IPAddress;
                }
                set
                {
                    commands[4].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port4")]
            public int? Port4
            {
                get
                {
                    return commands[4].Port;
                }
                set
                {
                    commands[4].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed4")]
            public string CommandPressed4
            {
                get
                {
                    return commands[4].CommandPressed;
                }
                set
                {
                    commands[4].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased4")]
            public string CommandReleased4
            {
                get
                {
                    return commands[4].CommandReleased;
                }
                set
                {
                    commands[4].CommandReleased = value;
                }
            }
            #endregion

            #region State5
            [JsonProperty(PropertyName = "ipAddress5")]
            public string IPAddress5
            {
                get
                {
                    return commands[5].IPAddress;
                }
                set
                {
                    commands[5].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port5")]
            public int? Port5
            {
                get
                {
                    return commands[5].Port;
                }
                set
                {
                    commands[5].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed5")]
            public string CommandPressed5
            {
                get
                {
                    return commands[5].CommandPressed;
                }
                set
                {
                    commands[5].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased5")]
            public string CommandReleased5
            {
                get
                {
                    return commands[5].CommandReleased;
                }
                set
                {
                    commands[5].CommandReleased = value;
                }
            }
            #endregion

            #region State6
            [JsonProperty(PropertyName = "ipAddress6")]
            public string IPAddress6
            {
                get
                {
                    return commands[6].IPAddress;
                }
                set
                {
                    commands[6].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port6")]
            public int? Port6
            {
                get
                {
                    return commands[6].Port;
                }
                set
                {
                    commands[6].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed6")]
            public string CommandPressed6
            {
                get
                {
                    return commands[6].CommandPressed;
                }
                set
                {
                    commands[6].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased6")]
            public string CommandReleased6
            {
                get
                {
                    return commands[6].CommandReleased;
                }
                set
                {
                    commands[6].CommandReleased = value;
                }
            }
            #endregion

            #region State7
            [JsonProperty(PropertyName = "ipAddress7")]
            public string IPAddress7
            {
                get
                {
                    return commands[7].IPAddress;
                }
                set
                {
                    commands[7].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port7")]
            public int? Port7
            {
                get
                {
                    return commands[7].Port;
                }
                set
                {
                    commands[7].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed7")]
            public string CommandPressed7
            {
                get
                {
                    return commands[7].CommandPressed;
                }
                set
                {
                    commands[7].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased7")]
            public string CommandReleased7
            {
                get
                {
                    return commands[7].CommandReleased;
                }
                set
                {
                    commands[7].CommandReleased = value;
                }
            }
            #endregion

            #region State8
            [JsonProperty(PropertyName = "ipAddress8")]
            public string IPAddress8
            {
                get
                {
                    return commands[8].IPAddress;
                }
                set
                {
                    commands[8].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port8")]
            public int? Port8
            {
                get
                {
                    return commands[8].Port;
                }
                set
                {
                    commands[8].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed8")]
            public string CommandPressed8
            {
                get
                {
                    return commands[8].CommandPressed;
                }
                set
                {
                    commands[8].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased8")]
            public string CommandReleased8
            {
                get
                {
                    return commands[8].CommandReleased;
                }
                set
                {
                    commands[8].CommandReleased = value;
                }
            }
            #endregion

            #region State9
            [JsonProperty(PropertyName = "ipAddress9")]
            public string IPAddress9
            {
                get
                {
                    return commands[9].IPAddress;
                }
                set
                {
                    commands[9].IPAddress = value;
                }
            }

            [JsonProperty(PropertyName = "port9")]
            public int? Port9
            {
                get
                {
                    return commands[9].Port;
                }
                set
                {
                    commands[9].Port = value;
                }
            }

            [JsonProperty(PropertyName = "commandPressed9")]
            public string CommandPressed9
            {
                get
                {
                    return commands[9].CommandPressed;
                }
                set
                {
                    commands[9].CommandPressed = value;
                }
            }

            [JsonProperty(PropertyName = "commandReleased9")]
            public string CommandReleased9
            {
                get
                {
                    return commands[9].CommandReleased;
                }
                set
                {
                    commands[9].CommandReleased = value;
                }
            }
            #endregion
        }

        public class CommandAction
        {
            public string IPAddress { get; set; }

            public int? Port { get; set; }

            public string CommandPressed { get; set; }

            public string CommandReleased { get; set; }
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

            if(!string.IsNullOrEmpty(settings.CurrentCommandAction.CommandPressed) && !string.IsNullOrEmpty(settings.CurrentCommandAction.IPAddress) && settings.CurrentCommandAction.Port.HasValue)
            {
                byte[] data = Encoding.ASCII.GetBytes(settings.CurrentCommandAction.CommandPressed);

                try
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(settings.CurrentCommandAction.IPAddress), settings.CurrentCommandAction.Port.Value);
                    udpClient.Connect(ep);
                    udpClient.Send(data, data.Length);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Released");
            if(!string.IsNullOrEmpty(settings.CurrentCommandAction.CommandReleased) && !string.IsNullOrEmpty(settings.CurrentCommandAction.IPAddress) && settings.CurrentCommandAction.Port.HasValue)
            {
                byte[] data = Encoding.ASCII.GetBytes(settings.CurrentCommandAction.CommandReleased);

                try
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(settings.CurrentCommandAction.IPAddress), settings.CurrentCommandAction.Port.Value);
                    udpClient.Connect(ep);
                    udpClient.Send(data, data.Length);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            settings.CurrentState++;
            if(settings.CurrentState >= settings.DesiredStates)
                settings.CurrentState = 0;
            SetStateAsync((uint)settings.CurrentState);
        }

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

        private Task SetStateAsync(uint state)
        {
            return Connection.SetStateAsync(state);
        }

        #endregion
    }
}