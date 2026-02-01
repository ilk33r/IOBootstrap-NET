import { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationBackofficeRequestRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
    }

    public static initialize(errorTitle: string, errorMessage: string): ValidationRule {
        const response = new ValidationBackofficeRequestRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;

        return response;
    }

    public validationResult(value: string): boolean {
        if (value === "") {
            return true;
        }
        
        const isValid = /^([a-zA-Z0-9-_@./\\\#\+\ \(\)\?\n\=]+)$/i.test(value);
        const sanitizedString = value.RemoveHTML();
        if (sanitizedString == value) {
            return isValid;
        }

        return false;
    }
}

export default ValidationBackofficeRequestRule;