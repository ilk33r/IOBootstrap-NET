import FileVariationsModel from "../models/FileVariationsModel";

class FilesListState {

    count: number;
    files: FileVariationsModel[];

    constructor() {
        this.count = 0;
        this.files = [];
    }
}

export default FilesListState;
