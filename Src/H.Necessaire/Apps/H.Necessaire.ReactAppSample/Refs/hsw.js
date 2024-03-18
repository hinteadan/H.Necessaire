//H's Service Worker bootstrapper

const serviceWorkerGlobalScope = this;

console.info(this);
console.info(self);
console.info(serviceWorkerGlobalScope);

importScripts(
    'bridge.js',
    'bridge.meta.js',
    'newtonsoft.json.js',
    'H.Necessaire.BridgeDotNet.js',
    'H.Necessaire.BridgeDotNet.meta.js'
);

importScripts('HServiceWorker.js');