export interface ValidationRule {

    errorTitle: string
    errorMessage: string

    validationResult(value: string): boolean;
}
