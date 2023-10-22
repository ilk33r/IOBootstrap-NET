class AppContext {

    private static _instance: AppContext;
    
    private values: { [key: string]: any } = {};
    
    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public numberForKey(key: string): number | null {
        const item = this.values[key];
        return (item === undefined) ? null : item;
    }

    public objectForKey(key: string): any | null {
        const item = this.values[key];
        return (item === undefined) ? null : item;
    }

    public stringForKey(key: string) : string | null {
        const item = this.values[key];
        return (item === undefined) ? null : item;
    }

    public setNumberForKey(key: string, value: number) {
        this.values[key] = value;
    }

    public setObjectForKey(key: string, value: any) {
        this.values[key] = value;
    }

    public setStringForKey(key: string, value: string) {
        this.values[key] = value;
    }
    
    public removeObject(key: string) {
        delete(this.values[key]);
    }
}

export default AppContext;
