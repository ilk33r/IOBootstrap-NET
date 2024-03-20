class FormDataOptionModel {

    name: string;
    value: string;

    constructor() {
        this.name = "";
        this.value = "";
    }

    static initialize(name: string, value: string): FormDataOptionModel {
        let response = new FormDataOptionModel();
        response.name = name;
        response.value = value;

        return response;
    }
}

export default FormDataOptionModel;
