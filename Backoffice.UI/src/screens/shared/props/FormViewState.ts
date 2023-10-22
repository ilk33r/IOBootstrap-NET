class FormViewState {

    hasError: boolean;
    errorMessage: string;
    inputValue: string;
    imagePreviewURL: string;

    constructor() {
        this.hasError = false;
        this.errorMessage = "";
        this.inputValue = "";
        this.imagePreviewURL = "";
    }
}

export default FormViewState;
