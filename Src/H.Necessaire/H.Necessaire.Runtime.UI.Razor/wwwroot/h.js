// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

class H {
    static Instance = new H();
    constructor() {
        this.html = document.getElementsByTagName("html")[0];
        this.body = document.getElementsByTagName("body")[0];
    }

    Debug(test) {
        console.log('Log from h.js');
        console.log(test);
    }

    ApplyBranding(branding) {
        const { html, body } = this;

        console.log(branding);

        html.style.backgroundColor = branding.backgroundColor;
        body.style.backgroundColor = branding.backgroundColor;
        body.style.fontFamily = branding.fontFamily;
    }
}


export function Debug(test) {
    H.Instance.Debug(test);
}

export function ApplyBranding(branding) {
    H.Instance.ApplyBranding(branding);
}