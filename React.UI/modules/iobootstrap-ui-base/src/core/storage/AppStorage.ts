class AppStorage {

    private static _instance: AppStorage;
    
    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public stringForKey(key: string) : string | null {
        const item = window.localStorage.getItem(key);
        return (item === undefined) ? null : item;
    }

    public setStringForKey(key: string, value: string) {
        window.localStorage.setItem(key, value);
    }

    public removeObject(key: string) {
        window.localStorage.removeItem(key);
    }
}

export default AppStorage;