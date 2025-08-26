// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

class H {
    static Instance = new H();
    constructor() {

    }

    Debug(test) {
        console.log('Log from h.js');
        console.log(test);
    }
}


export function Debug(test) {
    H.Instance.Debug(test);
}