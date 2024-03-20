import FormDataOptionModel from "../models/FormDataOptionModel";
import FormType from "../interfaces/FormType";
import { ValidationRule } from "iobootstrap-ui-base";

type FormTypeSelectViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypeSelectProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    isEnabled: boolean;
    errorHandler: FormTypeSelectViewErrorHandler | null;
    validations: ValidationRule[];
    options: FormDataOptionModel[];

    constructor() {
        this.index = 0;
        this.inputType = "text";
        this.name = "";
        this.value = "";
        this.isEnabled = true;
        this.errorHandler = null;
        this.validations = [];
        this.options = [];
    }

    static initialize(name: string, value: string, isEnabled: boolean, options: FormDataOptionModel[]): FormType {
        let response = new FormTypeSelectProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;
        response.options = options;

        return response;
    }

    static initializeWithValidations(name: string, value: string, isEnabled: boolean, options: FormDataOptionModel[], validations: ValidationRule[]): FormType {
        let response = new FormTypeSelectProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;
        response.validations = validations;
        response.options = options;

        return response;
    }
}

export default FormTypeSelectProps;
