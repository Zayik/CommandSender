using BarRaider.SdTools;

namespace CommandSender
{
    class Program
    {
        static void Main(string[] args)
        {
            // UIncomment to enable debugging. Note that this will prevent the plugin from running in Stream Deck until a debugger is attached.
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            Logger.Instance.LogMessage(TracingLevel.INFO, "Plugin starting");
            SDWrapper.Run(args);
        }
    }
}
