(function (document, DotNet) {

    document.addEventListener('keyup', ev => {

        const isEscapeKey = ev.key && ev.key.toLowerCase() === 'escape';

        if (isEscapeKey) {
            DotNet.invokeMethodAsync('H.Necessaire.Runtime.UI.Razor', 'HJsGlobals.EscapeKeyPressed');
        }

    });

})(document, DotNet);