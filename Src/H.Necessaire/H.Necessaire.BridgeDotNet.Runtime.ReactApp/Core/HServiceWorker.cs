using Bridge;
using System;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorker), ExportAsNamespace = "")]
    public class HServiceWorker
    {
        [External]
        [Name("self")]
        protected extern static dynamic Self { get; }

        [External]
        [Name("caches")]
        protected extern static dynamic Caches { get; }

        public static void Main()
        {
            new Action(() =>
            {


                if (Self == null)
                    return;

                LogInfo("I'm inside the Service Worker");
                LogInfo(Self);

            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                LogError($"Error occurred while starting {nameof(HServiceWorker)}", ex);
            });
        }


        private static void LogInfo(object message)
        {
            Bridge.Script.Call("console.info", message);
        }

        private static void LogWarning(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Bridge.Script.Call("console.warn", message);
                return;
            }

            Bridge.Script.Call("console.warn", $"{message}. Message: {ex.Message}.{Environment.NewLine}{Environment.NewLine}{ex}");
        }

        private static void LogError(string message, Exception ex = null)
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
