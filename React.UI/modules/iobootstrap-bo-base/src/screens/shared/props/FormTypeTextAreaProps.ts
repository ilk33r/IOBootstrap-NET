import { ValidationRule } from "iobootstrap-ui-base";
import FormType from "../interfaces/FormType";

type FormTypeTextAreaViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypeTextAreaProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    isEnabled: boolean;
    errorHandler: FormTypeTextAreaViewErrorHandler | null;
    validations: ValidationRule[];

    constructor() {
        this.index = 0;
        this.inputType = "";
        this.name = "";
        this.value = "";
        this.isEnabled = true;
        this.errorHandler = null;
        this.validations = [];
    }

    static initialize(name: string, value: string, isEnabled: boolean): FormType {
        let response = new FormTypeTextAreaProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypeTextAreaProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }
}

export default FormTypeTextAreaProps;
