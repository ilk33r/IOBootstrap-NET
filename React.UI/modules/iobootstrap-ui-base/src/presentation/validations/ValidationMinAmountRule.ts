import type { ValidationRule } from "../inerfaces/ValidationRule";

class ValidationMinAmountRule implements ValidationRule {

    public errorTitle: string
    public errorMessage: string
    public minAmount: number

    constructor() {
        this.errorTitle = "";
        this.errorMessage = "";
        this.minAmount = 0;
    }

    public static initialize(errorTitle: string, errorMessage: string, minAmount: number): ValidationRule {
        const response = new ValidationMinAmountRule();
        response.errorTitle = errorTitle;
        response.errorMessage = errorMessage;
        response.minAmount = minAmount;

        return response;
    }

    public validationResult(value: string): boolean {
        if (value.length > 0) {
            let numberValue = Number(value);
            if (!isNaN(numberValue) && numberValue > this.minAmount) {
                return true;
            }
        }

        return false;
    }
}

export default ValidationMinAmountRule;