import { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationDateRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
    }

    public static initialize(errorTitle: string, errorMessage: string): ValidationRule {
        const response = new ValidationDateRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;

        return response;
    }

    public validationResult(value: string): boolean {
        const date = new Date(value);
        if (date === undefined) {
            return false;
        }

        const today = new Date();
        if (date.getFullYear() > today.getFullYear() + 50) {
            return false;
        }

        return true;
    }
}

export default ValidationDateRule;