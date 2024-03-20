import { ValidationRule } from "iobootstrap-ui-base";
import FormType from "../interfaces/FormType";

type FormTypePopupSelectionViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypePopupSelectionProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    selectedItemId: number;
    selectionURL: string;
    isEnabled: boolean;
    errorHandler: FormTypePopupSelectionViewErrorHandler | null;
    validations: ValidationRule[];

    constructor() {
        this.index = 0;
        this.inputType = "text";
        this.name = "";
        this.value = "";
        this.selectedItemId = 0;
        this.selectionURL = "";
        this.isEnabled = true;
        this.errorHandler = null;
        this.validations = [];
    }

    static initialize(name: string, value: string, selectedItemId: number, selectionURL: string, isEnabled: boolean): FormType {
        let response = new FormTypePopupSelectionProps();
        response.name = name;
        response.value = value;
        response.selectedItemId = selectedItemId;
        response.selectionURL = selectionURL;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, selectedItemId: number, selectionURL: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypePopupSelectionProps();
        response.name = name;
        response.value = value;
        response.selectedItemId = selectedItemId;
        response.selectionURL = selectionURL;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }
}

export default FormTypePopupSelectionProps;
