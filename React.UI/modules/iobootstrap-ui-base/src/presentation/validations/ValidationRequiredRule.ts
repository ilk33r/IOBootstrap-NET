import type { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationRequiredRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
    }

    public static initialize(errorTitle: string, errorMessage: string): ValidationRule {
        const response = new ValidationRequiredRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;

        return response;
    }

    public validationResult(value: string): boolean {
        if (value.length > 0) {
            return true;
        }

        return false;
    }
}

export default ValidationRequiredRule;
