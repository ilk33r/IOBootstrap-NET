import { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationRegexRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    public regex: RegExp

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.regex = /^$/i;
    }

    public static initialize(errorTitle: string, errorMessage: string, regex: RegExp): ValidationRule {
        const response = new ValidationRegexRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.regex = regex;

        return response;
    }

    public validationResult(value: string): boolean {
        if (value === "") {
            return true;
        }
        
        return this.regex.test(value);
    }
}

export default ValidationRegexRule;