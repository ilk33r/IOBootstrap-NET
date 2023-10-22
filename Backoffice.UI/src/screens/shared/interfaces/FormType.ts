import ValidationRule from "../../../presentation/inerfaces/ValidationRule";

interface FormType {

    index: number;
    name: string;
    value: string;
    isEnabled: boolean;
    validations: ValidationRule[];
}

export default FormType;
