window.networkService = {
    isOnline: function () {
        return navigator.onLine;
    },

    registerStatusChange: function(dotNetObj) {
        function update() {
            dotNetObj.invokeMethodAsync('UpdateStatus', navigator.onLine);
        }

        window.addEventListener('online', update);
        window.addEventListener('offline', update);

        window._networkServiceHandlers = { update, dotNetObj };
    },

    unregisterStatusChange: function() {
        if (window._networkServiceHandlers) {
            window.removeEventListener('online', window._networkServiceHandlers.update);
            window.removeEventListener('offline', window._networkServiceHandlers.update);
            window._networkServiceHandlers = null;
        }
    }
};