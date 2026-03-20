window.offlineService = {
    dbName: "MiProyectoOfflineDB",
    stores: ["Productos", "Queue"],
    db: null,

    initDB: async function () {
        if (this.db) return this.db;
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(this.dbName, 1);

            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                this.stores.forEach(storeName => {
                    if (!db.objectStoreNames.contains(storeName)) {
                        db.createObjectStore(storeName, { keyPath: "id", autoIncrement: true });
                    }
                });
            };

            request.onsuccess = (event) => {
                this.db = event.target.result;
                resolve(this.db);
            };

            request.onerror = (event) => reject(request.error);
        });
    },

    _getStore: async function (storeName, mode) {
        const db = await this.initDB();
        if (!db.objectStoreNames.contains(storeName))
            throw new Error(`Object store ${storeName} no existe`);
        return db.transaction(storeName, mode).objectStore(storeName);
    },

    saveItems: async function (storeName, itemsJson) {
        const items = JSON.parse(itemsJson);
        const store = await this._getStore(storeName, "readwrite");
        store.clear();
        items.forEach(item => store.add(item));
        await new Promise((resolve, reject) => {
            store.transaction.oncomplete = () => resolve(true);
            store.transaction.onerror = () => reject(store.transaction.error);
        });
    },

    getItems: async function (storeName) {
        const store = await this._getStore(storeName, "readonly");
        const getAll = store.getAll();
        return await new Promise((resolve, reject) => {
            getAll.onsuccess = () => resolve(JSON.stringify(getAll.result));
            getAll.onerror = () => reject(getAll.error);
        });
    },

    clearStore: async function (storeName) {
        const store = await this._getStore(storeName, "readwrite");
        store.clear();
    },

    addToQueue: async function (storeName, itemJson) {
        const item = JSON.parse(itemJson);
        const store = await this._getStore(storeName, "readwrite");
        store.add(item);
    },

    getQueue: async function (storeName) {
        const store = await this._getStore(storeName, "readonly");
        const getAll = store.getAll();
        return await new Promise((resolve, reject) => {
            getAll.onsuccess = () => resolve(JSON.stringify(getAll.result));
            getAll.onerror = () => reject(getAll.error);
        });
    }
};