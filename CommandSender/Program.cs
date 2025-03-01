using BarRaider.SdTools;

namespace CommandSender
{
    class Program
    {
        static void Main(string[] args)
        {
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            var pluginFile = Path.Combine("C:\\Users\\genol\\AppData\\Roaming\\Elgato\\StreamDeck\\logs", "startup.log");
            File.AppendAllText(pluginFile, $"{DateTime.UtcNow}: Plugin starting\n");
            Logger.Instance.LogMessage(TracingLevel.INFO, "Plugin starting");

            try
            {
                using(var mgr = new ConnectionManager()) // Your class
                {
                    // Assuming PluginAction is your action class
                    //PluginAction plugin = new PluginAction();
                    File.AppendAllText(pluginFile, $"{DateTime.UtcNow}: Connecting to Stream Deck\n");
                    Logger.Instance.LogMessage(TracingLevel.INFO, "Connecting to Stream Deck\n");

                    SDWrapper.Run(args);

                }
            }
            catch(Exception ex)
            {
                File.AppendAllText(pluginFile, $"{DateTime.UtcNow}: Error: {ex}\n");
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"{DateTime.UtcNow}: Error: {ex}\n");

            }
        }
    }
}
