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

    GetScrollPosition(elementID) {
        const element = document.getElementById(elementID);
        if (!element)
            return null;

        return {
            scrollTop: element.scrollTop,
            scrollLeft: element.scrollLeft,
            scrollHeight: element.scrollHeight,
            clientHeight: element.clientHeight,
        };
    }

    async GetConsumerInfo(id) {
        const runtimePlatform = await this.GetRuntimePlatform();
        const result = {
            ID: id,
            IDTag: `${id}: ${runtimePlatform.UserAgent || window.navigator.userAgent}`,
            DisplayName: runtimePlatform.UserAgent || window.navigator.userAgent,
            Notes: this.GetRawConsumerProperties(),
            RuntimePlatform: runtimePlatform,
        };
        return result;
    }

    GetRawConsumerProperties() {
        let result = [
            { ID: "X-H.Necessaire-Platform", Value: "WebBrowser" },
            { ID: "X-H.Necessaire-Timezone-Difference-To-UTC-InMinutes", Value: new Date().getTimezoneOffset().toString() },
            { ID: "X-H.Necessaire-AsOfUtc-DateTime", Value: new Date().toUTCString() },
            { ID: "X-H.Necessaire-AsOf-Ticks", Value: new Date().getTime().toString() },
            { ID: "X-H.Necessaire-IpAddress", Value: null },
            { ID: "X-H.Necessaire-Port", Value: null },
            { ID: "X-H.Necessaire-Host", Value: null },
            { ID: "X-H.Necessaire-Protocol", Value: null },
            { ID: "X-H.Necessaire-UserAgent", Value: window.navigator.userAgent },
            { ID: "X-H.Necessaire-AiUserID", Value: null },
            { ID: "X-H.Necessaire-Origin", Value: null },
            { ID: "X-H.Necessaire-Referer", Value: null },
        ]

        try {
            result.push(
                { ID: "AppName", Value: window.navigator.appName },
                { ID: "AppVersion", Value: window.navigator.appVersion },
                { ID: "Geolocation", Value: (window.navigator.geolocation != null).toString() },
                { ID: "Language", Value: window.navigator.language },
                { ID: "IsOnline", Value: window.navigator.onLine.toString() },
                { ID: "CPU", Value: window.navigator.oscpu },
                { ID: "Platform", Value: window.navigator.platform },
                { ID: "Product", Value: window.navigator.product },
                { ID: "UserAgent", Value: window.navigator.userAgent },
            );
        }
        catch (err) {
            console.warn(err);
        }

        try {
            result.push(
                { ID: "X-H.Necessaire-DisplayWidth", Value: window.screen.width.toString() },
                { ID: "X-H.Necessaire-DisplayWindowWidth", Value: window.innerWidth.toString() },
                { ID: "X-H.Necessaire-DisplayHeight", Value: window.screen.height.toString() },
                { ID: "X-H.Necessaire-DisplayWindowHeight", Value: window.innerHeight.toString() },
                { ID: "X-H.Necessaire-DisplayPixelDepth", Value: window.screen.pixelDepth.toString() },
                { ID: "X-H.Necessaire-DisplayColorDepth", Value: window.screen.colorDepth.toString() },
            );
        }
        catch (err) {
            console.warn(err);
        }

        try {
            result.push(
                { ID: "Performance-NavigationType", Value: this.PrintNavigationType() },
                { ID: "Performance-NavigationRedirectCount", Value: window.performance.navigation.redirectCount.toString() },
                { ID: "Performance-Timing-ConnectEnd", Value: window.performance.timing.connectEnd.toString() },
                { ID: "Performance-Timing-ConnectStart", Value: window.performance.timing.connectStart.toString() },
                { ID: "Performance-Timing-DomainLookupEnd", Value: window.performance.timing.domainLookupEnd.toString() },
                { ID: "Performance-Timing-DomainLookupStart", Value: window.performance.timing.domainLookupStart.toString() },
                { ID: "Performance-Timing-DomComplete", Value: window.performance.timing.domComplete.toString() },
                { ID: "Performance-Timing-DomContentLoadedEventEnd", Value: window.performance.timing.domContentLoadedEventEnd.toString() },
                { ID: "Performance-Timing-DomContentLoadedEventStart", Value: window.performance.timing.domContentLoadedEventStart.toString() },
                { ID: "Performance-Timing-DomInteractive", Value: window.performance.timing.domInteractive.toString() },
                { ID: "Performance-Timing-DomLoading", Value: window.performance.timing.domLoading.toString() },
                { ID: "Performance-Timing-FetchStart", Value: window.performance.timing.fetchStart.toString() },
                { ID: "Performance-Timing-LoadEventEnd", Value: window.performance.timing.loadEventEnd.toString() },
                { ID: "Performance-Timing-LoadEventStart", Value: window.performance.timing.loadEventStart.toString() },
                { ID: "Performance-Timing-NavigationStart", Value: window.performance.timing.navigationStart.toString() },
                { ID: "Performance-Timing-RedirectEnd", Value: window.performance.timing.redirectEnd.toString() },
                { ID: "Performance-Timing-RedirectStart", Value: window.performance.timing.redirectStart.toString() },
                { ID: "Performance-Timing-RequestStart", Value: window.performance.timing.requestStart.toString() },
                { ID: "Performance-Timing-ResponseEnd", Value: window.performance.timing.responseEnd.toString() },
                { ID: "Performance-Timing-ResponseStart", Value: window.performance.timing.responseStart.toString() },
                { ID: "Performance-Timing-SecureConnectionStart", Value: window.performance.timing.secureConnectionStart.toString() },
                { ID: "Performance-Timing-UnloadEventEnd", Value: window.performance.timing.unloadEventEnd.toString() },
                { ID: "Performance-Timing-UnloadEventStart", Value: window.performance.timing.unloadEventStart.toString() },
            );
        }
        catch (err) {
            console.warn(err);
        }

        return result;
    }

    PrintNavigationType() {
        switch (window.performance.navigation.type.valueOf()) {
            case 0: return "Direct";
            case 1: return "Reload";
            case 2: return "BackOrForward";
            default: return "OtherUnknown";
        }
    }

    async GetRuntimePlatform() {
        try
        {
            const userAgentPlaformInfo = this.GetUserAgentPlaformInfo();

            if (window.navigator.userAgentData && window.navigator.userAgentData.getHighEntropyValues) {
                const highEntropyValues = await window.navigator.userAgentData.getHighEntropyValues(["platformVersion", "platform", "mobile", "brands"]);
                userAgentPlaformInfo.Attributes = highEntropyValues.brands?.map(x => {
                    return {
                        ID: x.brand,
                        Value: x.version,
                    }
                });
                userAgentPlaformInfo.PlatformVersion = highEntropyValues.platformVersion;
                userAgentPlaformInfo.Platform = highEntropyValues.platform;
                userAgentPlaformInfo.IsMobile = highEntropyValues.mobile;
            }

            return userAgentPlaformInfo;
        }
        catch (err)
        {
            console.warn(err);
            return null;
        }
    }

    GetUserAgentPlaformInfo() {
        const platform = this.GetPlatform();
        return {
            UserAgent: window.navigator.userAgent,
            IsWindows: !platform ? null : platform.toLowerCase().indexOf("win") === 0,
            WindowsVersion: null,
            IsMobile: this.GetIsMobile(),
            Platform: platform,
            PlatformVersion: null,
            Attributes: this.GetBrands()?.map(x => {
                return {
                    ID: x.brand,
                    Value: x.version,
                }
            }),
        }
    }

    GetBrands() {
        try { return window.navigator.userAgentData.brands; } catch (err) { console.warn(err); return null; };
    }

    GetIsMobile() {
        try { return window.navigator.userAgentData.mobile; } catch (err) { console.warn(err); return null; };
    }

    GetPlatform() {
        try { return window.navigator.userAgentData.platform; } catch (err) { console.warn(err); return null; };
    }

    SetSessionValue(key, value) {
        sessionStorage.setItem(key, value);
    }

    GetSessionValue(key) {
        return sessionStorage.getItem(key);
    }

    ZapSessionValue(key) {
        sessionStorage.removeItem(key);
    }

    SetLocalValue(key, value) {
        localStorage.setItem(key, value);
    }

    GetLocalValue(key) {
        return localStorage.getItem(key);
    }

    ZapLocalValue(key) {
        localStorage.removeItem(key);
    }

}


export function Debug(test) {
    H.Instance.Debug(test);
}

export function ApplyBranding(branding) {
    H.Instance.ApplyBranding(branding);
}

export function GetScrollPosition(elementID) {
    return H.Instance.GetScrollPosition(elementID);
}

export function GetConsumerInfo(id) {
    return H.Instance.GetConsumerInfo(id);
}

export function SetSessionValue(key, value) {
    return H.Instance.SetSessionValue(key, value);
}

export function GetSessionValue(key) {
    return H.Instance.GetSessionValue(key);
}

export function ZapSessionValue(key) {
    return H.Instance.ZapSessionValue(key);
}

export function SetLocalValue(key, value) {
    return H.Instance.SetLocalValue(key, value);
}

export function GetLocalValue(key) {
    return H.Instance.GetLocalValue(key);
}

export function ZapLocalValue(key) {
    return H.Instance.ZapLocalValue(key);
}
