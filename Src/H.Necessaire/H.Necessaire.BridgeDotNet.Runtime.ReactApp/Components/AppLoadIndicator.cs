using Bridge.Html5;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Components
{
    public class AppLoadIndicator : IDisposable
    {
        private readonly HTMLElement loadingContainer;

        public AppLoadIndicator()
        {
            loadingContainer = new HTMLDivElement();
            DrawAndDisplayLoadingIndicator();
        }

        private void DrawAndDisplayLoadingIndicator()
        {
            loadingContainer.InnerHTML = "<em style='padding: 15px;'>Loading, please wait...</em>";
            Document.Body.AppendChild(loadingContainer);
        }

        public void Dispose()
        {
            loadingContainer.Remove();
        }
    }
}
