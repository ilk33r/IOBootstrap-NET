import { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationMaxDateRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    public maxDate: Date;

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.maxDate = new Date();
    }

    public static initialize(errorTitle: string, errorMessage: string, maxDate: Date): ValidationRule {
        const response = new ValidationMaxDateRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.maxDate = maxDate;

        return response;
    }

    public validationResult(value: string): boolean {
        const date = new Date(value);
        if (date === undefined) {
            return false;
        }

        const dateTimeInterval = Math.floor(date.getTime() / 1000);
        const maxDateTimeInterval = Math.floor(this.maxDate.getTime() / 1000);

        if (dateTimeInterval > maxDateTimeInterval) {
            return false;
        }

        return true;
    }
}

export default ValidationMaxDateRule;