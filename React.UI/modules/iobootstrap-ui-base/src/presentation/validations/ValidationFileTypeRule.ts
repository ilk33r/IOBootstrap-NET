import type { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationFileTypeRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    
    private extensions: string[]

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.extensions = [];
    }

    public static initialize(errorTitle: string, errorMessage: string, extensions: string[]): ValidationRule {
        const response = new ValidationFileTypeRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.extensions = extensions;

        return response;
    }

    public validationResult(value: string): boolean {
        let isValid = false;

        this.extensions.forEach(it => {
            const extensionLength = it.length;

            if (value.length < extensionLength) {
                return;
            }

            const lastCharacters = value.slice(-extensionLength);
            if (lastCharacters === it) {
                isValid = true;
            }
        });

        return isValid;
    }
}

export default ValidationFileTypeRule;
