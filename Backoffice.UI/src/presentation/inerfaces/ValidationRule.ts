interface ValidationRule {

    errorTitle: string
    errorMessage: string

    validationResult(value: string): boolean;
}

export default ValidationRule;
