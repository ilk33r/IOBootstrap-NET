import IndicatorViewPresenter from "../inerfaces/IndicatorViewPresenter";

class IndicatorPresenter implements IndicatorViewPresenter {

    private static _instance: IndicatorPresenter;

    public indicatorView: IndicatorViewPresenter | undefined;
    
    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public present(): void {
        if (this.indicatorView !== undefined) {
            this.indicatorView.present();
        }
    }

    public dismiss(): void {
        if (this.indicatorView !== undefined) {
            this.indicatorView.dismiss();
        }
    }
}

export default IndicatorPresenter;
