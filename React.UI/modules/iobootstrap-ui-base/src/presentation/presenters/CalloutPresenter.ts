import type { CalloutViewPresenter } from "../inerfaces/CalloutViewPresenter";

class CalloutPresenter implements CalloutViewPresenter {

    private static _instance: CalloutPresenter;

    public calloutView: CalloutViewPresenter | undefined;
    
    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public show(type: string, title: string, message: string): void {
        if (this.calloutView !== undefined) {
            this.calloutView.show(type, title, message);
        }
    }

    public dismiss(): void {
        if (this.calloutView !== undefined) {
            this.calloutView.dismiss();
        }
    }
}

export default CalloutPresenter;