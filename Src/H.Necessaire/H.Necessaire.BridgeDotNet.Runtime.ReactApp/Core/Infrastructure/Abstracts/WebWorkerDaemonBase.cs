using Bridge.Html5;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class WebWorkerDaemonBase : ImADaemon
    {
        public WebWorkerDaemonBase(Func<ImAWebWorkerDaemonAction> workerFactory, Func<ImAnAppWireup> webWorkerWireup = null, params string[] scriptsToInclude)
        {
            this.daemonWorker = workerFactory;
            this.webWorkerWireup = webWorkerWireup;
            this.scriptsToInclude = scriptsToInclude ?? new string[0];
            this.workerJsCode = new Blob(new BlobDataObject [] { PrintWorkerJavaScript().Replace("\"use strict\";", string.Empty) }, new BlobPropertyBag { Type = "application/javascript; charset=utf-8" });
            this.workerUrl = Retyped.dom.URL.createObjectURL(workerJsCode);
        }

        Retyped.dom.Worker daemonThread;
        readonly string[] scriptsToInclude;
        readonly Func<ImAWebWorkerDaemonAction> daemonWorker;
        readonly Func<ImAnAppWireup> webWorkerWireup;
        readonly Blob workerJsCode;
        readonly string workerUrl;

        public Task Start(CancellationToken? cancellationToken = null)
        {
            Start();

            return true.AsTask();
        }

        public Task Stop(CancellationToken? cancellationToken = null)
        {
            End();

            return true.AsTask();
        }

        private void Start()
        {
            if (daemonThread != null)
                return;

            daemonThread = new Retyped.dom.Worker(workerUrl);

            //daemonThread.onmessage = x =>
            //{
            //    if (x.data.ToString() == "WebWorkerThreadDoneExecuting")
            //    {
            //        daemonThread = null;
            //    }
            //};

            //daemonThread.onerror = x =>
            //{
            //    daemonThread = null;
            //};
        }

        public void End()
        {
            Retyped.dom.URL.revokeObjectURL(workerUrl);
            daemonThread.terminate();
            //Script.Write("self.postMessage('WebWorkerThreadDoneExecuting'); self.close();");
        }

        private string PrintWorkerJavaScript()
        {
            string[] jsFiles = new string[] {
                Window.Location.Origin + "/bridge.js",
                Window.Location.Origin + "/bridge.meta.js",
                Window.Location.Origin + "/ProductiveRage.Immutable.js",
                Window.Location.Origin + "/ProductiveRage.Immutable.meta.js",
                Window.Location.Origin + "/newtonsoft.json.js",
                Window.Location.Origin + "/Bridge.React.js",
                Window.Location.Origin + "/Bridge.React.meta.js",
                Window.Location.Origin + "/jquery-2.2.4.js",
                Window.Location.Origin + "/productiveRage.immutable.extensions.js",
                Window.Location.Origin + "/productiveRage.immutable.extensions.meta.js",
                Window.Location.Origin + "/ProductiveRage.ReactRouting.js",
                Window.Location.Origin + "/ProductiveRage.ReactRouting.meta.js",
                Window.Location.Origin + "/dexie.js",
                Window.Location.Origin + "/H.Necessaire.BridgeDotNet.js",
                Window.Location.Origin + "/H.Necessaire.BridgeDotNet.meta.js",
                Window.Location.Origin + "/H.Necessaire.BridgeDotNet.Runtime.ReactApp.js",
                Window.Location.Origin + "/H.Necessaire.BridgeDotNet.Runtime.ReactApp.meta.js",
            }
            .Concat(scriptsToInclude)
            .ToArray()
            ;

            string actionAsString
                = $"" +
                $"onmessage = x => {{ console.log(x); }}" +
                $"{PrintFakeDomForJquery()}" +
                $"{string.Join(string.Empty, jsFiles.Select(x => $"importScripts('{x}');"))} " +
                $"self.dexie = Dexie;" +
                $"H.Necessaire.BridgeDotNet.Runtime.ReactApp.AppBase.MainAsWebWorker({(webWorkerWireup == null ? "null" : $"({ webWorkerWireup})()")});" +
                $"{Environment.NewLine}{Environment.NewLine}" +
                $"var worker = ({daemonWorker})();" +
                $"if (worker) worker.DoWork();" +
                $"{Environment.NewLine}{Environment.NewLine}"
                //+ $"{(isKeptAlive ? string.Empty : "self.postMessage('WebWorkerThreadDoneExecuting'); self.close();")}"
                ;

            return actionAsString;
        }

        private string PrintFakeDomForJquery()
        {
            return @"
            var document = self.document = { isWebWorkerContext : true, webWorkerId : '" + Guid.NewGuid().ToString() + @"', parentNode: null, nodeType: 9, toString: function() {return ""FakeDocument""}};
            var window = self.window = self;
            var fakeElement = Object.create(document);
            fakeElement.nodeType = 1;
            fakeElement.toString = function() { return ""FakeElement""};
            fakeElement.parentNode = fakeElement.firstChild = fakeElement.lastChild = fakeElement;
            fakeElement.ownerDocument = document;

            document.head = document.body = fakeElement;
            document.ownerDocument = document.documentElement = document;
            document.getElementById = document.createElement = function() { return fakeElement; };
            document.createDocumentFragment = function() { return this; };
            document.getElementsByTagName = document.getElementsByClassName = function() { return [fakeElement]; };
            document.getAttribute = document.setAttribute = document.removeChild =
            document.addEventListener = document.removeEventListener =
            function() { return null; };
            document.cloneNode = document.appendChild = function() { return this; };
            document.appendChild = function(child) { return child; };
            ";
        }
    }
}
