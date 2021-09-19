import ImageVariationsModel from "../models/ImageVariationsModel";

class ImagesListState {

    count: number;
    images: ImageVariationsModel[];

    constructor() {
        this.count = 0;
        this.images = [];
    }
}

export default ImagesListState;
