class FileVariationsModel {

    id: number | null;
    fileName: string;
    fileType: string;
    publicId: string;
    description: string | null;
    additionalData: string | null;
    createdBy: string | null;
    createdDate: string;

    constructor() {
        this.id = null;
        this.fileName = "";
        this.publicId = "";
        this.fileType = "";
        this.description = null;
        this.additionalData = null;
        this.createdBy = null;
        this.createdDate = "";
    }
}

export default FileVariationsModel;
