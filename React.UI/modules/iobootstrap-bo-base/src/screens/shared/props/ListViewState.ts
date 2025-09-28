import ListDataItemModel from "../models/ListDataItemModel";

class ListViewState {

    items: ListDataItemModel[] | null;

    constructor() {
        this.items = null;
    }
}

export default ListViewState;