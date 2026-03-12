window.networkHelper = {
    isOnline: function () {
        return navigator.onLine;
    },

    registerConnectionEvents: function (dotnetHelper) {
        window.addEventListener("online", () => {
            dotnetHelper.invokeMethodAsync("OnOnline");
        });

        window.addEventListener("offline", () => {
            dotnetHelper.invokeMethodAsync("OnOffline");
        });
    }
};