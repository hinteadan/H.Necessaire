//H's Service Worker bootstrapper

const serviceWorkerGlobalScope = self;

importScripts(
    'bridge.js',
    'bridge.meta.js',
    'newtonsoft.json.js',
    'ProductiveRage.Immutable.js',
    'ProductiveRage.Immutable.meta.js',
    'productiveRage.immutable.extensions.js',
    'productiveRage.immutable.extensions.meta.js',
    'H.Necessaire.BridgeDotNet.js',
    'H.Necessaire.BridgeDotNet.meta.js'
);

importScripts('HServiceWorker.js');