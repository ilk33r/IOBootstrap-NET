export interface CalloutViewPresenter {

    show(type: string, title: string, message: string): void;
    dismiss(): void;
}