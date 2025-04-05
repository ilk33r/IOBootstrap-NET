import { ValidationRule } from "iobootstrap-ui-base";
import { FormTypeChangeHandler } from "./FormTypeChangeHandler";

interface FormType {

    index: number;
    name: string;
    value: string;
    isEnabled: boolean;
    validations: ValidationRule[];
    changeHandler: FormTypeChangeHandler | null;
}

export default FormType;
