using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace H.Necessaire.WebSockets.Concrete
{
    class ClientNotificationsBehavior : WebSocketBehavior
    {
        readonly ConcurrentDictionary<string, string> sessionPerUser = new ConcurrentDictionary<string, string>();
        public void Initialize(Action<IWebSocketServerToClientOperations> wireup)
        {
            wireup(new WebSocketServerToClientOperations(() => Sessions, GetSessionIdByUserId));
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            string clientId = GetClientIdFromQueryString(Context.QueryString);

            IWebSocketSession userSession = Sessions.Sessions?.LastOrDefault(x => GetClientIdFromQueryString(x.Context.QueryString) == clientId);

            if (userSession != null)
                sessionPerUser.AddOrUpdate(clientId, userSession.ID, (a, b) => userSession.ID);

            CleanupSessions();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            CleanupSessions();
        }

        private static string GetClientIdFromQueryString(NameValueCollection queryString)
        {
            string keyName = null;

            foreach (var key in queryString.Keys)
                if (string.Equals(key.ToString(), "client", StringComparison.InvariantCultureIgnoreCase))
                    keyName = key.ToString();
            if (keyName == null)
                foreach (var key in queryString.Keys)
                    if (string.Equals(key.ToString(), "clientid", StringComparison.InvariantCultureIgnoreCase))
                        keyName = key.ToString();
            if (keyName == null)
                foreach (var key in queryString.Keys)
                    if (string.Equals(key.ToString(), "user", StringComparison.InvariantCultureIgnoreCase))
                        keyName = key.ToString();
            if (keyName == null)
                foreach (var key in queryString.Keys)
                    if (string.Equals(key.ToString(), "userid", StringComparison.InvariantCultureIgnoreCase))
                        keyName = key.ToString();

            if (keyName == null)
                return null;

            return queryString[keyName];
        }

        private string GetSessionIdByUserId(string userId)
        {
            if (!sessionPerUser.ContainsKey(userId))
                return null;

            if (!Sessions.TryGetSession(sessionPerUser[userId], out IWebSocketSession userSession))
                return null;

            return userSession.ID;
        }

        private void CleanupSessions()
        {
            foreach (KeyValuePair<string, string> inactiveEntry in sessionPerUser.Where(x => x.Value.In(Sessions.InactiveIDs)).ToArray())
            {
                sessionPerUser.TryRemove(inactiveEntry.Key, out string inactiveID);
            }

            Sessions.Sweep();
        }
    }
}
