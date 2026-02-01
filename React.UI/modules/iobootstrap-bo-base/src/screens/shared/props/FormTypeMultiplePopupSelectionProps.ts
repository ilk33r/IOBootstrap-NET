import { ValidationRule } from "iobootstrap-ui-base";
import FormType from "../interfaces/FormType";
import { FormTypeChangeHandler } from "../interfaces/FormTypeChangeHandler";

type FormTypeMultiplePopupSelectionViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypeMultiplePopupSelectionProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    selectedItemIds: number[];
    selectionURL: string;
    isEnabled: boolean;
    errorHandler: FormTypeMultiplePopupSelectionViewErrorHandler | null;
    validations: ValidationRule[];
    changeHandler: FormTypeChangeHandler | null;

    constructor() {
        this.index = 0;
        this.inputType = "text";
        this.name = "";
        this.value = "";
        this.selectedItemIds = [];
        this.selectionURL = "";
        this.isEnabled = true;
        this.errorHandler = null;
        this.validations = [];
        this.changeHandler = null;
    }

    static initialize(name: string, value: string, selectedItemIds: number[], selectionURL: string, isEnabled: boolean): FormType {
        let response = new FormTypeMultiplePopupSelectionProps();
        response.name = name;
        response.value = value;
        response.selectedItemIds = selectedItemIds;
        response.selectionURL = selectionURL;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, selectedItemIds: number[], selectionURL: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypeMultiplePopupSelectionProps();
        response.name = name;
        response.value = value;
        response.selectedItemIds = selectedItemIds;
        response.selectionURL = selectionURL;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }
}

export default FormTypeMultiplePopupSelectionProps;
