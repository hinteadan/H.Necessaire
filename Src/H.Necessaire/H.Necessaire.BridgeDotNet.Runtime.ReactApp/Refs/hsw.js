//H's Service Worker bootstrapper

const $$serviceWorkerGlobalScope = self || this;

importScripts(
    'bridge.js',
    'bridge.meta.js',
    'newtonsoft.json.js',
    'H.Necessaire.BridgeDotNet.js',
    'H.Necessaire.BridgeDotNet.meta.js'
);

importScripts('HServiceWorker.js');