export interface UploadModalViewPresenter {

    present(): void;
    setProgress(progress: number): void;
    dismiss(): void;
}