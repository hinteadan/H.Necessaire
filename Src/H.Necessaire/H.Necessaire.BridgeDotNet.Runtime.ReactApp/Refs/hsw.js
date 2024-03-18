//H's Service Worker bootstrapper

importScripts(
    'bridge.js',
    'bridge.meta.js',
    'newtonsoft.json.js',
    'H.Necessaire.BridgeDotNet.js',
    'H.Necessaire.BridgeDotNet.meta.js'
);

importScripts('HServiceWorker.js');

(new HServiceWorker.H.Necessaire.BridgeDotNet.Runtime.ReactApp.HServiceWorker()).Main();