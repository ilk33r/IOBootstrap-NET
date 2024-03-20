import { ValidationRule } from "iobootstrap-ui-base";

interface FormType {

    index: number;
    name: string;
    value: string;
    isEnabled: boolean;
    validations: ValidationRule[];
}

export default FormType;
