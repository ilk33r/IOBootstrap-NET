interface FormElement {

    getValue(): string | null;
    getBlobValue(): Blob | null;
}

export default FormElement;
