interface FormElement {

    getValue(): string | null;
    getBlobValue(): Blob | null;
    setValue(value: string): void;
}

export default FormElement;
