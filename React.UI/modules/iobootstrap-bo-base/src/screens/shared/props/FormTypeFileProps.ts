import { ValidationRule } from "iobootstrap-ui-base";
import FormType from "../interfaces/FormType";

type FormTypeFileViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypeFileProps {

    index: number;
    inputType: string;
    name: string;
    value: string;
    fileName: string;
    isEnabled: boolean;
    errorHandler: FormTypeFileViewErrorHandler | null;
    validations: ValidationRule[];

    constructor() {
        this.index = 0;
        this.inputType = "text";
        this.name = "";
        this.value = "";
        this.fileName = "";
        this.isEnabled = true;
        this.errorHandler = null;
        this.validations = [];
    }

    static initialize(name: string, value: string, fileName: string, isEnabled: boolean): FormType {
        let response = new FormTypeFileProps();
        response.name = name;
        response.value = value;
        response.fileName = fileName;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, fileName: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypeFileProps();
        response.name = name;
        response.value = value;
        response.fileName = fileName;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }
}

export default FormTypeFileProps;
