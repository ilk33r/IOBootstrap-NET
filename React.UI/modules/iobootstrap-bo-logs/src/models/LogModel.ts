class LogModel {

    id: number | null;
    requestDate: string | null;
    ipV4: string | null;
    port: number | null;
    responseCode: number | null;
    requestPath: string | null;
    requestHeaders: string | null;
    responseHeaders: string | null;
    requestBody: string | null;
    responseBody: string | null;

    constructor() {
        this.id = null;
        this.requestDate = null;
        this.ipV4 = null;
        this.port = null;
        this.responseCode = null;
        this.requestPath = null;
        this.requestHeaders = null;
        this.responseHeaders = null;
        this.requestBody = null;
        this.responseBody = null;
    }
}

export default LogModel;
