import FormType from "../interfaces/FormType";
import ValidationRule from "../../../presentation/inerfaces/ValidationRule";

class FormTypeNumberProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    isEnabled: boolean;
    validations: ValidationRule[];

    constructor() {
        this.index = 0;
        this.inputType = "number";
        this.name = "";
        this.value = "";
        this.isEnabled = true;
        this.validations = [];
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
}

export default FormTypeNumberProps;
