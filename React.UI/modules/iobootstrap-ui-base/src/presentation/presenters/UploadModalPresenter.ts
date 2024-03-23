import { UploadModalViewPresenter } from "../inerfaces/UploadModalViewPresenter";

class UploadModalPresenter implements UploadModalViewPresenter {

    private static _instance: UploadModalPresenter;

    public uploadModalView: UploadModalViewPresenter | undefined;
    
    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public present(): void {
        if (this.uploadModalView !== undefined) {
            this.uploadModalView.present();
        }
    }

    public setProgress(progress: number): void {
        if (this.uploadModalView !== undefined) {
            this.uploadModalView.setProgress(progress);
        }
    }

    public dismiss(): void {
        if (this.uploadModalView !== undefined) {
            this.uploadModalView.dismiss();
        }
    }
}

export default UploadModalPresenter;