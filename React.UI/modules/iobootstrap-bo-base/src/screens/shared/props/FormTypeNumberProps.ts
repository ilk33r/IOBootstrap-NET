import { ValidationRule } from "iobootstrap-ui-base";
import FormType from "../interfaces/FormType";
import { FormTypeChangeHandler } from "../interfaces/FormTypeChangeHandler";

class FormTypeNumberProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    isEnabled: boolean;
    validations: ValidationRule[];
    changeHandler: FormTypeChangeHandler | null;

    constructor() {
        this.index = 0;
        this.inputType = "number";
        this.name = "";
        this.value = "";
        this.isEnabled = true;
        this.validations = [];
        this.changeHandler = null;
    }

    static initialize(name: string, value: string, isEnabled: boolean): FormType {
        let response = new FormTypeNumberProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypeNumberProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }

    static initializeWithChangeListener(name: string, value: string, isEnabled: boolean, validations: ValidationRule[], changeHandler: FormTypeChangeHandler): FormType {
        let response = new FormTypeNumberProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;
        response.validations = validations;
        response.changeHandler = changeHandler;

        return response;
    }
}

export default FormTypeNumberProps;
