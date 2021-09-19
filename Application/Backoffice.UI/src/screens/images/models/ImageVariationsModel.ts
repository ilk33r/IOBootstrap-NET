class ImageVariationsModel {

    id: number | null;
    fileName: string;
    width: number | null;
    height: number | null;
    scale: number | null;
    keepRatio: boolean | null;

    constructor() {
        this.id = null;
        this.fileName = "";
        this.width = null;
        this.height = null;
        this.scale = null;
        this.keepRatio = null;
    }
}

export default ImageVariationsModel;
