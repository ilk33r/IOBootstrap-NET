import ListDataItemModel from "../models/ListDataItemModel";

class ListViewState {

    items: ListDataItemModel[] | null;
    filteredItemIndexes: number[] | null;

    constructor() {
        this.items = null;
        this.filteredItemIndexes = null;
    }
}

export default ListViewState;