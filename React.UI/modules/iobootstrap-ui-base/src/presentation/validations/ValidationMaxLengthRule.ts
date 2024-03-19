import type { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationMaxLengthRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    public maxLength: number

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.maxLength = 0;
    }

    public static initialize(errorTitle: string, errorMessage: string, maxLength: number): ValidationRule {
        const response = new ValidationMaxLengthRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.maxLength = maxLength;

        return response;
    }

    public validationResult(value: string): boolean {
        if (value.length < this.maxLength) {
            return true;
        }

        return false;
    }
}

export default ValidationMaxLengthRule;
