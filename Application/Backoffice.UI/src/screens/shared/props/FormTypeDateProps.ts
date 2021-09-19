import FormType from "../interfaces/FormType";
import ValidationRule from "../../../presentation/inerfaces/ValidationRule";

type FormTypeDateViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypeDateProps implements FormType {

    index: number;
    inputType: string;
    name: string;
    value: string;
    isEnabled: boolean;
    errorHandler: FormTypeDateViewErrorHandler | null;
    validations: ValidationRule[];

    constructor() {
        this.index = 0;
        this.inputType = "date";
        this.name = "";
        this.value = "";
        this.isEnabled = true;
        this.errorHandler = null;
        this.validations = [];
    }

    static initialize(name: string, value: string, isEnabled: boolean): FormType {
        let response = new FormTypeDateProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypeDateProps();
        response.name = name;
        response.value = value;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }
}

export default FormTypeDateProps;
