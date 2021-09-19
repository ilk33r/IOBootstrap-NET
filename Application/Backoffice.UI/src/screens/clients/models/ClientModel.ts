class ClientModel {

    id: number;
    clientDescription: string;
    isEnabled: boolean;
    requestCount: number;
    maxRequestCount: number;
    clientID: string;
    clientSecret: string;

    constructor() {
        this.id = 0;
        this.clientDescription = "";
        this.isEnabled = false;
        this.requestCount = 0;
        this.maxRequestCount = 0;
        this.clientID = "";
        this.clientSecret = "";
    }
}

export default ClientModel;
