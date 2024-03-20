import { ValidationRule } from "iobootstrap-ui-base";
import FormType from "../interfaces/FormType";

type FormTypeImageViewErrorHandler = (errorTitle: string, errorMessage: string) => void;

class FormTypeImageProps {

    index: number;
    inputType: string;
    name: string;
    value: string;
    fileName: string;
    isEnabled: boolean;
    errorHandler: FormTypeImageViewErrorHandler | null;
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
        let response = new FormTypeImageProps();
        response.name = name;
        response.value = value;
        response.fileName = fileName;
        response.isEnabled = isEnabled;

        return response;
    }

    static initializeWithValidations(name: string, value: string, fileName: string, isEnabled: boolean, validations: ValidationRule[]): FormType {
        let response = new FormTypeImageProps();
        response.name = name;
        response.value = value;
        response.fileName = fileName;
        response.isEnabled = isEnabled;
        response.validations = validations;

        return response;
    }
}

export default FormTypeImageProps;
