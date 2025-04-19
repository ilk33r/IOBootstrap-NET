import type { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationFileTypeRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    
    private extension: string

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.extension = "";
    }

    public static initialize(errorTitle: string, errorMessage: string, extension: string): ValidationRule {
        const response = new ValidationFileTypeRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.extension = extension;

        return response;
    }

    public validationResult(value: string): boolean {
        const extensionLength = this.extension.length;
        
        if (value.length < extensionLength) {
            return false;
        }

        const lastCharacters = value.slice(-extensionLength);
        if (lastCharacters === this.extension) {
            return true;
        }

        return false;
    }
}

export default ValidationFileTypeRule;
