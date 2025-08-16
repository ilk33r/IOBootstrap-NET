class ListDataItemModel {

    isChild: boolean;
    itemList: string[];
    textWraps: boolean[];

    constructor() {
        this.isChild = false;
        this.itemList = [];
        this.textWraps = [];
    }
}

export default ListDataItemModel;
