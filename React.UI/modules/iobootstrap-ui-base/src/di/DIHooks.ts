export type DIHooksFunction = (param: any | null) => any | null;

class DIHooks {

    private static _instance: DIHooks;
    
    private values: { [key: string]: DIHooksFunction } = {};

    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public hookForKey(key: string) : DIHooksFunction | null {
        const item = this.values[key];
        return (item === undefined) ? null : item;
    }

    public setHookForKey(key: string, value: DIHooksFunction) {
        this.values[key] = value;
    }
}

export default DIHooks;
