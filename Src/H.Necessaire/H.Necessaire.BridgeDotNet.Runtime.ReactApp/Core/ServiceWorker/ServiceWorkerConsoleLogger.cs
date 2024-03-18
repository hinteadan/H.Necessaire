using Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorker))]
    public static class ServiceWorkerConsoleLogger
    {
        public static void LogDebug(object message)
        {
            Bridge.Script.Call("console.debug", message);
        }

        public static void LogInfo(object message)
        {
            Bridge.Script.Call("console.info", message);
        }

        public static void LogWarning(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Bridge.Script.Call("console.warn", message);
                return;
            }

            Bridge.Script.Call("console.warn", $"{message}. Message: {ex.Message}.{Environment.NewLine}{Environment.NewLine}{ex}");
        }

        public static void LogError(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Bridge.Script.Call("console.error", message);
                return;
            }

            Bridge.Script.Call("console.error", $"{message}. Message: {ex.Message}.{Environment.NewLine}{Environment.NewLine}{ex}");
        }
    }
}
