import type { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationMinLengthRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    public minLength: number

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.minLength = 0;
    }

    public static initialize(errorTitle: string, errorMessage: string, minLength: number): ValidationRule {
        const response = new ValidationMinLengthRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.minLength = minLength;

        return response;
    }

    public validationResult(value: string): boolean {
        if (value.length > this.minLength) {
            return true;
        }

        return false;
    }
}

export default ValidationMinLengthRule;